using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingUnlockData
{
    public GameObject buildButton;
    public int unlockLevel;
}

public class MeadHall : MonoBehaviour
{
    Building building;
    ResourceManager resourceManager;

    [SerializeField] public int maxBuildingLvls;//this scales with the max lvl of other buildings u can have with the current mead hall lvl
    //for example: if this value is set to 3, then maximum level of all other buildings can be 3 on mead hall level 1, then 6 on mead hall level 2 etc.


    [SerializeField] private List<BuildingUnlockData> unlockableBuildings;


    void Start()
    {
        building = GetComponent<Building>();
        resourceManager = FindObjectOfType<ResourceManager>();

        foreach (var data in unlockableBuildings)
        {
            data.buildButton.gameObject.SetActive(false);
        }
    }

    public void OnLevelChange()
    {
        //update max population
        resourceManager.maxPopulation = Mathf.RoundToInt(resourceManager.maxPopulation * building.buildingData.upgradeMultiplier);

        foreach (var data in unlockableBuildings)//check if there is any building to unlock
        {
            if(building.BuildingLevel == data.unlockLevel) data.buildButton.gameObject.SetActive(true);
        }
    }

    public int GetMaxLvl()//get max lvl other buildings can have
    {
        return building.BuildingLevel * maxBuildingLvls;
    }
}
