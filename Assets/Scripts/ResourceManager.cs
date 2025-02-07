using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public enum ResourceType { BuildingMats, Gold, Food, Population }
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    public event Action<ResourceType, int> OnResourceChanged;

    private void Awake()
    {
        //make sure there is only 1 resource manager in scene
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //initialize
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }
    }

    public int GetResourceAmount(ResourceType type)
    {
        return resources[type];
    }

    public void AddResource(ResourceType type, int amount)
    {
        resources[type] += amount;
        OnResourceChanged?.Invoke(type, resources[type]);
    }

    //returns false if not enough resources
    public bool SpendResource(ResourceType type, int amount)
    {
        if (resources[type] >= amount)
        {
            resources[type] -= amount;
            OnResourceChanged?.Invoke(type, resources[type]);
            return true;
        }
        return false;
    }
}
