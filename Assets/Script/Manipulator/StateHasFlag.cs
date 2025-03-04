using UnityEngine;

public class StateHasFlag : StateManipulator
{
    public StateHasFlag(Manipulator manipulator) : base(manipulator) { }

    public override void Enter()
    {
        Manipulator.SwitchLayerMask(LayerMask.NameToLayer(Terrain));
    }

    public override void Exit()
    {
        Manipulator.ShangedStatusFlag(true);
    }

    public override void HandleInput(RaycastHit hitInfo)
    {
        if (Manipulator.TryMakePoint(hitInfo))
        {
            
            
            Manipulator.SetFlag(hitInfo.point);
        }
    }
}