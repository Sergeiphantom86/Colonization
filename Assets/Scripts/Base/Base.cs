using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Warehouse), typeof(Manipulator), typeof(NearbyResourceIdentifier))]
public class Base : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _flagPosition;
    [SerializeField] private Flag _flag;
    [SerializeField] private int _resourcesForBot = 3;
    [SerializeField] private int _resourcesForBase = 5;

    private int _minQuantityBots;
    private bool _isConstructionFlagSet;

    private Scanner _scanner;
    private Warehouse _warehouse;
    private Transform _transform;
    private Manipulator _manipulator;
    private NearbyResourceIdentifier _nearbyResourceIdentifier;

    private Queue<Bot> _bots;
    private HashSet<int> _allBots;
    private Queue<Vector3> _resourceLocations;
    private Vector3 _constructionLocation;

    public event Action<Vector3> CreateBot;
    public event Action ResourceSearch;

    private void Awake()
    {
        _minQuantityBots = 3;
        _transform = transform;
        _isConstructionFlagSet = false;

        _bots = new Queue<Bot>();
        _allBots = new HashSet<int>();
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
        _manipulator.OnFlagStateChanged += TrySetFlag;
        _scanner.HasResources += SortByDistanceToTarget;
        _manipulator.OnConstructionPlaced += SetLocationOfConstruction;
    }

    private void OnDisable()
    {
        _manipulator.OnFlagStateChanged -= TrySetFlag;
        _scanner.HasResources -= SortByDistanceToTarget;
        _manipulator.OnConstructionPlaced -= SetLocationOfConstruction;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out Bot bot))
        {
            bot.OnDroppedItem += DropItem;

            RegisterBot(bot);
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

        _bots.Enqueue(newBot);
    }

    private void SortByDistanceToTarget(List<Resource> resources)
    {
        if (resources.Count < 0) return;

        Queue<Resource> sortedResources = _nearbyResourceIdentifier.TryGetNearestResources(resources, transform.position);

        AddNearbyResources(sortedResources);
    }

    private void AddNearbyResources(Queue<Resource> resources)
    {
        for (int i = 0; i < _bots.Count; i++)
        {
            if (resources.TryDequeue(out Resource resource) == false) return;

            resource.MarkAsBusy();

            AddTargetPosition(resource.transform.position);
        }
    }

    private void RegisterBot(Bot bot)
    {
        if (bot == null) return;

        if (bot.IsBuilder)
        {
            _allBots.Remove(bot.GetInstanceID());
        }
        else
        {
            RegistrationEmployeeBots(bot);

            if (_allBots.Contains(bot.GetInstanceID())) return;

            _allBots.Add(bot.GetInstanceID());

        }

        bot.OnDroppedItem -= DropItem;
    }

    private void RegistrationEmployeeBots(Bot bot)
    {
        _bots.Enqueue(bot);
        bot.transform.SetParent(_transform);
    }

    private void DropItem(Item dropItem)
    {
        if (dropItem is Flag flag)
        {
            DropFlag(flag);
        }
        else if (dropItem is Resource resource)
        {
            DropResource(resource);
        }
    }

    private void DropResource(Resource resource)
    {
        _warehouse.AddResource(resource);

        if (_isConstructionFlagSet && _allBots.Count > _minQuantityBots)
        {
            TryCreateBase(_resourcesForBase);
        }
        else
        {
            TryCreateBot(_resourcesForBot);
        }
    }

    private void TryCreateBot(int resourcesForPayment)
    {
        if (TryToPay(resourcesForPayment))
        {
            CreateNewBot();
            UseResources(resourcesForPayment);
        }
    }

    private void TryCreateBase(int resourcesForPayment)
    {
        if (TryToPay(resourcesForPayment))
        {
            _flag = null;
            AddConstructionPoint(_constructionLocation);
            UseResources(resourcesForPayment);
        }
    }

    private bool TryToPay(int requiredAmount)
    {
        return _warehouse.QuantityResources >= requiredAmount;
    }

    private void UseResources(int amount)
    {
        _warehouse.RemoveResource(amount);
    }

    private void AddConstructionPoint(Vector3 targetPosition)
    {
        AddTargetPosition(targetPosition);

        _isConstructionFlagSet = false;
    }

    private void ProcessBotAssignments()
    {
        if (_bots.Count > 0)
        {
            if (_resourceLocations.Count <= 0)
            {
                ResourceSearch?.Invoke();
            }
            else if (_resourceLocations.Count > 0)
            {
                _bots.Dequeue().SetMovementTarget(_resourceLocations.Dequeue());
            }
        }
    }

    private void CreateNewBot()
    {
        CreateBot?.Invoke(_spawnPoint.position);
    }

    private void DropFlag(Flag flag)
    {
        _flag = flag;
        flag.transform.SetParent(_flagPosition);
        flag.transform.position = _flagPosition.position;
    }

    private void AddTargetPosition(Vector3 resourcePosition)
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
}