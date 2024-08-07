using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

public class Planner : MonoBehaviour
{
    public ParticleSystem particlePrefab;
    private readonly List<Tuple<Vector3, Vector3>> _debugRayList = new List<Tuple<Vector3, Vector3>>();
    public MeshRenderer _renderer;
    public Material _runMaterial;
    public Transform chestTransform;
    public Transform enemyTransform;
    public Transform knifeTransform;
    public GameObject equippedKnife;
    public TextMeshPro conversationText;

    private bool isMoving = false;
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();

    public float contador;
    public bool StartSequenceBool;
    public Transform currentTarget;

    [SerializeField] PhysicalNodeGrid physicalNodeGrid;
    [SerializeField] Pathfinder pathfinder;
    [SerializeField] Character character; // Suponiendo que Character es el componente que maneja la velocidad

    public Node currentTargetNode;


    MaterialPropertyBlock block;


    public Node lastNode;
    public Node clickNode;

    private void MoveTowardsTarget(Transform target)
    {
        // float step = 5 * Time.deltaTime; // Velocidad del movimiento
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        currentTargetNode = physicalNodeGrid.GetClosest(target.position);
        if (currentTargetNode != null && isMoving)
        {
            StartCoroutine(PathfindAndMove());
        }

        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            isMoving = false;
        }
    }
    // Corutina que usa Pathfinder para encontrar y seguir un camino
    private IEnumerator PathfindAndMove()
    {
        isMoving = true;
        pathfinder.current = physicalNodeGrid.GetClosest(transform.position);
        pathfinder.target = currentTargetNode;
        pathfinder.path = pathfinder.CallPathfind(currentTargetNode);

        if (pathfinder.path.Count > 0)
        {
            pathfinder.current = pathfinder.path[0];
            int targetIndex = 1;

            while (targetIndex < pathfinder.path.Count)
            {
                Node targetNode = pathfinder.path[targetIndex];
                while (Vector3.Distance(targetNode.transform.position, transform.position) > 0.1f)
                {
                    Vector3 direction = (targetNode.transform.position - transform.position).normalized;
                    character.velocity = direction * 2; // Asumiendo que `movementSpeed` es parte del script `Character`
                    yield return null;
                }

                targetIndex++;
                pathfinder.current = targetNode;
            }

            character.velocity = Vector3.zero;
        }

        clickNode = physicalNodeGrid.GetClosest(currentTarget.transform.position);
        if (clickNode != null)
        {

            PathMove();
        }
        /*
        pathfinder.target = currentTargetNode;
        pathfinder.path = pathfinder.CallPathfind(currentTargetNode);
        pathfinder.current = physicalNodeGrid.GetClosest(transform.position);

        if (pathfinder.path.Count > 0)
        {
            int targetIndex = 0;

            while (targetIndex < pathfinder.path.Count)
            {
                Node targetNode = pathfinder.path[targetIndex];
                while (Vector3.Distance(targetNode.transform.position, transform.position) > 0.1f)
                {
                    Vector3 direction = (targetNode.transform.position - transform.position).normalized;
                    character.velocity = direction * 300; // Ajusta la velocidad según sea necesario
                    yield return null;
                }

                targetIndex++;
            }

            character.velocity = Vector3.zero;
            isMoving = false;
        }

        yield return null;*/



    }

    private void PathMove()
    {
        if (pathfinder.current == null)
        {
        }

        lastNode = physicalNodeGrid.GetClosest(/*clickNode.WorldPosition*/currentTargetNode.WorldPosition);//posible error
        pathfinder.current = physicalNodeGrid.GetClosest(transform.position);

        pathfinder.target = lastNode;
        pathfinder.path = pathfinder.CallPathfind(lastNode);

        if (pathfinder.path.Count > 0)
        {
            pathfinder.current = pathfinder.path[0];
        }
        else
        {
            return;
        }

        StartCoroutine(FollowPathAndCheckForPlayer());
        pathfinder.UpdateTarget(lastNode);
    }

    private IEnumerator FollowPathAndCheckForPlayer()
    {
        isMoving = true;
        int targetIndex = 1;

        while (targetIndex < pathfinder.path.Count)
        {
            Node targetNode = pathfinder.path[targetIndex];

            while (Vector3.Distance(targetNode.transform.position, transform.position) > 1f)
            {
                Vector3 dir = (targetNode.transform.position - transform.position).normalized;
                character.velocity = dir * 2;

                yield return null;
            }

            character.velocity = Vector3.zero;

            if (targetIndex < pathfinder.path.Count - 1)
            {
                targetIndex++;
                pathfinder.current = pathfinder.path[targetIndex - 1];
            }
            else
            {
                pathfinder.path.Clear();
                pathfinder.current = null;
                pathfinder.target = null;
                lastNode = null;
                isMoving = false;
                yield break;
            }
        }

        character.velocity = Vector3.zero;
        isMoving = false;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveTowardsTarget(currentTarget); // Asume que siempre va al cofre cuando está en movimiento
        }

        if (StartSequenceBool) return;

        contador += Time.deltaTime;
        if (contador >= 4 /*&& actionQueue.Count<= 5*/)
        {
            StartSequenceBool = true;
            StartCoroutine(ProcessActions());

        }
    }

    public void EnqueHeal()
    {
        EnqueueAction(Heal());
    }

    public IEnumerator Heal()
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        var particles = Instantiate(particlePrefab, this.transform.position, rotation);
        particles.Play();
        yield return null;
    }

    public void GoToChest()
    {
        EnqueueAction(GoToPosition(chestTransform, 10f));

    }

    public void GoToEnemy()
    {
        EnqueueAction(GoToPosition(enemyTransform, 0f, () => Destroy(enemyTransform.gameObject)));
    }

    public void GoToKnife()
    {

        EnqueueAction(GoToPosition(knifeTransform, 0f, () =>
        {
            knifeTransform.gameObject.SetActive(false);
            equippedKnife.SetActive(true);
        }));
    }

    public void StartConversation()
    {
        EnqueueAction(Conversation());
    }

    private IEnumerator GoToPosition(Transform target, float waitTime, Action onArrival = null)
    {

        currentTarget = target;
        isMoving = true;
        yield return new WaitUntil(() => !isMoving);

        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }

        onArrival?.Invoke();
    }
    private IEnumerator Conversation()
    {
        string[] conversationLines = new string[]
        {
            "Hola, ¿cómo estás?",
            "Estoy bien, ¿y tú?",
            "Muy bien, gracias.",
            "Qué bueno, ¿qué has hecho últimamente?",
            "He estado trabajando en un proyecto de Unity.",
            "¡Genial! ¿De qué trata?",
            "Es un juego de aventuras.",
            "Suena interesante, ¡buena suerte!"
        };

        for (int i = 0; i < conversationLines.Length; i++)
        {
            conversationText.text = conversationLines[i];
            yield return new WaitForSeconds(1f);
        }

        conversationText.text = "";
    }

    private void EnqueueAction(IEnumerator action)
    {
        actionQueue.Enqueue(action);
        if (StartSequenceBool == true && actionQueue.Count == 1)
        {
            StartCoroutine(ProcessActions());
        }
    }

    private IEnumerator ProcessActions()
    {
        while (actionQueue.Count > 0)
        {
            yield return StartCoroutine(actionQueue.Dequeue());
        }
    }
    private void Start()
    {
        StartCoroutine(Plan());
    }

    private void Check(Dictionary<string, bool> state, ItemType type)
    {
        var items = Navigation.instance.AllItems();
        var inventories = Navigation.instance.AllInventories();
        var floorItems = items.Except(inventories);
        var item = floorItems.FirstOrDefault(x => x.type == type);
        var here = transform.position;
        state["accessible" + type.ToString()] = item != null && Navigation.instance.Reachable(here, item.transform.position, _debugRayList);

        var inv = inventories.Any(x => x.type == type);
        state["otherHas" + type.ToString()] = inv;

        state["dead" + type.ToString()] = false;
    }

    private IEnumerator Plan()
    {
        yield return new WaitForSeconds(0.2f);

        var observedState = new Dictionary<string, bool>();

        var nav = Navigation.instance;
        var floorItems = nav.AllItems();
        var inventory = nav.AllInventories();
        var everything = nav.AllItems().Union(nav.AllInventories());

        Check(observedState, ItemType.Key);
        Check(observedState, ItemType.Entity);
        Check(observedState, ItemType.Mace);
        Check(observedState, ItemType.PastaFrola);
        Check(observedState, ItemType.Door);
        Check(observedState, ItemType.Pocion);

        GoapState initial = new GoapState
        {
            worldState = new WorldState
            {
                /*playerHP = 88,
                cercaDeItem = true,
                espacioDeInventario = 1,
                energia = 15,
                enRangoDeAtaque = false,
                tieneArmaEquipada = "none",
                tenerPocionDeCuracion = false,
                enCombate = false,
                enUbicacionDeLaMision = false,
                misionCompletada = false,
                */
                IHaveChest = false,
                GoldQuantity = 0,
                TengoArma = "",
                Fervor = 3F,
                Password = false


            }
        };

        var actions = CreatePossibleActionsList();

        GoapState goal = new GoapState
        {
            worldState = new WorldState
            {
                /*misionCompletada = true*/
                IHaveChest = true,
            }
        };

        Func<GoapState, float> heuristic = (curr) =>
        {
            int count = 0;
            if (/*!curr.worldState.misionCompletada*/!curr.worldState.IHaveChest)
            {
                count++;
            }
            return count;
        };

        Func<GoapState, bool> objective = (curr) =>
        {
            return /*curr.worldState.misionCompletada;*/curr.worldState.IHaveChest;
        };

        var plan = Goap.Execute(initial, null, objective, heuristic, actions);

        if (plan == null)
            Debug.Log("Couldn't plan");
        else
        {
            Debug.Log("Plan generated successfully");
            foreach (var action in plan)
            {
                Debug.Log("Action: " + action.Name);
                if (action.Name == "Recoger Cuchillo") GoToKnife();
                else if (action.Name == "Atacar") GoToEnemy();
                else if (action.Name == "Obtener Oro") EnqueHeal();
                else if (action.Name == "Cofre") GoToChest();
                else if (action.Name == "Sobornar") StartConversation();

            }

            var actDict = new Dictionary<string, ActionEntity>
    {
        { "Recoger Cuchillo", ActionEntity.Open },
        { "Atacar", ActionEntity.Kill },
        { "Obtener Oro", ActionEntity.Success },
        { "Cofre", ActionEntity.MoveToPastaFrola },
        { "Sobornar", ActionEntity.PickUp }
    };

            GetComponent<Guy>().ExecutePlan(
                plan.Select(a =>
                {
                    Item i2 = everything.FirstOrDefault(i => i.type == a.item);
                    if (actDict.ContainsKey(a.Name) && i2 != null)
                    {
                        return Tuple.Create(actDict[a.Name], i2);
                    }
                    else
                    {
                        return null;
                    }
                }).Where(a => a != null).ToList()
            );
        }
    }

    private List<GoapAction> CreatePossibleActionsList()
    {
        return new List<GoapAction>
        {
            new GoapAction("Recoger Cuchillo")
                .SetCost(4f)
                .SetItem(ItemType.Key)
                .Pre(gS => /*gS.worldState.playerHP > 50*/ gS.worldState.TengoArma=="")
                .Effect(gS =>
                {
                   /* gS.worldState.espacioDeInventario += 1;
                    gS.worldState.energia -= 1;
                    gS.worldState.tieneArmaEquipada = "Cuchillo";
                    //GoToKnife();
                    Debug.Log("A")*/
                    
                    gS.worldState.TengoArma="Cuchillo"
                    ;
                    return gS;
                }),
            new GoapAction("Atacar")
                .SetCost(4f)
                .SetItem(ItemType.Mace)
                .Pre(gS => /*gS.worldState.playerHP > 5*/ gS.worldState.TengoArma=="Cuchilo")
                .Effect(gS =>
                {
                    /*gS.worldState.playerHP -= 4;
                    gS.worldState.energia -= 5;
                    //GoToEnemy();
                    Debug.Log("b");*/

                      gS.worldState.Password=true;
                      gS.worldState.TengoArma="";


                    return gS;
                }),
            new GoapAction("Obtener Oro")
                .SetCost(1f)
                .Pre(gS => /*gS.worldState.playerHP < 10*/   gS.worldState.Fervor==3f)
                .Effect(gS =>
                {
                   /* gS.worldState.playerHP += 4;
                    gS.worldState.energia -= 2;
                    //EnqueHeal();
                    Debug.Log("c");*/

                      gS.worldState.GoldQuantity=16;

                    return gS;
                }),
            new GoapAction("Cofre")
                .SetCost(2f)
                .Pre(gS => /*gS.worldState.enUbicacionDeLaMision*/   gS.worldState.Password==true )
                .Effect(gS =>
                {
                   /* gS.worldState.enUbicacionDeLaMision = true;
                    gS.worldState.energia -= 15;
                    gS.worldState.misionCompletada = true;
                    //StartConversation();
                    //GoToChest();
                    Debug.Log("d");
                    //_renderer.material= _runMaterial;
                   */
                     gS.worldState.IHaveChest=true;
                    return gS;

                }),
            new GoapAction("Sobornar")
                .SetCost(2f)
                //.SetItem(ItemType.Pocion) WTF??!!!!
                .Pre(gS =>/*gS.worldState.energia >= 2*/   gS.worldState.GoldQuantity>=15 )
                .Effect(gS =>
                {
                    /*
                    gS.worldState.espacioDeInventario -= 5;
                    gS.worldState.energia -= 2;
                    gS.worldState.tenerPocionDeCuracion = true;
                    gS.worldState.enUbicacionDeLaMision=true;

                    //StartConversation();
                    Debug.Log("e");
                    */

                      gS.worldState.Password =true;

                    return gS;
                })
        };
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var t in _debugRayList)
        {
            Gizmos.DrawRay(t.Item1, (t.Item2 - t.Item1).normalized);
            Gizmos.DrawCube(t.Item2 + Vector3.up, Vector3.one * 0.2f);
        }
    }

}













