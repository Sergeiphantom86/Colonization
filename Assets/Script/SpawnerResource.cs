using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpawnerResource : Spawner<Resource>
{
    private int _radius;

    private void Start()
    {
        _radius = 7;

        StartCoroutine(WaitForResource());
    }

    private Vector3 RandomNavSphere(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;

        randomDirection += transform.position;

        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, radius, -1);

        return navHit.position;
    }

    private IEnumerator WaitForResource()
    {
        int delay = 2;

        WaitForSeconds wait = new (delay);
        
        for (int i = 0; i <= PoolCapacity; i++)
        {
            yield return wait;

            Spawn(RandomNavSphere(_radius));
        }
    }
}


