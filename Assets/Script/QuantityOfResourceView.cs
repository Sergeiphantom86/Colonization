using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class QuantityOfResourceView : MonoBehaviour
{
    [SerializeField] private Warehouse _warehouse;

    private TextMeshProUGUI _infoQuantitiesResource;

    private void Awake()
    {
        _infoQuantitiesResource = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _warehouse.QuantityHasChanged += ShowTextQuantityResource;
    }

    private void OnDisable()
    {
        _warehouse.QuantityHasChanged -= ShowTextQuantityResource;
    }

    private void ShowTextQuantityResource(int quantity)
    {
        _infoQuantitiesResource.text = $"Ресурсы бызы\n\n'{quantity}'";
    }
}