using UnityEngine;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T _prefab;
    private ObjectPool<T> _pool;

    protected virtual void Awake()
    {
        _pool = new ObjectPool<T>(CreateItem);
    }

    public T Spawn(Vector3 position)
    {
        var item = _pool.Get();
        item.transform.position = position;
        item.gameObject.SetActive(true);
        return item;
    }

    private T CreateItem()
    {
        var item = Instantiate(_prefab);
        item.gameObject.SetActive(false);
        return item;
    }
}