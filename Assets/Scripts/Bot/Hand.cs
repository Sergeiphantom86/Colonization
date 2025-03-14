using UnityEngine;

public class Hand : MonoBehaviour
{
    public Item Item { get; private set; }

    public void TakeItem(Item item)
    {
        item.transform.parent = transform;
        item.transform.position = transform.position;

        Item = item;
    }

    public void PutItem(Item item)
    {
        Item = null;
    }
}