using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarManager : MonoBehaviour
{
    private int WarTrophies = 0;

    [SerializeField] GameObject warWindow;
    [SerializeField] GameObject warResultWindow;
    [SerializeField] GameObject NotEnoughPopWindow;

    UIManager uiManager;
    ResourceManager resourceManager;
    MeadHall meadHall;
    [SerializeField] Building offensiveBuilding; 
    [SerializeField] Building defensiveBuilding;
    [SerializeField] TextMeshProUGUI warTrophiesText;

    void Start()
    {
        warWindow.SetActive(false);
        warResultWindow.SetActive(false);
        NotEnoughPopWindow.SetActive(false);

        warTrophiesText.text = WarTrophies.ToString();

        uiManager = FindObjectOfType<UIManager>();
        resourceManager = FindObjectOfType<ResourceManager>();
        meadHall = FindObjectOfType<MeadHall>();
    }

    public void ShowWarWindow()
    {
        warWindow.SetActive(true);
        uiManager.areBuildingsClickable = false;

        for (int i = 1; i <= 3; i++)
        {
            //Find correct panel
            GameObject warTypePanel = GameObject.Find($"WarType{i}");
            if (warTypePanel == null) continue;

            //Find UI elements
            TextMeshProUGUI winRatioText = warTypePanel.transform.Find("WarTypePanel/WinRatioText").GetComponent<TextMeshProUGUI>();
            GameObject rewardsPanel = warTypePanel.transform.Find("RewardsPanel").gameObject;
            GameObject lossesPanel = warTypePanel.transform.Find("LossesPanel").gameObject;

            //Calculate values
            float winChance = (int)(CalculateWinChance(i) * 100);
            (int goldReward, int popChangeReward, int trophiesReward) = CalculateRewards(i, true);
            (int goldLoss, int popChangeLoss, int trophiesLoss) = CalculateRewards(i, false);

            //show values
            winRatioText.text = winChance + "%";
            UpdateBonusPanel(rewardsPanel, goldReward, popChangeReward, trophiesReward);
            UpdateBonusPanel(lossesPanel, goldLoss, popChangeLoss, trophiesLoss);
        }
    }


    public void StartWar(int riskLevel) // 1 - low risk, 2 - medium, 3 - high
    {
        float winChance = CalculateWinChance(riskLevel);
        bool isWin = Random.value <= winChance; //win/lose

        int goldChange, populationChange, trophyChange;
        (goldChange, populationChange, trophyChange) = CalculateRewards(riskLevel, isWin);

        //dont start war if not enough population
        if (GetPossiblePopulationLoss(riskLevel) > resourceManager.GetResourceAmount(ResourceManager.ResourceType.Population))
        {
            NotEnoughPopWindow.SetActive(true);
            return;
        }

        resourceManager.AddResource(ResourceManager.ResourceType.Gold, goldChange);
        resourceManager.AddResource(ResourceManager.ResourceType.Population, populationChange);
        WarTrophies = Mathf.Max(0, WarTrophies + trophyChange);
        warTrophiesText.text = WarTrophies.ToString();//update UI


        //show war results
        warResultWindow.SetActive(true);
        warResultWindow.transform.Find("WarResultTxt").GetComponent<TextMeshProUGUI>().text = (isWin ? "Wygrana!" : "Przegrana!"); //result
        UpdateBonusPanel(warResultWindow.transform.Find("RewardsPanel").gameObject, goldChange, populationChange, trophyChange);

        Debug.Log($"Wynik wojny: {(isWin ? "Wygrana!" : "Przegrana!")} | Zmiana z³ota: {goldChange} | Zmiana populacji: {populationChange} | Trofea: {trophyChange}");
    }

    private float CalculateWinChance(int riskLevel)
    {
        float baseChance;
        switch (riskLevel)
        {
            case 1: baseChance = 0.85f; break;
            case 2: baseChance = 0.65f; break;
            case 3: baseChance = 0.40f; break;
            default: baseChance = 0.5f; break;
        }
        
        float attackBonus = offensiveBuilding.BuildingLevel * 0.05f;
        float defenseBonus = defensiveBuilding.BuildingLevel * 0.02f; // Mniejszy wp³yw na szansê

        return Mathf.Clamp(baseChance + attackBonus + defenseBonus, 0.05f, 0.99f);
    }

    private (int gold, int population, int trophies) CalculateRewards(int riskLevel, bool isWin)
    {
        int baseGold = 10 * riskLevel;
        int basePopLoss = -3 * riskLevel;
        int baseTrophies = 5 * riskLevel;

        float rewardMultiplier = 1f + (defensiveBuilding.BuildingLevel * 0.25f);

        if (isWin)
        {
            return ((int)(baseGold * rewardMultiplier), basePopLoss / 2, baseTrophies);
        }
        else
        {
            return ((int)(-baseGold / 2 * rewardMultiplier), basePopLoss * 2, -baseTrophies);
        }
    }

    private int GetPossiblePopulationLoss(int riskLevel)//to check if user can even start the war
    {
        return 6 * riskLevel;
    }

    private void UpdateBonusPanel(GameObject panel, int gold, int pop, int trophies)//to correctly show values in warWindow
    {
        TextMeshProUGUI goldText = panel.transform.Find("BonusText1").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI popText = panel.transform.Find("BonusText2").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI trophiesText = panel.transform.Find("BonusText3").GetComponent<TextMeshProUGUI>();

        goldText.text = $"{(gold >= 0 ? "+" : "")}{gold}"; //if value is negative, it will type it with the "-" automatically, if not add "+"
        popText.text = $"{(pop >= 0 ? "+" : "")}{pop}";
        trophiesText.text = $"{(trophies >= 0 ? "+" : "")}{trophies}";
    }

    public void CloseWarWindow()
    {
        warWindow.SetActive(false);
        uiManager.areBuildingsClickable = true;
    }

    public void CloseWarResultWindow()
    {
        warResultWindow.SetActive(false);
    }

    public void CloseNotEnoughPopWindow()
    {
        NotEnoughPopWindow.SetActive(false);
    }
}