/*
    private readonly List<Tuple<Vector3, Vector3>> _debugRayList = new List<Tuple<Vector3, Vector3>>();

    private void Start()
    {
        StartCoroutine(Plan());
    }

    private void Check(Dictionary<string, bool> state, ItemType type)
    {

        var items = Navigation.instance.AllItems();
        var inventories = Navigation.instance.AllInventories();
        var floorItems = items.Except(inventories);//devuelve una coleccion como la primera pero removiendo los que estan en la segunda
        var item = floorItems.FirstOrDefault(x => x.type == type);
        var here = transform.position;
        state["accessible" + type.ToString()] = item != null && Navigation.instance.Reachable(here, item.transform.position, _debugRayList);

        var inv = inventories.Any(x => x.type == type);
        state["otherHas" + type.ToString()] = inv;

        state["dead" + type.ToString()] = false;
    }

    //No es necesario que sea una corrutina excepto que se calcule GOAP con timeslicing.
    private IEnumerator Plan()
    {
        yield return new WaitForSeconds(0.2f);


        #region Si no se usan objetos modulares, se puede eliminar
        var observedState = new Dictionary<string, bool>();

        var nav = Navigation.instance;//Consigo los items
        var floorItems = nav.AllItems();
        var inventory = nav.AllInventories();
        var everything = nav.AllItems().Union(nav.AllInventories());// .Union() une 2 colecciones sin agregar duplicados(eso incluye duplicados en la misma coleccion)

        //Chequeo los booleanos para cada Item, generando mi modelo de mundo (mi diccionario de bools) en ObservedState
        //Facu: Estos Check buscan los objetos en el mundo y se lo "asignan"/"conecta" al nodo más cercano
        Check(observedState, ItemType.Key);
        Check(observedState, ItemType.Entity);
        Check(observedState, ItemType.Mace);
        Check(observedState, ItemType.PastaFrola);
        Check(observedState, ItemType.Door);
        Check(observedState, ItemType.Pocion);
        #endregion

        GoapState initial = new GoapState(); //Crear GoapState
        initial.worldState = new WorldState()
        {

            playerHP = 88,
            cercaDeItem = true,
            espacioDeInventario = 1,
            energia = 15,
            enRangoDeAtaque = false,
            tieneArmaEquipada = "none",
            tenerPocionDeCuracion = false,
            enCombate = false,
            enUbicacionDeLaMision = false,
            misionCompletada = false,


        };


        //Si uso items modulares:
        //initial.worldState.values = observedState;  //le asigno los valores actuales, conseguidos antes
        //initial.worldState.values["doorOpen"] = false; //agrego el bool "doorOpen"

        //Calculo las acciones
        var actions = CreatePossibleActionsList();

        #region opcional

        #endregion

        //Es opcional, no es necesario buscar por un nodo que cumpla perfectamente con las condiciones
        GoapState goal = new GoapState();

        goal.worldState.misionCompletada= true;


        //Crear la heuristica personalizada para no calcular nodos de mas
        Func<GoapState, float> heuristic = (curr) =>
        {
            int count = 0;

            if (curr.worldState.misionCompletada =! true)// Creo, no estoy seguro
            {
                count++;
            }
            return count;
        };

        //Esto seria el reemplazo de goal, donde se pide que cumpla con las condiciones pasadas.
        Func<GoapState, bool> objective = (curr) =>
         {

             return curr.worldState.misionCompletada = true;
         };

        #region Opcional
        var actDict = new Dictionary<string, ActionEntity>() {
              { "Kill"  , ActionEntity.Kill }
            , { "Pickup", ActionEntity.PickUp }
            , { "Open"  , ActionEntity.Open }
        };
        #endregion

        var plan = Goap.Execute(initial, null, objective, heuristic, actions);

        if (plan == null)
            Debug.Log("Couldn't plan");
        else
        {
            GetComponent<Guy>().ExecutePlan(
                plan
                .Select(a =>
                {
                    Item i2 = everything.FirstOrDefault(i => i.type == a.item);
                    if (actDict.ContainsKey(a.Name) && i2 != null)
                    {
                        return Tuple.Create(actDict[a.Name], i2);
                    }
                    else
                    {
                        return null;
                    }
                }).Where(a => a != null)
                .ToList()
            );
        }
    }

    private List<GoapAction> CreatePossibleActionsList()
    {
        return new List<GoapAction>()
        {

              new GoapAction("Recoger Item/ cuchillo")
                .SetCost(2f)
                .SetItem(ItemType.Key) 

                .Pre((gS)=>
                {

                           return gS.worldState.playerHP > 50 && gS.worldState.cercaDeItem==true&&
                    gS.worldState.espacioDeInventario>0&&gS.worldState.energia>0;
                })
                //Ejemplo de setteo de Effect
                .Effect((gS) =>
                    {

                       gS.worldState.espacioDeInventario+=1; gS.worldState.energia-=1;  gS.worldState.tieneArmaEquipada=("Cuchillo");
                        return gS;
                    }
                )
                , new GoapAction("Atacar")
                .SetCost(5f)
                .SetItem (ItemType.Mace)
                .Pre((gS)=>
                {

                           return gS.worldState.tieneArmaEquipada==("Cuchillo") && gS.worldState.enRangoDeAtaque==true&&
                    gS.worldState.playerHP>5;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.playerHP-=4; gS.worldState.energia-=5;
                        return gS;
                    }
                )

                , new GoapAction("Curarse")
                .SetCost(3f)
                .SetItem (ItemType.Pocion)
                .Pre((gS)=>
                {

                           return gS.worldState.enCombate==false && gS.worldState.tenerPocionDeCuracion==true&&
                    gS.worldState.playerHP<10;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.playerHP+=4; gS.worldState.energia-=2;
                        return gS;
                    }
                )

                , new GoapAction("Correr")
                .SetCost(1f)
                .SetItem (ItemType.PastaFrola)
                .Pre((gS)=>
                {

                           return gS.worldState.enUbicacionDeLaMision==false && gS.worldState.enCombate==false&&
                    gS.worldState.energia>=15;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.enUbicacionDeLaMision=true; gS.worldState.energia-=15; gS.worldState.misionCompletada=true;
                        return gS;
                    }
                )

                , new GoapAction("Lootear")
                .SetCost(2f)
                .Pre((gS)=>
                {

                           return gS.worldState.tenerPocionDeCuracion==false && gS.worldState.espacioDeInventario>=5&&
                    gS.worldState.energia>=2;
                })
                .Effect((gS) =>
                    {

                       gS.worldState.espacioDeInventario-=5; gS.worldState.energia-=2; ; gS.worldState.tenerPocionDeCuracion=true;
                        return gS;
                    }
                )


        };
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (var t in _debugRayList)
        {
            Gizmos.DrawRay(t.Item1, (t.Item2 - t.Item1).normalized);
            Gizmos.DrawCube(t.Item2 + Vector3.up, Vector3.one * 0.2f);
        }
    }*/