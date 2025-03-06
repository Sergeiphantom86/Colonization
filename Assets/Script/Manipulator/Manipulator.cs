using System;
using UnityEngine;

[RequireComponent(typeof(InputHandler), typeof(PositionFinder))]
public class Manipulator : MonoBehaviour
{
    [SerializeField] private LayerMask _baseLayerMask;
    [SerializeField] private LayerMask _terainLayerMask;
    [SerializeField] private LayerMask _currentLayerMask;

    private Flag _flag;
    private Camera _mainCamera;
    private InputHandler _inputHandler;
    private PositionFinder _positionFinder;
    private Base _parentBase;

    public event Action<bool> OnFlagStateChanged;
    public event Action<Vector3> OnConstructionPlaced;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _inputHandler = GetComponent<InputHandler>();
        _positionFinder = GetComponent<PositionFinder>();

        _currentLayerMask = _baseLayerMask;
    }

    private void OnEnable() => _inputHandler.OnTap += HandleInput;

    private void OnDisable() => _inputHandler.OnTap -= HandleInput;

    public void Initialize(Base parentBase)
    {
        _parentBase = parentBase;
    }

    private void HandleInput()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _currentLayerMask))
        {
            bool isBase = hit.collider.TryGetComponent(out Base clickedBase);
            bool isValidNavMesh = _positionFinder.TryGetNavMeshPosition(hit.point);

            switch (true)
            {
                case true when isBase:
                    TakeFlag(clickedBase, flagIsSet: false);
                    break;

                case true when isValidNavMesh:
                    SetFlag(flagIsSet: true);
                    break;
            }
        }
    }

    private void TakeFlag(Base clickedBase, bool flagIsSet)
    {
        if (clickedBase.TryGetFlag(out Flag flag) == false) return;

        if (flag == null) return;

        _flag = flag;
        MoveFlag(_mainCamera.ScreenToWorldPoint(Input.mousePosition), _terainLayerMask, flagIsSet);

    }

    private void SetFlag(bool flagIsSet)
    {
        if (_flag.NameParentBase != _parentBase.name) return;

        MoveFlag(_positionFinder.LastValidPosition, _baseLayerMask, flagIsSet);
    
        OnConstructionPlaced?.Invoke(_positionFinder.LastValidPosition);
    }

    private void MoveFlag(Vector3 position, LayerMask layerMask, bool flagIsSet)
    {
        AssignPosition(position);
        SwitchLayerMasks(layerMask);
        OnFlagStateChanged?.Invoke(flagIsSet);
    }

    private void AssignPosition(Vector3 position)
    {
        _flag.transform.position = position;
    }

    private void SwitchLayerMasks(LayerMask layerMask)
    {
        _currentLayerMask = layerMask;
    }
}