using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ResourceMover : MonoBehaviour
{
    protected Rigidbody2D Rigidbody;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Relocate(Vector3 vector)
    {
        //Rigidbody.DOPath(_waypoints, 1);

        transform.DOMove(vector, 1);
    }
}
