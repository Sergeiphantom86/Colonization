using System.Collections.Generic;
using UnityEngine;

public class ScannerArea : MonoBehaviour
{
    public List<Resource> ScanForResources(Vector3 position, float scanRadius, LayerMask targetLayer)
    {
        Collider[] colliders = Physics.OverlapSphere(position, scanRadius, targetLayer);

        return FilterAvailableResources(colliders);
    }

    private List<Resource> FilterAvailableResources(Collider[] colliders)
    {
        List<Resource> result = new List<Resource>();

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Resource>(out var resource) && resource.IsAvailable)
            {
                result.Add(resource);
            }
        }

        return result;
    }
}