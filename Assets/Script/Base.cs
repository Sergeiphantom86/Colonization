using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Warehouse))]
public class Base : MonoBehaviour
{
    private List<Bot> _bots;
    private List<Resource> _resourceForCollecting;
    private Warehouse _warehouse;

    private void Awake()
    {
        _warehouse = GetComponent<Warehouse>();
        _resourceForCollecting = new List<Resource>();
        _bots = new List<Bot>();
    }

    private void Update()
    {
        if (_resourceForCollecting.Count > 0 && _bots.Count > 0)
        {
            StartCoroutine(Wait());
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Bot bot))
        {
            Debug.Log($"{bot.name}{bot.Busy}");
            
            if (bot.Busy == false)
            {
                _bots.Add(bot);
            }
        }

        if (collision.TryGetComponent(out Resource resource))
        {
            _resourceForCollecting.Add(resource);
        }
    }

    public void AddResourece(Resource resource)
    {
        _warehouse.AddRecource(resource);
    }

    private IEnumerator Wait()
    {
        int daley = 2;

        WaitForSeconds wait = new WaitForSeconds(daley);

        yield return wait;
        
        while (_resourceForCollecting.Count > 0 && _bots.Count > 0)
        {
            _bots[0].SetResourcePosition(_resourceForCollecting[0].transform.position);
            _bots[0].SetBusyness(true);
            _resourceForCollecting.RemoveAt(0);
            _bots.RemoveAt(0);

            yield return wait;
        }
    }
}