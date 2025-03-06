using System.Collections;
using UnityEngine;

public class Resource : Item
{
    [SerializeField] private LayerMask _freeLayer;
    [SerializeField] private LayerMask _busyLayer;
    [SerializeField] private ParticleSystem _particleSystem;

    private bool _isAvailable = true;

    private void Start()
    {
        _particleSystem.Stop();
    }

    public bool IsAvailable
    {
        get => _isAvailable;
        set
        {
            _isAvailable = value;
            gameObject.layer = value ?
                (int)Mathf.Log(_freeLayer.value, 2) :
                (int)Mathf.Log(_busyLayer.value, 2);
        }
    }

    public void MarkAsBusy() => IsAvailable = false;

    public void HighlitResource()
    {
        if (_particleSystem != null)
        {
            _particleSystem.Play();
            StartCoroutine(DisableResource());
        }
    }

    private IEnumerator DisableResource()
    {
        yield return new WaitForSeconds(20);

        _particleSystem.Stop();
        _isAvailable = true;
        gameObject.layer = (int)Mathf.Log(_freeLayer.value, 2);
    }
}