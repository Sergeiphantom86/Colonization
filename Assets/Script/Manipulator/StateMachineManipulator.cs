using System;
using System.Collections.Generic;

public class StateMachineManipulator
{
    public StateManipulator StateCurrent { get; private set; }

    private readonly Dictionary<Type, StateManipulator> _states = new();

    public void AddState(StateManipulator state)
    {
        _states.Add(state.GetType(), state);
    }

    public void SetState<T>() where T : StateManipulator
    {
        var type = typeof(T);

        if (StateCurrent != null && StateCurrent.GetType() == type)
        {
            return;
        }

        if (_states.TryGetValue(type, out var newState))
        {
            StateCurrent?.Exit();

            StateCurrent = newState;

            StateCurrent.Enter();
        }
    }
}