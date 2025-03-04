using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Transform _hand;
    private NavMeshAgent _agent;
    private Vector3 _startPosition;
    private Vector3 _resourcePosition;
    private bool _isLoaded;
    private bool _onRoute;

    public bool Busy { get; private set; }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _startPosition = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Resource resource))
        {
            if (resource.transform.position == _resourcePosition)
            {
                InteractWithResource(resource, true, _hand.transform);

                resource.transform.position = _hand.transform.position;
            }

            SetResourcePosition(_startPosition);
        }

        if (_isLoaded)
        {
            if (collision.TryGetComponent(out Base botBase))
            {
                TransferResource(botBase, gameObject.GetComponentInChildren<Resource>());
            }
        }
    }

    public void SetBusyness(bool busy)
    {
        Busy = busy;
    }

    public void SetResourcePosition(Vector3 position)
    {
        if (_onRoute == false)
        {
            _agent.SetDestination(position);
            _resourcePosition = position;
            _onRoute = true;
        }
    }

    private void TransferResource(Base botBase, Resource resource)
    {
        botBase.AddResourece(resource);
        resource.TurnOffCollider();
        SetBusyness(false);
        resource.Relocate(PlaceRandomPosition(_startPosition));
        InteractWithResource(resource, false, botBase.transform);
    }

    private void InteractWithResource(Resource resource, bool isLoaded, Transform transform)
    {
        _isLoaded = isLoaded;

        resource.transform.parent = transform;

        _onRoute = false;
    }

    private Vector3 PlaceRandomPosition(Vector3 baseTransform)
    {
        return new Vector3(GetRandomPositionNumber(baseTransform.x), _startPosition.y, GetRandomPositionNumber(baseTransform.z));
    }

    private float GetRandomPositionNumber(float position)
    {
        float square = 1;

        return Random.Range(position, position + square);
    }
}