using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PositionFinder))]
public class Barracks : MonoBehaviour
{
    [SerializeField] private Base _botBase;
    PositionFinder _positionFinder;
    private int _quantityBotsInRow;
    private float _quantityRow;
    private Queue<Transform> _positionsInBarracks;

    private void Awake()
    {
        _quantityBotsInRow = 5;
        _positionsInBarracks = new Queue<Transform>();
        //_locationIdentifierForPlacement = GetComponent<LocationIdentifierForPlacement>();
    }

    private void OnEnable()
    {
        //_botBase.WentToBarracks += SendToPosition;
    }

    private void OnDisable()
    {
        //_botBase.WentToBarracks -= SendToPosition;
    }

    private void SendToPosition(Bot bot)
    {
        //_positionFinder.TryFindPlacementPosition(bot.transform.position, 5, out Vector3 resultPosition);
    }

    private Queue<Transform> SetQuantityBots(int quantityBots)
    {
        CalculateNumberOfRows(quantityBots);
        return PutInPlaces(quantityBots);
    }

    private void CalculateNumberOfRows(float quantityBots)
    {
        _quantityRow = quantityBots / _quantityBotsInRow;

        if (quantityBots % _quantityBotsInRow != 0)
        {
            _quantityRow += 1;
        }
    }

    private Queue<Transform> PutInPlaces(float quantityBots)
    {
        float verticalStep = 1.5f;

        float positionX = transform.position.x;

        for (int i = 0; i < _quantityRow; i++)
        {
            transform.position = new Vector3(positionX, transform.position.y, transform.position.z + verticalStep);

            DistributeHorizontally(quantityBots);
        }

        return _positionsInBarracks;
    }

    private void DistributeHorizontally(float quantityBots)
    {
        float horizontalStep = 1.5f;

        for (int j = 0; j < _quantityBotsInRow; j++)
        {
            if (quantityBots == 0)
            {
                return;
            }

            quantityBots--;

            transform.position = new Vector3(transform.position.x + horizontalStep, transform.position.y, transform.position.z);

            _positionsInBarracks.Enqueue(transform);
        }
    }
}