using UnityEngine;
using UnityEngine.AI;

public class PositionFinder : MonoBehaviour 
{
    private float _maxSampleDistance = 10;

    public Vector3 LastValidPosition { get; private set; }

    public bool TryGetNavMeshPosition(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit navHit, _maxSampleDistance, NavMesh.AllAreas) == false)
            return false;

        if (IsValidPosition(position) == false)
            return false;

        LastValidPosition = navHit.position;
        return true;
    }

    private bool IsValidPosition(Vector3 position)
    {
        return !float.IsNaN(position.x) &&
               !float.IsNaN(position.y) &&
               !float.IsNaN(position.z) &&
               !float.IsInfinity(position.x) &&
               !float.IsInfinity(position.y) &&
               !float.IsInfinity(position.z);
    }
}