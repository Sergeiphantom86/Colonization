using UnityEngine;

[RequireComponent(typeof(Base))]
public class SpawnerBots : Spawner<Bot>
{
    private Base _botsBase;

    private new void Awake()
    {
        base.Awake();
         _botsBase = GetComponent<Base>();
    }

    private void OnEnable()
    {
        _botsBase.CreateBot += Create;
    }

    private void OnDisable()
    {
        _botsBase.CreateBot -= Create;
    }

    private Bot Create(Vector3 spawnPosition)
    {
        return Spawn(spawnPosition);
    }
}