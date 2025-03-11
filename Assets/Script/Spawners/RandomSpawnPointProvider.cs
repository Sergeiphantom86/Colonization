using UnityEngine;

[RequireComponent(typeof(PositionFinder))]
public class RandomSpawnPointProvider : MonoBehaviour
{
    [SerializeField] private int _maxAttempts = 10;
    private PositionFinder _positionFinder;

    private void Awake()
    {
        _positionFinder = GetComponent<PositionFinder>();
    }

    public Vector3 RandomNavSphere(float radius, Vector3 centralPoint)
    {
        int attempts = 0;
        bool positionFound = false;
        Vector3 resultPosition = centralPoint;

        while (attempts < _maxAttempts && positionFound == false)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            Vector3 randomPosition = centralPoint + randomDirection;

            if (_positionFinder.TryGetNavMeshPosition(randomPosition))
            {
                resultPosition = _positionFinder.LastValidPosition;
                positionFound = true;
            }

            attempts++;
        }

        return resultPosition;
    }
}