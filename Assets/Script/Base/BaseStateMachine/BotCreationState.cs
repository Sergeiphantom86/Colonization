public class BotCreationState : BaseState
{
    private const int _�onstructionPayment = 3;

    public BotCreationState(BaseStateMachine stateMachine, Base botBase) : base(stateMachine, botBase) { }

    public override void Enter()
    {
        BotBase.ConfigurePayment(_�onstructionPayment);
    }
}