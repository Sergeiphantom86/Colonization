using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ScannerArea))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private LayerMask _freeResource;
    [SerializeField] private Base _botBase;

    private float _scanRadius;
    private Transform _transform;
    private ScannerArea _scannerArea;

    private List<Resource> _resources;

    public event Action<List<Resource>> HasResources;

    private void Awake()
    {
        _scannerArea = GetComponent<ScannerArea>();
        _scanRadius = 100;
        _transform = transform;
    }

    private void OnEnable() => _botBase.ResourceSearch += SetRequiredAmountResources;
    private void OnDisable() => _botBase.ResourceSearch -= SetRequiredAmountResources;

    private void SetRequiredAmountResources()
    {
        _resources = _scannerArea.ScanForResources(_transform.position, _scanRadius, _freeResource)
                .Where(resource => resource.IsAvailable)
                .ToList();

        HasResources?.Invoke(_resources);
    }
}