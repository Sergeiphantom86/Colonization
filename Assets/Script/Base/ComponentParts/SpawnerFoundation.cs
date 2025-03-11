using UnityEngine;

public class SpawnerFoundation : Spawner<Foundation>
{
    public Foundation CreateFoundation(Vector3 createPosition)
    {
        return Spawn(createPosition);
    }
}