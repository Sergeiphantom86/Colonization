using System;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private Bot _bot;

    public Item Item { get; private set; }

    private void OnEnable()
    {
        _bot.OnItemPicked += TakeItem;
        _bot.OnDroppedItem += PutItem;
    }

    private void OnDisable()
    {
        _bot.OnItemPicked -= TakeItem;
        _bot.OnDroppedItem -= PutItem;
    }

    private void TakeItem(Item item)
    {
        item.transform.parent = transform;
        item.transform.position = transform.position;

        Item = item;
    }

    private void PutItem(Item item)
    {
        Item = null;
    }
}