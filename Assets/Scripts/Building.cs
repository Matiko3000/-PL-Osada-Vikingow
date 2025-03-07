using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public BuildingData buildingData;
    private bool isProducing = false;
    public int BuildingLevel = 0;

    void Start()
    {
        StartProduction();
    }

    private void StartProduction()
    {
        if (!isProducing && buildingData.productionRates != null)
        {
            StartCoroutine(ProduceResources());
        }
    }

    //Handle clicking on the building
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.IsChildOf(transform) && FindObjectOfType<UIManager>().areBuildingsClickable)//check if all u can click the building
                {
                    FindObjectOfType<BuildingUIManager>().ShowBuildingWindow(this);
                }
            }
        }
    }


    private IEnumerator ProduceResources()
    {
        isProducing = true;

        while (true) //production loop
        {
            yield return new WaitForSeconds(buildingData.productionRates.productionTime);

            int amountToProduce = buildingData.GetProductionAmount(buildingData.productionRates, BuildingLevel);//calculate production based on level
            ResourceManager.Instance.AddResource(buildingData.productionRates.resourceType, amountToProduce);
        }
    }
}
