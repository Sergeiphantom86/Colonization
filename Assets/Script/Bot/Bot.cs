using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Hand _hand;

    private bool _isBuilder;
    private Transform _transform;
    private Vector3 _startPosition;
    private readonly float _baseSpeed = 5;
    private readonly float _loadedSpeed = 3;
    private readonly float _turningSpeed = 0.1f;
    private float _delay;

    private NavMeshAgent _agent;
    private BotStateMachine _stateMachine;
    private WaitForSeconds _waitingForTurn;

    public Base HomeBase { get; set; }
    public bool IsBuilder { get { return _isBuilder; } }
    public float BaseSpeed { get { return _baseSpeed; } }
    public Vector3 StartPosition { get { return _startPosition; } }

    public event Action<Item> OnItemPicked;
    public event Action<Item> OnDroppedItem;
    public event Action<Vector3> OnBuild;
    public event Action<Vector3> OnBuildFoundation;

    private void Awake()
    {
        _delay = 1.5f;
        _transform = transform;
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = true;
        _stateMachine = new BotStateMachine();
        _waitingForTurn = new WaitForSeconds(_delay);

        _stateMachine.AddState(new StateMovingToItem(_stateMachine, this, _agent));
        _stateMachine.AddState(new StateReturningToBase(_stateMachine, this, _agent));
        _stateMachine.AddState(new StateBuildingBase(_stateMachine, this, _agent));
    }

    private void Start()
    {
        SaveStartPosition(_transform.position);
    }

    private void OnTriggerEnter(Collider collider)
    {
        _stateMachine.HandleTrigger(collider);
    }

    public void StartConstructionBase()
    {
        OnBuild?.Invoke(_transform.position);
    }

    public void StartConstructionFoundation()
    {
        OnBuildFoundation?.Invoke(_transform.position);
    }

    public void SetConstructionState(bool isBuilder)
    {
        _isBuilder = isBuilder;
    }

    public void SetMovementTarget(Vector3 itemPosition)
    {
        _stateMachine.SetState<StateMovingToItem>(itemPosition);
    }

    public void SaveStartPosition(Vector3 startPosition)
    {
        _startPosition = startPosition;
    }

    public void PickItem(Item item)
    {
        OnItemPicked?.Invoke(item);

        _stateMachine.SetState<StateReturningToBase>(_transform.parent.position);
    }

    public void DropItem()
    {
        Item droppedItem = _hand.Item;

        if (droppedItem == null) return;

        OnDroppedItem?.Invoke(droppedItem);

        if (IsBuilder)
        {
            _stateMachine.SetState<StateBuildingBase>(StartPosition);
        }
    }

    public void Turn()
    {
        StartCoroutine(WaitTurn());
    }

    private IEnumerator WaitTurn()
    {
        _agent.speed = _turningSpeed;

        yield return _waitingForTurn;

        _agent.speed = _loadedSpeed;
    }
}