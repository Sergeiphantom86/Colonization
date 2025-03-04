using System;
using System.Collections.Generic;

public class BaseStateMachine
{
    public BaseState StateCurrent { get; private set; }

    private readonly Dictionary<Type, BaseState> _states = new ();

    public void AddState(BaseState state)
    {
        _states.Add(state.GetType(), state);
    }

    public void SetState<T>() where T : BaseState
    {
        var type = typeof(T);

        if (StateCurrent != null && StateCurrent.GetType() == type)
        {
            return;
        }

        if (_states.TryGetValue(type, out var newState ))
        {
            StateCurrent?.Exit();

            StateCurrent = newState;    

            StateCurrent.Enter();
        }
    }

    public void Reset()
    {
        StateCurrent?.Exit();
        StateCurrent = null;
        _states.Clear();
    }
}