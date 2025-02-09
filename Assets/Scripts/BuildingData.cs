using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ResourceManager;

[Serializable]
public class ResourceProduction
{
    public ResourceType resourceType; //type of produced material
    public int amountPerCycle;        //amount produced per cycle
    public float productionTime;      //time beetwen cycles
}

[Serializable]
public class ResourceCost
{
    public ResourceManager.ResourceType resourceType;
    public int amount;
}

[CreateAssetMenu(fileName = "New Building", menuName = "Buildings/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("PopUp Window")]
    public string buildingName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Pricing")]
    public List<ResourceCost> buildCosts;
    public float upgradeMultiplier;

    [Header("Instantiating")]
    public string buildButtonName;

    [Header("Producing")]
    public List<ResourceProduction> productionRates;


    //get current upgrade costs(using this bcs scriptable object saves data after closing game, which messes up the prices after launching 2nd time in editor)
    public List<ResourceCost> GetUpgradeCosts(int buildingLevel)
    {
        List<ResourceCost> tempCosts = new List<ResourceCost>();

        //multiply the values
        foreach (var cost in buildCosts)
        {
            var newCost = new ResourceCost
            {
                resourceType = cost.resourceType,
                amount = Mathf.RoundToInt(cost.amount * Mathf.Pow(upgradeMultiplier, buildingLevel)) //multiply depending on the level
            };
            tempCosts.Add(newCost);
        }

        return tempCosts;
    }
}
