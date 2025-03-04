using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Warehouse), typeof(Manipulator))]
public class Base : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _flagPosition;
    [SerializeField] private Flag _flag;
    [SerializeField] private int _requiredBots = 3;
    [SerializeField] private int _flagResourcesRequired = 5;

    private int _currentPayment;
    private int _minQuantityBots;
    private bool _isConstructionFlagSet;

    private Scanner _scanner;
    private Transform _transform;
    private BotManager _botManager;
    private Manipulator _manipulator;
    private BaseStateMachine _stateMachine;

    private Queue<Bot> _bots;
    private Queue<Vector3> _resourceLocations;
    private Vector3 _constructionLocation;

    public event Func<int, bool> CanPay;
    public event Action<int> ResourceSearch;
    public event Action<Resource> OnPutItem;
    public event Func<Vector3, Bot> CreateBot;

    private void Awake()
    {
        _currentPayment = 3;
        _minQuantityBots = 3;
        _transform = transform;
        _bots = new Queue<Bot>();
        _isConstructionFlagSet = false;
        _resourceLocations = new Queue<Vector3>();
        _botManager = new BotManager();
        _scanner = GetComponentInChildren<Scanner>();
        _manipulator = GetComponent<Manipulator>();
    }

    private void Start()
    {
        CreateNewBot();
    }

    private void Update()
    {
        if (_isConstructionFlagSet && _botManager.TotalBots > _minQuantityBots)
        {
            //_stateMachine.SetState<BaseCreationState>();
            _currentPayment = _flagResourcesRequired;
        }

        ProcessBotAssignments();
        HandlePayment();
    }

    private void OnDestroy() => _stateMachine?.Reset();

    private void OnEnable()
    {
        _scanner.HasAppeared += AddResourcePosition;
        _manipulator.OnFlagStateChanged += TrySetFlag;
        _manipulator.OnConstructionPlaced += SetLocationOfConstruction;
    }

    private void OnDisable()
    {
        _scanner.HasAppeared -= AddResourcePosition;
        _manipulator.OnFlagStateChanged -= TrySetFlag;
        _manipulator.OnConstructionPlaced -= SetLocationOfConstruction;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Bot bot))
        {
            RegisterBot(bot);
            
            bot.OnDroppedItem += DropItem;
        }
    }

    public void InitializeBase()
    {
        CanPay = null;
        ResourceSearch = null;
        OnPutItem = null;
        CreateBot = null;

        _resourceLocations.Clear();
        _bots.Clear();
    }

    public Flag GetFlag() => _flag;

    public void ConfigurePayment(int newPayment) => _currentPayment = newPayment;

    private void InitializeStateMachine()
    {
        _stateMachine = new BaseStateMachine();

        _stateMachine.AddState(new BotCreationState(_stateMachine, this));
        _stateMachine.AddState(new BaseCreationState(_stateMachine, this));

        _stateMachine.SetState<BotCreationState>();
    }

    private void RegisterBot(Bot bot)
    {
        if (bot.IsBuilder == false)
        {
            _bots.Enqueue(bot);
            _botManager.RegisterBot(bot);
            bot.transform.SetParent(_transform);

            return;
        }

        _botManager.RemoveBot(bot);
    }

    private void DropItem(Item dropItem)
    {
        if (dropItem is Flag flag)
        {
            PutUpFlag(flag);
        }
        else if (dropItem is Resource resource)
        {
            OnPutItem?.Invoke(resource);
        }
    }

    private void ProcessBotAssignments()
    {
        if (_bots.Count > 0)
        {
            FindResources();
            AssignBotToResource();
        }
    }

    private void AssignBotToResource()
    {
        while (_resourceLocations.Count > 0 && _bots.Count > 0)
        {
            _bots.Peek().OnDroppedItem -= DropItem;

            _bots.Dequeue().SetMovementTarget(_resourceLocations.Dequeue());
        }
    }

    private void HandlePayment()
    {
        if (CanPay?.Invoke(_currentPayment) == false) return;

        if (_isConstructionFlagSet && _botManager.TotalBots > _minQuantityBots)
        {
            ProcessFoundPoint(_constructionLocation);

            return;
        }

        CreateNewBot();
    }

    private void CreateNewBot()
    {
        Bot newBot = CreateBot?.Invoke(_spawnPoint.position);

        if (newBot == null) return;

        newBot.HomeBase = this;

        _bots.Enqueue(newBot);
    }

    private void PutUpFlag(Flag flag)
    {
        flag.transform.SetParent(_flagPosition);
        flag.transform.position = _flagPosition.position;
    }

    private void ProcessFoundPoint(Vector3 targetPosition)
    {
        _resourceLocations.Enqueue(targetPosition);
        //_stateMachine.SetState<BotCreationState>();
        _currentPayment = _requiredBots;
        _isConstructionFlagSet = false;
    }

    private void AddResourcePosition(Vector3 resourcePosition)
    {
        _resourceLocations.Enqueue(resourcePosition);
    }

    private void SetLocationOfConstruction(Vector3 constructionLocation)
    {
        _constructionLocation = constructionLocation;
    }

    private void TrySetFlag(bool isSetFlag)
    {
        _isConstructionFlagSet = isSetFlag;
    }

    private void FindResources()
    {
        if (_resourceLocations.Count <= 0)
        {
            ResourceSearch?.Invoke(_bots.Count);
        }
    }
}