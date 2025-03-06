using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RandomSpawnPointProvider))]
public class SpawnerResource : Spawner<Resource>
{
    [SerializeField] private int _radius;
    [SerializeField] private float _delay;
    [SerializeField] private int _quantityResourses;
    [SerializeField] private int _quantityCycles;

    private WaitForSeconds _wait;
    private WaitForSeconds _waitingBetweenCycles;
    private RandomSpawnPointProvider _randomSpawnPointProvider;

    protected override void Awake()
    {
        base.Awake();

        _wait = new(_delay);
        _waitingBetweenCycles = new(40);
        _randomSpawnPointProvider = GetComponent<RandomSpawnPointProvider>();
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(SpawnCycles(_quantityCycles, _quantityResourses));
    }

    private IEnumerator SpawnCycles(int cyclesCount, int resourcesPerCycle)
    {
        for (int i = 0; i <= cyclesCount; i++)
        {
            yield return StartCoroutine(SpawnResources(resourcesPerCycle));
            yield return _waitingBetweenCycles;
        }
    }

    private IEnumerator SpawnResources(int resourcesCount)
    {
        for (int j = 0; j <= resourcesCount; j++)
        {
            Vector3 position = _randomSpawnPointProvider.RandomNavSphere(_radius, transform.position);

            if (position != Vector3.zero)
            {
                Spawn(position);
                yield return _wait;
            }
        }
    }
}