using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Warehouse), typeof(Manipulator), typeof(NearbyResourceIdentifier))]
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
    private Warehouse _warehouse;
    private Transform _transform;
    private Manipulator _manipulator;
    private NearbyResourceIdentifier _nearbyResourceIdentifier;

    private Queue<Bot> _bots;
    private List<Bot> _allBots;
    private Queue<Vector3> _resourceLocations;
    private Vector3 _constructionLocation;

    public event Action<Vector3> CreateBot;
    public event Action ResourceSearch;
    public event Action<Resource, int> OnPutItem;
    public event Action<List<Resource>, Vector3> HasSortedResources;

    private void Awake()
    {
        _currentPayment = 3;
        _minQuantityBots = 3;
        _transform = transform;
        _isConstructionFlagSet = false;

        _bots = new Queue<Bot>();
        _allBots = new List<Bot>();
        _resourceLocations = new Queue<Vector3>();

        _manipulator = GetComponent<Manipulator>();
        _scanner = GetComponentInChildren<Scanner>();
        _warehouse = GetComponentInChildren<Warehouse>();
        _nearbyResourceIdentifier = GetComponent<NearbyResourceIdentifier>();

        _manipulator.Initialize(this);
    }

    private void Start()
    {
        CreateNewBot();

        _flag.Initialize(name);
    }

    private void Update()
    {
        ProcessBotAssignments();
    }

    private void OnEnable()
    {
        _warehouse.CanCreate += HandlePayment;
        _manipulator.OnFlagStateChanged += TrySetFlag;
        _scanner.HasResources += SortByDistanceToTarget;
        _manipulator.OnConstructionPlaced += SetLocationOfConstruction;
        _nearbyResourceIdentifier.SortedResources += AssignBotToResource;
    }

    private void OnDisable()
    {
        _warehouse.CanCreate -= HandlePayment;
        _manipulator.OnFlagStateChanged -= TrySetFlag;
        _scanner.HasResources -= SortByDistanceToTarget;
        _manipulator.OnConstructionPlaced -= SetLocationOfConstruction;
        _nearbyResourceIdentifier.SortedResources -= AssignBotToResource;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Bot bot))
        {
            RegisterBot(bot);

            bot.OnDroppedItem += DropItem;
        }
    }

    public bool TryGetFlag(out Flag flag)
    {
        flag = _flag;
        return flag != null;
    }

    public void AddNewBot(Bot newBot)
    {
        if (newBot == null) return;

        newBot.HomeBase = this;

        _bots.Enqueue(newBot);
    }

    private void SortByDistanceToTarget(List<Resource> resources)
    {
        if (resources.Count < 0) return;

        HasSortedResources?.Invoke(resources, _transform.position);
    }

    private void AssignBotToResource(Queue<Resource> resources)
    {
        if (resources.TryDequeue(out Resource resource) == false) return;

        for (int i = 0; i < _bots.Count; i++)
        {
            _bots.Peek().OnDroppedItem -= DropItem;

            resource.MarkAsBusy();
            _bots.Dequeue().SetMovementTarget(resource.transform.position);
        }
    }

    private void RegisterBot(Bot bot)
    {
        if (bot.IsBuilder == false)
        {
            _bots.Enqueue(bot);
            bot.transform.SetParent(_transform);

            if (bot == null || _allBots.Contains(bot)) return;

            _allBots.Add(bot);
        }

        _allBots.Remove(bot);
    }

   

    private void DropItem(Item dropItem)
    {
        if (dropItem is Flag flag)
        {
            PutUpFlag(flag);
        }
        else if (dropItem is Resource resource)
        {
            OnPutItem?.Invoke(resource, _currentPayment);
        }
    }

    private void ProcessBotAssignments()
    {
        if (_bots.Count > 0)
        {
            FindResources();
        }
    }

    private void TryChangeStateOfBots()
    {
        if (_isConstructionFlagSet && _allBots.Count > _minQuantityBots)
        {
            _currentPayment = _flagResourcesRequired;
            _flag = null;
        }
    }

    private void HandlePayment()
    {
        TryChangeStateOfBots();

        if (_isConstructionFlagSet && _allBots.Count > _minQuantityBots)
        {
            ProcessFoundPoint(_constructionLocation);

            return;
        }

        CreateNewBot();
    }

    private void CreateNewBot()
    {
        CreateBot?.Invoke(_spawnPoint.position);
    }

    private void PutUpFlag(Flag flag)
    {
        _flag = flag;
        flag.transform.SetParent(_flagPosition);
        flag.transform.position = _flagPosition.position;
    }

    private void ProcessFoundPoint(Vector3 targetPosition)
    {
        AddResourcePosition(targetPosition);
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
            ResourceSearch?.Invoke();
        }
    }
}