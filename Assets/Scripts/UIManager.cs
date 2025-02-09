using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buildingMatsText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI foodText;
    [SerializeField] TextMeshProUGUI populationText;

    private void Start()
    {
        //Sub to event
        ResourceManager.Instance.OnResourceChanged += UpdateResourceDisplay;

        //refresh the UI
        foreach (ResourceManager.ResourceType type in System.Enum.GetValues(typeof(ResourceManager.ResourceType)))
        {
            ResourceManager.Instance.AddResource(type, 0);
        }
    }


    //Gets called when amount of some resources changes
    private void UpdateResourceDisplay(ResourceManager.ResourceType type, int newAmount)
    {
        switch (type)
        {
            case ResourceManager.ResourceType.BuildingMats:
                buildingMatsText.text = newAmount.ToString();
                break;
            case ResourceManager.ResourceType.Gold:
                goldText.text = newAmount.ToString();
                break;
            case ResourceManager.ResourceType.Food:
                foodText.text = newAmount.ToString();
                break;
            case ResourceManager.ResourceType.Population:
                populationText.text = newAmount.ToString();
                break;
        }
    }
}
