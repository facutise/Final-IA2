using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using IA2;

public enum ActionEntity
{
	Kill,
    PickUp,
	NextStep,
	FailedStep,
	Open,
	Success
}

public class Guy : MonoBehaviour
{

    /*
	private EventFSM<ActionEntity> _fsm;
    private Item _target;

    private Entity _ent;

	IEnumerable<Tuple<ActionEntity, Item>> _plan;

	private void PerformAttack(Entity us, Item other) {
		Debug.Log("PerformAttack",other.gameObject);
		if(other != _target) return;

		var mace = _ent.items.FirstOrDefault(it => it.type == ItemType.Mace);
		if(mace) {
			other.Kill();
			if(other.type == ItemType.Door)
				Destroy(_ent.Removeitem(mace).gameObject);
			_fsm.Feed(ActionEntity.NextStep);
		}
		else
			_fsm.Feed(ActionEntity.FailedStep);
	}

	private void PerformOpen(Entity us, Item other) {
		if(other != _target) return;

		var key = _ent.items.FirstOrDefault(it => it.type == ItemType.Key);
		var door = other.GetComponent<Door>();
		if(door && key) {
			door.Open();
			Destroy(_ent.Removeitem(key).gameObject);
			_fsm.Feed(ActionEntity.NextStep);
		}
		else
			_fsm.Feed(ActionEntity.FailedStep);
	}

	private void PerformPickUp(Entity us, Item other) {
		if(other != _target) return;

		_ent.AddItem(other);
		_fsm.Feed(ActionEntity.NextStep);
	}

	private void NextStep(Entity ent, Waypoint wp, bool reached) {
		_fsm.Feed(ActionEntity.NextStep);
	}

	private void Awake() {
		_ent = GetComponent<Entity>();

        var any = new State<ActionEntity>("any");

        var idle = new State<ActionEntity>("idle");
        var bridgeStep = new State<ActionEntity>("planStep");
        var failStep = new State<ActionEntity>("failStep");
        var kill = new State<ActionEntity>("kill");
        var pickup = new State<ActionEntity>("pickup");
        var open = new State<ActionEntity>("open");
        var success = new State<ActionEntity>("success");

		kill.OnEnter += a => {
			_ent.GoTo(_target.transform.position);
			_ent.OnHitItem += PerformAttack;
		};

		kill.OnExit += a => _ent.OnHitItem -= PerformAttack;

		failStep.OnEnter += a => { _ent.Stop(); Debug.Log("Plan failed"); };

		pickup.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformPickUp; };
		pickup.OnExit += a => _ent.OnHitItem -= PerformPickUp;

		open.OnEnter += a => { _ent.GoTo(_target.transform.position); _ent.OnHitItem += PerformOpen; };
		open.OnExit += a => _ent.OnHitItem -= PerformOpen;

		bridgeStep.OnEnter += a => {
			var step = _plan.FirstOrDefault();
			if(step != null) {

				_plan = _plan.Skip(1);
				var oldTarget = _target;
				_target = step.Item2;
				if(!_fsm.Feed(step.Item1))
					_target = oldTarget;
			}
			else {
				_fsm.Feed(ActionEntity.Success);
			}
		};

		success.OnEnter += a => { Debug.Log("Success"); };
		success.OnUpdate += () => { _ent.Jump(); };
		
		StateConfigurer.Create(any)
			.SetTransition(ActionEntity.NextStep, bridgeStep)
			.SetTransition(ActionEntity.FailedStep, idle)
			.Done();

		StateConfigurer.Create(bridgeStep)
            .SetTransition(ActionEntity.Kill, kill)
            .SetTransition(ActionEntity.PickUp, pickup)
            .SetTransition(ActionEntity.Open, open)
            .SetTransition(ActionEntity.Success, success)
			.Done();
        
		_fsm = new EventFSM<ActionEntity>(idle, any);
    }

	public void ExecutePlan(List<Tuple<ActionEntity, Item>> plan) {
		_plan = plan;
		_fsm.Feed(ActionEntity.NextStep);
	}

	private void Update ()
    {
		//Never forget
        _fsm.Update();
	}
    */
    private EventFSM<ActionEntity> _fsm;
    private Item _target;
    private Entity _ent;
    private IEnumerable<Tuple<ActionEntity, Item>> _plan;

