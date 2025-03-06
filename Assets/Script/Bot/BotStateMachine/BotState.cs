using UnityEngine;
using UnityEngine.AI;

public class BotState
{
    protected readonly Bot _bot;
    protected readonly NavMeshAgent _agent;
    protected readonly BotStateMachine _stateMachine;

    public BotState(BotStateMachine stateMachine, Bot bot, NavMeshAgent agent)
    {
        _bot = bot;
        _agent = agent;
        _stateMachine = stateMachine;
    }

    public virtual void Enter(Vector3 target) => _agent.SetDestination(target);

    public virtual void Exit(Vector3 position) { }

    public virtual void HandleTrigger(Collider collision) { }
}