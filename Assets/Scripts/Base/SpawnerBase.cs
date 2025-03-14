using UnityEngine;

public class SpawnerBase : Spawner<Base>
{
    public Base CreateNewBase(Vector3 createBase)
    {
        return Spawn(createBase);
    }
}