using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action OnTap;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnTap?.Invoke();
        }
    }
}