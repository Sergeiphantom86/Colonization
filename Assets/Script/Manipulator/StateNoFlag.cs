using UnityEngine;

public class StateNoFlag : StateManipulator
{
    public StateNoFlag(Manipulator manipulator) : base(manipulator) { }

    public override void Enter()
    {
        Manipulator.SwitchLayerMask(LayerMask.NameToLayer(Base));
    }

    public override void Exit()
    {
        Manipulator.ShangedStatusFlag(false);
    }

    public override void HandleInput(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out Base botBase))
        {
            Flag = botBase.GetFlag();
            Manipulator.TakeFlag(Flag);
        }
    }
}