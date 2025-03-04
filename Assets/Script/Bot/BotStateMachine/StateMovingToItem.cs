using UnityEngine;
using UnityEngine.AI;

public class StateMovingToItem : BotState
{
    private Vector3 _targetPosition;
    private const float PositionThreshold = 0.1f;

    public StateMovingToItem(BotStateMachine botStateMachine, Bot bot, NavMeshAgent agent) : base(botStateMachine, bot, agent) { }

    public override void Enter(Vector3 startPosition)
    {
        _agent.speed = _bot.BaseSpeed;
        _agent.stoppingDistance = 0.1f;

        _targetPosition = startPosition;
        _agent.SetDestination(_targetPosition);
    }

    public override void Exit(Vector3 position)
    {
        _agent.speed = _bot.LoadedSpeed;
    }

    public override void HandleTrigger(Collider collision)
    {
        if (IsValidCollision(collision, out Item item) == false) return;

        if (IsWithinInteractionRange(item.transform.position))
        {
            HandleItemInteraction(item);
        }
    }

    private bool IsValidCollision(Collider collision, out Item item)
    {
        item = collision.GetComponent<Item>();
        return item != null;
    }

    private bool IsWithinInteractionRange(Vector3 itemPosition)
    {
        return Vector3.Distance(itemPosition, _targetPosition) < PositionThreshold;
    }

    private void HandleItemInteraction(Item item)
    {
        if (item is Flag flag)
        {
            ProcessFlagInteraction(flag);
        }

        PickUpItem(item);
    }

    private void ProcessFlagInteraction(Flag flag)
    {
        _bot.SetConstructionState(isBuilder: true);
        _bot.StartConstructionFoundation();
        _bot.SaveStartPosition(flag.transform.position);
    }

    private void PickUpItem(Item item)
    {
        _bot.PickItem(item);
        item.TurnOffCollider();
    }
}