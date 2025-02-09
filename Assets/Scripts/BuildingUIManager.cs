using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Resources;
using System.Collections.Generic;

public class BuildingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject upgradeWindowPrefab;
    [SerializeField] private GameObject NotEnoughMaterialsWindowPrefab;
    [SerializeField] private Transform uiCanvas; //parent

    [Header("Icons")]
    [SerializeField] Sprite buildingMatsIcon;
    [SerializeField] Sprite goldIcon;
    [SerializeField] Sprite foodIcon;
    [SerializeField] Sprite populationIcon;

    private GameObject currentWindow;

    public void ShowBuildingWindow(Building building)
    {
        //delete previous windows if exists
        if (currentWindow != null) Destroy(currentWindow);

        //create new one
        currentWindow = Instantiate(upgradeWindowPrefab, uiCanvas);

        //set UI
        currentWindow.transform.Find("Text_Title").GetComponent<TextMeshProUGUI>().text = building.buildingData.buildingName;
        currentWindow.transform.Find("Text_Info").GetComponent<TextMeshProUGUI>().text = building.buildingData.description;
        currentWindow.transform.Find("Text_Level").GetComponent<TextMeshProUGUI>().text = ("Aktualny poziom: " + building.BuildingLevel.ToString());

        //set prices using GetUpgradeCosts method
        Transform pricePanel = currentWindow.transform.Find("PricePanel");

        //hide price icon in case we dont use them
        pricePanel.Find("PriceText1").gameObject.SetActive(false);
        pricePanel.Find("PriceIcon1").gameObject.SetActive(false);
        pricePanel.Find("PriceText2").gameObject.SetActive(false);
        pricePanel.Find("PriceIcon2").gameObject.SetActive(false);

        List<ResourceCost> upgradeCosts = building.buildingData.GetUpgradeCosts(building.BuildingLevel);  // Get upgrade costs

        for (int i = 0; i < upgradeCosts.Count; i++)
        {
            if (i == 0)
            {
                pricePanel.Find("PriceText1").gameObject.SetActive(true);
                pricePanel.Find("PriceIcon1").gameObject.SetActive(true);
                pricePanel.Find("PriceText1").GetComponent<TextMeshProUGUI>().text = upgradeCosts[i].amount.ToString();
                pricePanel.Find("PriceIcon1").GetComponent<Image>().sprite = GetResourceIcon(upgradeCosts[i].resourceType);
            }
            else if (i == 1) //second cost(if exists)
            {
                pricePanel.Find("PriceText2").gameObject.SetActive(true);
                pricePanel.Find("PriceIcon2").gameObject.SetActive(true);
                pricePanel.Find("PriceText2").GetComponent<TextMeshProUGUI>().text = upgradeCosts[i].amount.ToString();
                pricePanel.Find("PriceIcon2").GetComponent<Image>().sprite = GetResourceIcon(upgradeCosts[i].resourceType);
            }
        }

        //set button actions
        currentWindow.transform.Find("BuildBtn").GetComponent<Button>().onClick.AddListener(() => BuildBuilding(building));
        currentWindow.transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(CloseWindow);
    }

    private void BuildBuilding(Building building)
    {
        List<ResourceCost> upgradeCosts = building.buildingData.GetUpgradeCosts(building.BuildingLevel);  // Get upgrade costs

        foreach (var cost in upgradeCosts)
        {
            if (ResourceManager.Instance.GetResourceAmount(cost.resourceType) < cost.amount)
            {
                ShowNotEnoughMaterialsWindow();
                return; // don't build if not enough resources
            }
        }

        foreach (var cost in upgradeCosts)  // Spend resources based on upgradeCosts
        {
            ResourceManager.Instance.SpendResource(cost.resourceType, cost.amount);
        }

        //show building in game
        if (building.BuildingLevel == 0) building.gameObject.SetActive(true);
        building.BuildingLevel += 1; //increase the level

        //destroy the button since from now on we will be using buildings to click
        GameObject Button = GameObject.Find(building.buildingData.buildButtonName);
        if (Button != null) Destroy(Button);

        CloseWindow();
    }

    private void CloseWindow()
    {
        if (currentWindow != null) Destroy(currentWindow);
    }

    //gives the right icon
    private Sprite GetResourceIcon(ResourceManager.ResourceType type)
    {
        switch (type)
        {
            case ResourceManager.ResourceType.BuildingMats:
                return buildingMatsIcon;
            case ResourceManager.ResourceType.Gold:
                return goldIcon;
            case ResourceManager.ResourceType.Food:
                return foodIcon;
            case ResourceManager.ResourceType.Population:
                return populationIcon;
        }
        return null;
    }

    private void ShowNotEnoughMaterialsWindow()
    {
        //delete previous windows if exists
        if (currentWindow != null) Destroy(currentWindow);

        //create new one
        currentWindow = Instantiate(NotEnoughMaterialsWindowPrefab, uiCanvas);
        currentWindow.transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(CloseWindow);
        currentWindow.transform.Find("CloseBtn2").GetComponent<Button>().onClick.AddListener(CloseWindow);
    }
}
