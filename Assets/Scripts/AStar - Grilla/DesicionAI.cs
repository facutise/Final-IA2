﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesicionAI : MonoBehaviour
{
    [SerializeField] ViewDetection detect;
    [SerializeField] Character character;
    [SerializeField] PhysicalNodeGrid grid;
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] Transform player;
    [SerializeField] Renderer render;
    [SerializeField] List<Node> path = new List<Node>();
    int currentNodeIndex = 0;
    MaterialPropertyBlock block;
    public State currentState;
    [SerializeField] Pathfinder pathfinder;
    private bool playerInSight = false;
    private Node lastKnownPlayerNode;
    public static List<DesicionAI> allGuardians = new List<DesicionAI>();
    float attackDistance = 2.0f;
    private static bool anyGuardInReturnState = false;
    bool cleanOne = false;

    public enum State
    {
        Patrol,
        Chase,
        Attack,
        ReturnToLastKnownPosition,
        HomeComing
    }

    private void Awake()
    {
        block = new MaterialPropertyBlock { };
        allGuardians.Add(this);
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (detect.InFieldOfView(player.position))
                {
                    ChangeState(State.Chase);
                }
                break;
            case State.Chase:
                Chase();
                if (!detect.InFieldOfView(player.position))
                {
                    ChangeState(State.ReturnToLastKnownPosition);
                }
                else if (Vector3.Distance(transform.position, player.position) < attackDistance)
                {
                    ChangeState(State.Attack);
                }
                break;
            case State.Attack:
                Attack();
                if (Vector3.Distance(transform.position, player.position) > attackDistance)
                {
                    ChangeState(State.Chase);
                }
                break;
            case State.ReturnToLastKnownPosition:
                ReturnToLastKnownPosition();
                if (detect.InFieldOfView(lastKnownPlayerNode.WorldPosition) && Vector3.Distance(transform.position, lastKnownPlayerNode.WorldPosition) < 1.0f && !detect.InFieldOfView(player.position))
                {
                    ChangeState(State.HomeComing);
                }
                else if (detect.InFieldOfView(player.position))
                {
                    ChangeState(State.Chase);
                }
                break;
            case State.HomeComing:
                HomeComing();
                if (detect.InFieldOfView(path[0].WorldPosition) && Vector3.Distance(transform.position, path[0].WorldPosition) < 1.0f && !detect.InFieldOfView(player.position))
                {
                    ChangeState(State.Patrol);
                }
                else if (detect.InFieldOfView(player.position))
                {
                    ChangeState(State.Chase);
                }
                break;
            default:
                break;
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    private void HomeComing()
    {

        if (cleanOne == false)
        {
            pathfinder.path.Clear();
            Debug.Log("Clean");
            cleanOne = true;
        }

        block.SetColor("_Color", Color.magenta);
        render.SetPropertyBlock(block);

        if (pathfinder.current == null)
        {
            lastKnownPlayerNode = grid.GetClosest(path[0].WorldPosition);
            pathfinder.current = grid.GetClosest(transform.position);
        }
        lastKnownPlayerNode = grid.GetClosest(path[0].WorldPosition);

        currentNodeIndex = 0;

        pathfinder.target = lastKnownPlayerNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownPlayerNode);

        pathfinder.current = pathfinder.path.Count > 0 ? pathfinder.path[0] : null;

        Vector3 current = transform.position;

        /*foreach (var node in pathfinder.path)  // Linea para checkear la vuelta
        {
            Debug.DrawLine(current, node.WorldPosition, Color.red, 99);
            current = node.WorldPosition;
        }*/

        character.velocity = Vector3.zero;

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastKnownPlayerNode);
    }

    private void ReturnToLastKnownPosition()
    {
        block.SetColor("_Color", Color.yellow);
        render.SetPropertyBlock(block);

        if (!anyGuardInReturnState)
        {
            anyGuardInReturnState = true;

            foreach (DesicionAI guard in allGuardians)
            {
                guard.ChangeState(State.ReturnToLastKnownPosition);
            }
        }

        if (pathfinder.current == null)
        {
            lastKnownPlayerNode = grid.GetClosest(player.position);
            pathfinder.current = grid.GetClosest(transform.position);
        }

        currentNodeIndex = 0;

        pathfinder.target = lastKnownPlayerNode;
        pathfinder.path = pathfinder.CallPathfind(lastKnownPlayerNode);

        pathfinder.current = pathfinder.path.Count > 0 ? pathfinder.path[0] : null;

        Vector3 current = transform.position;

        /*foreach (var node in pathfinder.path)  //Linea para checkear la ida
        {
            Debug.DrawLine(current, node.WorldPosition, Color.blue, 99);
            current = node.WorldPosition;
        }*/

        character.velocity = Vector3.zero;

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastKnownPlayerNode);

        foreach (DesicionAI guard in allGuardians)
        {
            guard.lastKnownPlayerNode = lastKnownPlayerNode;
        }
    }

    private IEnumerator FollowPathAndCheckForPlayer()
    {
        int targetIndex = 1;

        while (targetIndex < pathfinder.path.Count)
        {
            Node targetNode = pathfinder.path[targetIndex];

            while (Vector3.Distance(targetNode.transform.position, transform.position) > 1f)
            {
                Vector3 dir = (targetNode.transform.position - transform.position);
                character.velocity = dir.normalized * moveSpeed;

                if (detect.InFieldOfView(player.position))
                {
                    ChangeState(State.Chase);
                    yield break;
                }

                yield return null;
            }

            character.velocity = Vector3.zero;

            pathfinder.current = targetNode;


            if (targetIndex < pathfinder.path.Count)
            {
                pathfinder.target = pathfinder.path[targetIndex];
            }
            else
            {
                if (pathfinder.target == lastKnownPlayerNode)
                {
                    lastKnownPlayerNode = null;
                    yield break;
                }

                pathfinder.target = path[0];

                pathfinder.path = pathfinder.CallPathfind(pathfinder.target);

                if (pathfinder.path.Count == 0)
                {
                    yield break;
                }

                targetIndex = 0;
            }

            yield return new WaitForSeconds(0.1f);
        }

        character.velocity = Vector3.zero;
    }

    private void Patrol()
    {
        block.SetColor("_Color", Color.blue);
        render.SetPropertyBlock(block);

        if (path != null && path.Count > 0)
        {
            Node targetNode = path[currentNodeIndex];
            Vector3 dir = (targetNode.transform.position - transform.position);

            if (Vector3.Distance(targetNode.transform.position, transform.position) < 1f)
            {
                character.velocity = Vector3.zero;
                pathfinder.current = targetNode;
                currentNodeIndex++;

                if (currentNodeIndex < path.Count)
                {
                    pathfinder.target = path[currentNodeIndex];
                }
                else
                {
                    currentNodeIndex = 0;
                    pathfinder.target = path[currentNodeIndex];
                    pathfinder.UpdateTarget(path[currentNodeIndex]);
                }
            }
            else
            {
                character.velocity = dir.normalized * moveSpeed;
            }
        }
        else
        {
            character.velocity = Vector3.zero;
        }
    }

    private void Chase()
    {
        Node previousTarget = pathfinder.target;
        pathfinder.current = null;
        pathfinder.target = null;
        pathfinder.path.Clear();
        lastKnownPlayerNode = grid.GetClosest(player.position);
        block.SetColor("_Color", Color.green);
        render.SetPropertyBlock(block);
        var dir = player.position - transform.position;
        character.velocity = dir.normalized * moveSpeed;
        anyGuardInReturnState = false;
    }

    private void Attack()
    {
        block.SetColor("_Color", Color.red);
        render.SetPropertyBlock(block);
        character.velocity = Vector3.zero;
    }
}