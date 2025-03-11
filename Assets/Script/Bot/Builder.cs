using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Bot), typeof(ScannerArea), typeof(SpawnerFoundation))]
[RequireComponent(typeof(SpawnerBase))]
public class Builder : MonoBehaviour
{
    [SerializeField] private float _constructionDelay = 1f;
    [SerializeField] private LayerMask _treeLayer;

    private Bot _bot;
    private Foundation _foundation;
    private SpawnerBase _spawnerBase;
    private WaitForSeconds _constructionWait;
    private SpawnerFoundation _spawnerFoundation;

    private void Awake()
    {
        _bot = GetComponent<Bot>();
        _spawnerBase = GetComponent<SpawnerBase>();
        _spawnerFoundation = GetComponent<SpawnerFoundation>();
        _constructionWait = new WaitForSeconds(_constructionDelay);
    }

    private void OnEnable()
    {
        _bot.OnBuild += HandleConstructionBase;
        _bot.OnBuildFoundation += HandleConstructionFoundation;
    }
    private void OnDisable()
    {
        _bot.OnBuild -= HandleConstructionBase;
        _bot.OnBuildFoundation -= HandleConstructionFoundation;
    }

    private void HandleConstructionBase(Vector3 constructionSite)
    {
        StartCoroutine(BuildBaseRoutine(constructionSite));
    }

    private void HandleConstructionFoundation(Vector3 constructionSite)
    {
        StartCoroutine(BuildRoutine(constructionSite));
    }

    private IEnumerator BuildRoutine(Vector3 constructionSite)
    {
        yield return _constructionWait;

        _foundation = _spawnerFoundation.CreateFoundation(constructionSite);
    }

    private IEnumerator BuildBaseRoutine(Vector3 constructionSite)
    {
        Destroy(_foundation);

        Base newBase = _spawnerBase.CreateNewBase(constructionSite);
        Canvas canvas = FindObjectOfType<Canvas>();
        Display display = SetDisplayParent(newBase, canvas);

        SwitchStateBase(newBase, display, isActivated: false);

        yield return _constructionWait;

        SwitchStateBase(newBase, display, isActivated: true);

        _bot.gameObject.SetActive(false);
    }

    private Display SetDisplayParent(Base newBase, Canvas canvas)
    {
        if (TryGetRoofComponent(newBase, out Display display))
        {
            Transform parent = canvas != null
                ? canvas.transform
                : transform;

            display.transform.SetParent(parent);
        }

        return display;
    }

    private bool TryGetRoofComponent(Base newBase, out Display display)
    {
        display = newBase.GetComponentInChildren<Display>();

        return display != null;
    }

    private void SwitchStateBase(Base newBase, Display display, bool isActivated)
    {
        newBase.gameObject.SetActive(isActivated);
        display.gameObject.SetActive(isActivated);
    }
}