    private void PerformAction(Entity us, Item other, ActionEntity action)
    {
        if (other != _target) return;

        switch (action)
        {
            case ActionEntity.Kill:
                PerformKill(us, other);
                break;
            case ActionEntity.PickUp:
                PerformPickUp(us, other);
                break;
            case ActionEntity.Open:
                PerformOpen(us, other);
                break;
            default:
                _fsm.Feed(ActionEntity.FailedStep);
                break;
        }
    }

    private void PerformKill(Entity us, Item other)
    {
        var mace = _ent.items.FirstOrDefault(it => it.type == ItemType.Mace);
        if (mace)
        {
            other.Kill();
            if (other.type == ItemType.Door)
                Destroy(_ent.Removeitem(mace).gameObject);
            _fsm.Feed(ActionEntity.NextStep);
        }
        else
            _fsm.Feed(ActionEntity.FailedStep);
    }

    private void PerformOpen(Entity us, Item other)
    {
        var key = _ent.items.FirstOrDefault(it => it.type == ItemType.Key);
        var door = other.GetComponent<Door>();
        if (door && key)
        {
            door.Open();
            Destroy(_ent.Removeitem(key).gameObject);
            _fsm.Feed(ActionEntity.NextStep);
        }
        else
            _fsm.Feed(ActionEntity.FailedStep);
    }

    private void PerformPickUp(Entity us, Item other)
    {
        _ent.AddItem(other);
        _fsm.Feed(ActionEntity.NextStep);
    }

    private void NextStep()
    {
        if (_plan == null || !_plan.Any())
        {
            _fsm.Feed(ActionEntity.Success);
            return;
        }

        var next = _plan.First();
        _plan = _plan.Skip(1);

        _target = next.Item2;

        Navigation.instance.TryReach(
            transform,
            _target.transform.position,
            (success) =>
            {
                if (success)
                    PerformAction(_ent, _target, next.Item1);
                else
                    _fsm.Feed(ActionEntity.FailedStep);
            }
        );
    }

    private void Awake()
    {
        var idle = new State<ActionEntity>("idle");
        var bridgeStep = new State<ActionEntity>("bridgeStep");
        var failStep = new State<ActionEntity>("failStep");
        var kill = new State<ActionEntity>("kill");
        var pickup = new State<ActionEntity>("pickup");
        var open = new State<ActionEntity>("open");
        var success = new State<ActionEntity>("success");

        StateConfigurer.Create(idle)
            .SetTransition(ActionEntity.NextStep, bridgeStep)
            .SetTransition(ActionEntity.Success, success)
            .Done();

        StateConfigurer.Create(bridgeStep)
            .SetTransition(ActionEntity.Kill, kill)
            .SetTransition(ActionEntity.PickUp, pickup)
            .SetTransition(ActionEntity.Open, open)
            .SetTransition(ActionEntity.FailedStep, failStep)
            .Done();

        StateConfigurer.Create(kill)
            .SetTransition(ActionEntity.NextStep, idle)
            .Done();

        StateConfigurer.Create(pickup)
            .SetTransition(ActionEntity.NextStep, idle)
            .Done();

        StateConfigurer.Create(open)
            .SetTransition(ActionEntity.NextStep, idle)
            .Done();

        StateConfigurer.Create(failStep)
            .SetTransition(ActionEntity.NextStep, bridgeStep)
            .Done();

        StateConfigurer.Create(success)
            .Done();

        _fsm = new EventFSM<ActionEntity>(idle);
    }

    public void ExecutePlan(IEnumerable<Tuple<ActionEntity, Item>> plan)
    {
        _plan = plan;
        _fsm.Feed(ActionEntity.NextStep);
    }

    void Update()
    {
        _fsm.Update();
    }

}
