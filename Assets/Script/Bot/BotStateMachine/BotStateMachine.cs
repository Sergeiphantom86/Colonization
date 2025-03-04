using System;
using System.Collections.Generic;
using UnityEngine;

public class BotStateMachine
{
    public BotState CurrentState { get; private set; }

    private readonly Dictionary<Type, BotState> _states = new();

    public void AddState(BotState botState)
    {
        _states.Add(botState.GetType(), botState);
    }

    public void SetState<T>(Vector3 targetPosition) where T : BotState
    {
        var type = typeof(T);

        if (CurrentState != null && CurrentState.GetType() == type)
        {
            return;
        }

        if (_states.TryGetValue(type, out BotState newState))
        {
            CurrentState?.Exit(targetPosition);
            CurrentState = newState;
            CurrentState.Enter(targetPosition);
        }
    }

    public void Update() => CurrentState?.Update();

    public void HandleTrigger(Collider collision) => CurrentState?.HandleTrigger(collision);
}