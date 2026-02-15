using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineUpgrade : MonoBehaviour
{
    public int MachineID;
    [SerializeField] GameObject _upgradeWindow;
    
    [Header("----- Production Speed -----")]
    [SerializeField] float currentProductionSpeed;
    int productionSpeedUpgradeCost;
    int ProductionSpeedUpgradeLevel = 1;
    
    [SerializeField] TextMeshProUGUI ProductionSpeedText;
    [SerializeField] TextMeshProUGUI nextProductionSpeedText;
    [SerializeField] TextMeshProUGUI productionSpeedUpgradeCostText;
    [SerializeField] Button productionSpeedUpgradeButton;

    [Header("----- Fruit Capacity -----")]
    [SerializeField] int currentFruitCapacity; 
    int fruitCapacityUpgradeCost;
    int FruitCapacityUpgradeLevel = 1;

    [SerializeField] TextMeshProUGUI FruitCapacityText;
    [SerializeField] TextMeshProUGUI nextFruitCapacityText;
    [SerializeField] TextMeshProUGUI fruitCapacityUpgradeCostText;
    [SerializeField] Button fruitCapacityUpgradeButton;

    [Header("----- Cup Capacity -----")]
    [SerializeField] int currentCupCapacity; 
    int cupCapacityUpgradeCost;
    int CupCapacityUpgradeLevel = 1;

    [SerializeField] TextMeshProUGUI cupCapacityText;
    [SerializeField] TextMeshProUGUI nextcupCapacityText;
    [SerializeField] TextMeshProUGUI cupCapacityUpgradeCostText;
    [SerializeField] Button cupCapacityUpgradeButton;

    public void LoadMachineData()
    {
        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
        
        currentProductionSpeed = saveSystem.GetMachineProductionSpeed(MachineID);
        ProductionSpeedUpgradeLevel = saveSystem.GetMachineProductionSpeedUpgradeLevel(MachineID);
       
        currentFruitCapacity = saveSystem.GetMachineCurrentFruitCapacity(MachineID);
        FruitCapacityUpgradeLevel = saveSystem.GetMachineCurrentFruitCapacityUpgradeLevel(MachineID);
        
        currentCupCapacity = saveSystem.GetMachineCurrentCupCapacity(MachineID);
        CupCapacityUpgradeLevel = saveSystem.GetMachineCurrentCupCapacityUpgradeLevel(MachineID);

        GameManager.instance.Machines[MachineID].ProductionDuration = currentProductionSpeed;
        GameManager.instance.Machines[MachineID].totalFruitCapacity = currentFruitCapacity;
        GameManager.instance.Machines[MachineID].maxCupCapacity = currentCupCapacity;

        // Fruit & Cup
        GameManager.instance.Machines[MachineID].CurrentFruitCount = saveSystem.GetMachineCurrentFruitCount(MachineID);
        GameManager.instance.Machines[MachineID].CurrentCupCount = saveSystem.GetMachineCurrentCupCount(MachineID);

        GameManager.instance.Machines[MachineID].CheckProductionAndCups();

        GameManager.instance.Machines[MachineID].UpdateFruipFillBar();
        GameManager.instance.Machines[MachineID].UpdateCupFillBar();
        GameManager.instance.Machines[MachineID].StartMachine();

    }

    public void ShowUpgradeWindow(bool show)
    {
        _upgradeWindow.SetActive(show);

        if (show)
        {
            UpdatePanel();
        }
        else
        {
            GameManager.instance.PlaySound(0);
        }
    }

    void UpdatePanel()
    {
        // ProductionSpeed     
        if (ProductionSpeedUpgradeLevel == 26)
        {
            productionSpeedUpgradeButton.interactable = false;
            productionSpeedUpgradeCostText.text = "MAX";
            nextProductionSpeedText.text = "MAX";
            ProductionSpeedText.text = currentProductionSpeed.ToString();
        }
        else
        {
            productionSpeedUpgradeCost = 2310 * ProductionSpeedUpgradeLevel;
            productionSpeedUpgradeCostText.text = productionSpeedUpgradeCost.ToString();

            ProductionSpeedText.text = currentProductionSpeed.ToString();
            nextProductionSpeedText.text = (currentProductionSpeed - 1).ToString();

            if (productionSpeedUpgradeCost <= GameManager.instance.Money)
            {
                productionSpeedUpgradeButton.interactable = true;
            }
            else
            {
                productionSpeedUpgradeButton.interactable = false;
            }
        }


        // Fruit Capacity
        if (FruitCapacityUpgradeLevel == 41)
        {
            fruitCapacityUpgradeButton.interactable = false;
            fruitCapacityUpgradeCostText.text = "MAX";
            nextFruitCapacityText.text = "MAX";
            FruitCapacityText.text = currentFruitCapacity.ToString();          
        }
        else
        {
            fruitCapacityUpgradeCost = 1980 * FruitCapacityUpgradeLevel;
            fruitCapacityUpgradeCostText.text = fruitCapacityUpgradeCost.ToString();

            FruitCapacityText.text = currentFruitCapacity.ToString();
            nextFruitCapacityText.text = (currentFruitCapacity + 1).ToString();      
            GameManager.instance.Machines[MachineID].maxFruitCapacityText.text = (currentFruitCapacity + 1).ToString();

            if (fruitCapacityUpgradeCost <= GameManager.instance.Money)
            {
                fruitCapacityUpgradeButton.interactable = true;
            }
            else
            {
                fruitCapacityUpgradeButton.interactable = false;
            }
        }


        // Cup Capacity
        if (CupCapacityUpgradeLevel == 51)
        {
            cupCapacityUpgradeButton.interactable = false;
            cupCapacityUpgradeCostText.text = "MAX";
            nextcupCapacityText.text = "MAX";
            cupCapacityText.text = currentCupCapacity.ToString();          
        }
        else
        {
            cupCapacityUpgradeCost = 1350 * CupCapacityUpgradeLevel;
            cupCapacityUpgradeCostText.text = cupCapacityUpgradeCost.ToString();

            cupCapacityText.text = currentCupCapacity.ToString();
            nextcupCapacityText.text = (currentCupCapacity + 1).ToString();
            GameManager.instance.Machines[MachineID].maxCupCapacityText.text = (currentCupCapacity + 1).ToString();

            if (cupCapacityUpgradeCost <= GameManager.instance.Money)
            {
                cupCapacityUpgradeButton.interactable = true;
            }
            else
            {
                cupCapacityUpgradeButton.interactable = false;
            }
        }
    }

    public void OnUpgradeProductionSpeed_ButtonPressed()
    {      
        currentProductionSpeed--;
        ProductionSpeedUpgradeLevel++;
        GameManager.instance.UpdateMoney(-productionSpeedUpgradeCost);
        GameManager.instance.Machines[MachineID].ProductionDuration = currentProductionSpeed;

        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
        saveSystem.SetMachineProductionSpeed(MachineID, currentProductionSpeed);
        saveSystem.SetMachineProductionSpeedUpgradeLevel(MachineID, ProductionSpeedUpgradeLevel);

        GameManager.instance.PlaySound(4);
        UpdatePanel();
    }

    public void OnUpgradeFruitCapacity_ButtonPressed()
    {
        currentFruitCapacity++;
        FruitCapacityUpgradeLevel++;
        GameManager.instance.UpdateMoney(-fruitCapacityUpgradeCost);
        GameManager.instance.Machines[MachineID].totalFruitCapacity = currentFruitCapacity;

        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
        saveSystem.SetMachineCurrentFruitCapacity(MachineID, currentFruitCapacity);
        saveSystem.SetMachineCurrentFruitCapacityUpgradeLevel(MachineID, FruitCapacityUpgradeLevel);

        GameManager.instance.PlaySound(4);
        UpdatePanel();
    }

    public void OnUpgradeCupCapacity_ButtonPressed()
    {
        currentCupCapacity++;
        CupCapacityUpgradeLevel++;
        GameManager.instance.UpdateMoney(-cupCapacityUpgradeCost);
        GameManager.instance.Machines[MachineID].maxCupCapacity = currentCupCapacity;
        GameManager.instance.Machines[MachineID].CheckProduction();

        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
        saveSystem.SetMachineCurrentCupCapacity(MachineID, currentCupCapacity);
        saveSystem.SetMachineCurrentCupCapacityUpgradeLevel(MachineID, CupCapacityUpgradeLevel);

        GameManager.instance.PlaySound(4);
        UpdatePanel();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (!_upgradeWindow.activeInHierarchy)
        {
            ShowUpgradeWindow(true);
        }

    }


}
