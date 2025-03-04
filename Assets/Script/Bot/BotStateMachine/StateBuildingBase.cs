using UnityEngine.AI;
using UnityEngine;

public class StateBuildingBase : BotState
{
    public StateBuildingBase(BotStateMachine stateMachine, Bot bot, NavMeshAgent agent) : base(stateMachine, bot, agent) { }

    public override void Enter(Vector3 position)
    {
        _agent.SetDestination(_bot.StartPosition);
    }

    public override void HandleTrigger(Collider collision)
    {
        if (collision.TryGetComponent(out Foundation foundation))
        {
            _bot.SetConstructionState(isBuilder: false);
            _bot.StartConstructionBase();
        }
    }
}