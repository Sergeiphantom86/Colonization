public class BaseCreationState : BaseState
{
    private const int _ñonstructionPayment = 5;

    public BaseCreationState(BaseStateMachine stateMachine, Base botBase) : base(stateMachine, botBase) { }

    public override void Enter()
    {
        BotBase.ConfigurePayment(_ñonstructionPayment);
    }
}