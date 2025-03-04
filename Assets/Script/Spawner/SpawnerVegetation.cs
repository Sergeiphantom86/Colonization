using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RandomSpawnPointProvider))]
public class SpawnerVegetation : Spawner<Vegetation>
{
    [SerializeField] private float _spawnCount;
    [SerializeField] private float _radius;
    [SerializeField] private float _spawnDelay;

    private WaitForSeconds _spawnWait;
    private List<Vegetation> _spawnedVegetation;
    private RandomSpawnPointProvider _randomSpawnPointProvider;

    protected override void Awake()
    {
        base.Awake();

        _spawnedVegetation = new();
        _spawnWait = new WaitForSeconds(_spawnDelay);

        _randomSpawnPointProvider = GetComponent<RandomSpawnPointProvider>();
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 position = _randomSpawnPointProvider.RandomNavSphere(_radius, transform.position);

            if (position != Vector3.zero)
            {
                InitializeVegetation(position);
            }
        }

        yield return _spawnWait;

        EnableTrees();
    }

    private void InitializeVegetation(Vector3 position)
    {
        Vegetation vegetation = Spawn(position);
        vegetation.transform.SetParent(transform);
        vegetation.gameObject.SetActive(false);

        _spawnedVegetation.Add(vegetation);
    }

    private void EnableTrees()
    {
        foreach (var tree in _spawnedVegetation)
        {
            tree.gameObject.SetActive(true);
        }
    }
}