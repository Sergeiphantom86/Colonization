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
    private HashSet<Resource> _processingResources;

    public event Action<Vector3> HasAppeared;
    public event Func<List<Resource>, Vector3, List<Resource>> HasSortedResources;

    private void Awake()
    {
        _scannerArea = GetComponent<ScannerArea>();
        _scanRadius = 30;
        _processingResources = new HashSet<Resource>();
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
        while (true)
        {
            List<Resource> resources = _scannerArea.ScanForResources(_transform.position, _scanRadius, _freeResource);

            resources = TryGetNearestResource(resources, transform.position);

            ProcessResources(resources, requiredAmount);
            yield return new WaitForSeconds(_rescanCooldown);
        }
    }

    public List<Resource> TryGetNearestResource(List<Resource> detectedResources, Vector3 positionBase)
    {
         return detectedResources
            .Where(resource => resource != null)
            .OrderBy(resource => Vector3.Distance(resource.transform.position, positionBase))
            .ToList();
    }

    private void ProcessResources(List<Resource> resources, int requiredAmount)
    {
        int countToProcess = Mathf.Min(requiredAmount, resources.Count);
        
        for (int i = 0; i < countToProcess; i++)
        {
            if (resources[i].IsAvailable)
            {
                resources[i].MarkAsBusy();
                resources[i].HighlitResource();
                HasAppeared?.Invoke(resources[i].transform.position);
            }
        }
    }
}
