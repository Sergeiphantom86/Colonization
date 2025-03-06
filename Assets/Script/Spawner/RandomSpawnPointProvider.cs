using UnityEngine;

[RequireComponent(typeof(PositionFinder))]
public class RandomSpawnPointProvider : MonoBehaviour
{
    private PositionFinder _positionFinder;

    private void Awake()
    {
        _positionFinder = GetComponent<PositionFinder>();
    }

    public Vector3 RandomNavSphere(float radius, Vector3 centralPoint)
    {
        Vector3 randomPosition = Random.insideUnitSphere * radius;

        randomPosition += centralPoint;

        if (_positionFinder.TryGetNavMeshPosition(randomPosition))
        {
            return _positionFinder.LastValidPosition;
        }

        return Vector3.zero;
    }
}