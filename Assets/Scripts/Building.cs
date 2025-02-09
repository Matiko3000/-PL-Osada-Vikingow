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
        if (!isProducing && buildingData.productionRates.Count > 0)
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
                if (hit.transform.IsChildOf(transform))
                {
                    FindObjectOfType<BuildingUIManager>().ShowBuildingWindow(this);
                }
            }
        }
    }


    private IEnumerator ProduceResources()
    {
        isProducing = true;

        while (true) // Pêtla produkcji
        {
            foreach (var production in buildingData.productionRates)
            {
                yield return new WaitForSeconds(production.productionTime);
                ResourceManager.Instance.AddResource(production.resourceType, production.amountPerCycle);
            }
        }
    }

}
