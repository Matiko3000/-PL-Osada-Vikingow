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
    public bool isMeadHall = false;

    [Header("PopUp Window")]
    public string buildingName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Pricing")]
    public List<ResourceCost> buildCosts;
    public float upgradeCostMultiplier;

    [Header("Instantiating")]
    public string buildButtonName;

    [Header("Producing")]
    public ResourceProduction productionRates;
    public float upgradeMultiplier;

    [Header("Education")]
    [TextArea] public string eduText;



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
                amount = Mathf.RoundToInt(cost.amount * Mathf.Pow(upgradeCostMultiplier, buildingLevel)) //multiply depending on the level
            };
            tempCosts.Add(newCost);
        }

        return tempCosts;
    }

    public int GetProductionAmount(ResourceProduction baseProduction, int buildingLevel)
    {
        return Mathf.RoundToInt(baseProduction.amountPerCycle * Mathf.Pow(upgradeMultiplier, buildingLevel - 1));//use -1 bcs for example on lvl 1 u just wanna get it times 1 not more
    }
}
