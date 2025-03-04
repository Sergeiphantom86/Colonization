public class BaseCreationState : BaseState
{
    private const int _�onstructionPayment = 5;

    public BaseCreationState(BaseStateMachine stateMachine, Base botBase) : base(stateMachine, botBase) { }

    public override void Enter()
    {
        BotBase.ConfigurePayment(_�onstructionPayment);
    }
}