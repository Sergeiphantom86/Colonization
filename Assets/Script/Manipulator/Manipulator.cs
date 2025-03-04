using System;
using UnityEngine;

[RequireComponent(typeof(InputHandler), typeof(PositionFinder))]
public class Manipulator : MonoBehaviour
{
    private Flag _flag;
    private Camera _mainCamera;
    private InputHandler _inputHandler;
    private PositionFinder _positionFinder;
    private LayerMask _currentLayerMask;
    private StateMachineManipulator _stateMachine;

    public event Action<bool> OnFlagStateChanged;
    public event Action<Vector3> OnConstructionPlaced;

    private void Awake()
    {
        CacheComponents();

        _stateMachine = new StateMachineManipulator();
        _stateMachine.AddState(new StateHasFlag(this));
        _stateMachine.AddState(new StateNoFlag(this));

        _stateMachine.SetState<StateNoFlag>();
    }

    private void OnEnable() => _inputHandler.OnTap += HandleInput;

    private void OnDisable() => _inputHandler.OnTap -= HandleInput;

    public void SwitchLayerMask(LayerMask newMask)
    {
        _currentLayerMask = newMask;
    }

    public void ShangedStatusFlag(bool flagIsSet)
    {
        OnFlagStateChanged?.Invoke(flagIsSet);
    }

    public void SetConstructionPoint(Vector3 constructionPoint)
    {
        OnConstructionPlaced?.Invoke(constructionPoint);
    }
    public bool TryMakePoint(RaycastHit hit)
    {
        return _positionFinder.TryGetNavMeshPosition(hit.point);
    }

    public void MoveFlag(Vector3 position)
    {
        _flag.transform.position = position;
    }

    public void TakeFlag(Flag flag)
    {
        if (flag != null)
        {
            _flag = flag;
            Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            MoveFlag(mousePosition);
            _stateMachine.SetState<StateHasFlag>();
        }
    }

    public void SetFlag(Vector3 position)
    {
        MoveFlag(position);
        SetConstructionPoint(position);
        _stateMachine.SetState<StateNoFlag>();
    }

    private void CacheComponents()
    {
        _mainCamera = Camera.main;
        _inputHandler = GetComponent<InputHandler>();
        _positionFinder = GetComponent<PositionFinder>();
    }

    private void HandleInput()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << _currentLayerMask))
        {
            _stateMachine.StateCurrent.HandleInput(hit);
        }
    }
}