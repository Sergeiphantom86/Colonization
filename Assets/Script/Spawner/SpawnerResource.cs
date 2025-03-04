using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RandomSpawnPointProvider))]
public class SpawnerResource : Spawner<Resource>
{
    [SerializeField] private int _radius;
    [SerializeField] private float _delay;
    [SerializeField] private int _quantityResourses;

    private WaitForSeconds _wait;
    private WaitForSeconds _waitLongTime;
    private RandomSpawnPointProvider _randomSpawnPointProvider;

    protected override void Awake()
    {
        base.Awake();

        _wait = new(_delay);
        _waitLongTime = new(40);
        _randomSpawnPointProvider = GetComponent<RandomSpawnPointProvider>();
    }

    private IEnumerator Start()
    {
        for (int j = 0; j <= 10; j++)
        {
            for (int i = 0; i <= _quantityResourses; i++)
            {
                Vector3 position = _randomSpawnPointProvider.RandomNavSphere(_radius, transform.position);

                if (position != Vector3.zero)
                {
                    Spawn(position);

                    yield return _wait;
                }
            }

            yield return _waitLongTime;
        }
    }
}