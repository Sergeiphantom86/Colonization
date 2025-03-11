using UnityEngine;
using UnityEngine.AI;

public class StateReturningToBase : BotState
{
    public StateReturningToBase(BotStateMachine stateMachine, Bot bot, NavMeshAgent agent) : base(stateMachine, bot, agent) { }

    public override void Enter(Vector3 startPosition)
    {
        _agent.SetDestination(startPosition);
    }

    public override void Exit(Vector3 position)
    {
        _agent.speed = _bot.BaseSpeed;
    }

    public override void HandleTrigger(Collider collision)
    {
        if (collision.TryGetComponent(out Base _))
        {
            _bot.DropItem();
        }
    }
}