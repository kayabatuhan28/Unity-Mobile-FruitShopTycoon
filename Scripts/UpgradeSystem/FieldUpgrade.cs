using MobileTycoon;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldUpgrade : MonoBehaviour
{
    public int FieldID;
    public float ProductionDuration;
    int requiredCost;
    public int UpgradeLevel = 1;
    public List<Tree> TreeList;
    public List<BuildArea> Slots;

    [SerializeField] GameObject _upgradeWindow;
    [SerializeField] TextMeshProUGUI _currentDurationText;
    [SerializeField] TextMeshProUGUI _nextDurationText;
    [SerializeField] TextMeshProUGUI _upgradeCostText;
    [SerializeField] Button _upgradeButton;

   
    public void LoadFieldData()
    {
        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
        
        ProductionDuration = saveSystem.GetFieldProductionTime(FieldID);
        UpgradeLevel = saveSystem.GetFieldUpgradeLevel(FieldID);

        if (UpgradeLevel == 6)
        {
            gameObject.SetActive(false);
        }

        // Retrieving and setting slot data in the data system
        int fieldLength = saveSystem.GetFieldSlotLength(FieldID);
        for (int i = 0; i < fieldLength; i++)
        {
            int UpdateCost = saveSystem.GetFieldSlotSetupCost(FieldID, i);
            bool IsOpen = saveSystem.IsFieldSlotOpen(FieldID, i);
            Slots[i].UpdateCostText(UpdateCost, IsOpen);          

            if (saveSystem.IsFieldSlotOpen(FieldID, i))
            {
                Slots[i].setupObject.SetActive(false);
                TreeList[i].growthDuration = ProductionDuration;
                TreeList[i].gameObject.SetActive(true);
            }
        }
    }

    public void ShowUpgradeWindow(bool show)
    {
        _upgradeWindow.SetActive(show);

        if (show)
        {
            requiredCost = 2150 * UpgradeLevel;
            _upgradeCostText.text = requiredCost.ToString();

            _currentDurationText.text = ProductionDuration.ToString();
            _nextDurationText.text = (ProductionDuration - 1).ToString();

            if (requiredCost <= GameManager.instance.Money)
            {
                _upgradeButton.interactable = true;
            }
            else
            {
                _upgradeButton.interactable = false;
            }
        }
        else
        {
            GameManager.instance.PlaySound(0);
        }
    }

    public void UpgradeField()
    {
        ProductionDuration--;
        UpgradeLevel++;
        GameManager.instance.UpdateMoney(-requiredCost);

        foreach(var item in TreeList)
        {
            item.growthDuration = ProductionDuration;
        }

        ShowUpgradeWindow(false);

        if (UpgradeLevel == 6)
        {
            gameObject.SetActive(false);
        }

        GameManager.instance.PlaySound(4);

        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
        saveSystem.SetFieldProductionTime(FieldID, ProductionDuration);
        saveSystem.SetFieldUpgradeLevel(FieldID, UpgradeLevel);
        
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
