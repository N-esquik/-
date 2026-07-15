using System;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    private int _amountRock;

    public event Action<int> OnResourceCountChanged;

    public void AddResource(int amount)
    {
        _amountRock += amount;
        OnResourceCountChanged?.Invoke(_amountRock);
    }
}
