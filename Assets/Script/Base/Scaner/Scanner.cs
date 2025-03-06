using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(ScannerArea))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private LayerMask _freeResource;
    [SerializeField] private Base _botBase;
    [SerializeField] private float _rescanCooldown = 2f;

    private Coroutine _scanningRoutine;
    private float _scanRadius;
    private Transform _transform;
    private ScannerArea _scannerArea;

    public event Action<Vector3> HasAppeared;
    public event Func<List<Resource>, Vector3, List<Resource>> HasSortedResources;

    private void Awake()
    {
        _scannerArea = GetComponent<ScannerArea>();
        _scanRadius = 100;
        _transform = transform;
    }

    private void OnEnable() => _botBase.ResourceSearch += SetRequiredAmountResources;
    private void OnDisable() => _botBase.ResourceSearch -= SetRequiredAmountResources;

    private void SetRequiredAmountResources(int requiredAmount)
    {
        if (_scanningRoutine != null)
            StopCoroutine(_scanningRoutine);

        _scanningRoutine = StartCoroutine(ContinuousScanning(requiredAmount));
    }

    private IEnumerator ContinuousScanning(int requiredAmount)
    {
        int totalProcessed = 0;

        while (totalProcessed < requiredAmount)
        {
            List<Resource> resources = _scannerArea.ScanForResources(_transform.position, _scanRadius, _freeResource)
                .Where(resource => resource.IsAvailable)
                .ToList();

            if (HasSortedResources != null)
            {
                resources = HasSortedResources(resources, _transform.position);
            }

            int remaining = requiredAmount - totalProcessed;
            ProcessResources(resources, remaining, ref totalProcessed);

            yield return new WaitForSeconds(_rescanCooldown);
        }

        _scanningRoutine = null;
    }

    private void ProcessResources(List<Resource> resources, int remainingAmount, ref int totalProcessed)
    {
        int countToProcess = Mathf.Min(remainingAmount, resources.Count);

        for (int i = 0; i < countToProcess; i++)
        {
            totalProcessed++;
            resources[i].MarkAsBusy();
            resources[i].HighlitResource();
            HasAppeared?.Invoke(resources[i].transform.position);
        }
    }
}