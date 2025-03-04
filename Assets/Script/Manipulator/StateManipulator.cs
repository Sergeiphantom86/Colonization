using UnityEngine;

public class StateManipulator
{
    protected const string Terrain = nameof(Terrain);
    protected const string Base = nameof(Base);

    protected Flag Flag;
    protected Manipulator Manipulator;
    protected LayerMask _currentLayerMask;

    public StateManipulator(Manipulator manipulator)
    {
        Manipulator = manipulator;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void HandleInput(RaycastHit hitInfo) { }
}