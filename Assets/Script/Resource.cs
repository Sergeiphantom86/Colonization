using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(ResourceMover))]
public class Resource : MonoBehaviour
{
    private Collider _collider;
    private Rigidbody _rigidbody;
    private ResourceMover _resourceMover;

    public bool NewResource {  get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _resourceMover = GetComponent<ResourceMover>();
        NewResource = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Hand hand))
        {
            TurnOnIsKinematic();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Hand hand))
        {
            TurnOffIsKinematic();
        }
    }

    public void TurnOffCollider()
    {
        _collider.enabled = false;
    }

    public void Relocate(Vector3 vector)
    {
        _resourceMover.Relocate(vector);
    }

    private void TurnOnIsKinematic()
    {
        _rigidbody.isKinematic = true;
    }

    private void TurnOffIsKinematic()
    {
        _rigidbody.isKinematic = false;
    }
}