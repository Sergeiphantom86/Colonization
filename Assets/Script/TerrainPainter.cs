using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainPainter : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    [SerializeField] private float _brushSize = 5f;
    [SerializeField] private int _textureIndex = 1;
    [SerializeField] private float _opacity = 0.1f;

    private TerrainData _terrainData;
    private int _alphamapWidth;
    private int _alphamapHeight;
    private Vector3 _terrainSize;
    private Vector3 _lastPosition;
    private float[,,] originalAlphamapData;

    private void Start()
    {
        if (_terrain == null) _terrain = Terrain.activeTerrain;

        _terrainData = _terrain.terrainData;
        _alphamapWidth = _terrainData.alphamapWidth;
        _alphamapHeight = _terrainData.alphamapHeight;
        _terrainSize = _terrainData.size;
        _lastPosition = transform.position;

        originalAlphamapData = _terrainData.GetAlphamaps(0, 0, _alphamapWidth, _alphamapHeight);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _lastPosition) > 0.5f)
        {
            Paint();
            _lastPosition = transform.position;
        }
    }

    private void OnDisable()
    {
        ResetTerrain();
    }

    private void Paint()
    {
        Vector3 worldPos = transform.position;
        Vector3 terrainPos = _terrain.transform.position;

        Vector2 normalizedPos = new Vector2(
            (worldPos.x - terrainPos.x) / _terrainSize.x,
            (worldPos.z - terrainPos.z) / _terrainSize.z);

        int brushPixelSize = (int)(_brushSize * _alphamapWidth / _terrainSize.x);

        int pixelX = (int)(normalizedPos.x * _alphamapWidth);
        int pixelY = (int)(normalizedPos.y * _alphamapHeight);

        int startX = Mathf.Max(0, pixelX - brushPixelSize);
        int startY = Mathf.Max(0, pixelY - brushPixelSize);
        int sizeX = Mathf.Min(2 * brushPixelSize, _alphamapWidth - startX);
        int sizeY = Mathf.Min(2 * brushPixelSize, _alphamapHeight - startY);

        if (sizeX == 0 || sizeY == 0) return;

        float[,,] alphamapData = _terrainData.GetAlphamaps(startX, startY, sizeX, sizeY);

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                float distance = Vector2.Distance(
                    new Vector2(x, y),
                    new Vector2(pixelX - startX, pixelY - startY));

                if (distance < brushPixelSize)
                {
                    float falloff = 1 - Mathf.Clamp01(distance / brushPixelSize);
                    ApplyPaint(ref alphamapData, x, y, falloff * _opacity);
                }
            }
        }

        _terrainData.SetAlphamaps(startX, startY, alphamapData);
    }

    private void ApplyPaint(ref float[,,] alphamapData, int x, int y, float strength)
    {
        List<float> original = GetOriginalValuesWithTotal(alphamapData, x, y, out float total);
        NormalizeOriginalValues(original, total);
        UpdateAlphaData(ref alphamapData, x, y, CalculateTarget(original, strength, _textureIndex),
                       original, _textureIndex, GetOthersTotal(original, _textureIndex));
    }

    private List<float> GetOriginalValuesWithTotal(float[,,] data, int x, int y, out float total)
    {
        List<float> values = new ();
        for (int i = 0; i < data.GetLength(2); i++) values.Add(data[y, x, i]);
        total = values.Sum();
        return values;
    }

    private void NormalizeOriginalValues(List<float> values, float total)
    {
        if (total <= 0) return;
        for (int i = 0; i < values.Count; i++) values[i] /= total;
    }

    private float CalculateTarget(List<float> values, float strength, int index)
    {
        float target = values[index] + strength;
        return Mathf.Clamp01(target);
    }

    private float GetOthersTotal(List<float> values, int excludedIndex)
    {
        float total = 0;
        for (int i = 0; i < values.Count; i++) if (i != excludedIndex) total += values[i];
        return total;
    }

    private void UpdateAlphaData(ref float[,,] data, int x, int y, float targetValue,
                        List<float> original, int targetIndex, float othersTotal)
    {
        float remaining = 1 - targetValue;
        for (int i = 0; i < original.Count; i++)
            data[y, x, i] = (i == targetIndex) ?
                targetValue :
                (othersTotal > 0 ? original[i] * remaining / othersTotal : 0);
    }

    public void ResetTerrain()
    {
        if (originalAlphamapData != null)
        {
            _terrainData.SetAlphamaps(0, 0, originalAlphamapData);
        }
    }
}