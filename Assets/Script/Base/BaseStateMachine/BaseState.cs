public abstract class BaseState
{
    protected readonly Base BotBase;
    protected readonly BaseStateMachine StateMachine;

    public BaseState(BaseStateMachine stateMachine, Base botBase)
    {
        StateMachine = stateMachine;
        BotBase = botBase;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
}