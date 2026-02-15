using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfficeManager : MonoBehaviour
{
    [SerializeField] GameObject upgradeWindow;

    [Header("----- Movement Speed -----")]
    public int currentMovementSpeed;
    int movementSpeedUpgradeCost;
    int movementSpeedUpgradeLevel = 1;

    [SerializeField] TextMeshProUGUI currentMovementSpeedText;
    [SerializeField] TextMeshProUGUI nextMovementSpeedText;
    [SerializeField] TextMeshProUGUI movementSpeedUpgradeCostText;
    [SerializeField] Button movementSpeedUpgradeButton;

    [Header("----- Player Carry Capacity -----")]
    public int currentCarryCapacity;
    int carryCapacityUpgradeCost;
    int carryCapacityUpgradeLevel = 1;

    [SerializeField] TextMeshProUGUI currentCarryCapacityText;
    [SerializeField] TextMeshProUGUI nextCarryCapacityText;
    [SerializeField] TextMeshProUGUI carryCapacityUpgradeCostText;
    [SerializeField] Button carryCapacityUpgradeButton;

    [Header("----- Customer Capacity -----")]
    [SerializeField] int currentCustomerCapacity;
    int customerCapacityUpgradeCost;
    int customerCapacityUpgradeLevel = 1;

    [SerializeField] TextMeshProUGUI currentCustomerCapacityText;
    [SerializeField] TextMeshProUGUI nextCustomerCapacityText;
    [SerializeField] TextMeshProUGUI customerCapacityUpgradeCostText;
    [SerializeField] Button customerCapacityUpgradeButton;

   
    public void LoadOfficeManagerData()
    {
        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;
        currentMovementSpeed = saveSystem.GetCurrentMovementSpeed();
        movementSpeedUpgradeLevel = saveSystem.GetCurrentMovementSpeedUpgradeLevel();
        currentCarryCapacity = saveSystem.GetCurrentCarryCapacity();
        carryCapacityUpgradeLevel = saveSystem.GetCurrentCarryCapacityUpgradeLevel();

        currentCustomerCapacity = saveSystem.GetCurrentCustomerCapacity();
        customerCapacityUpgradeLevel = saveSystem.GetCurrentCustomerCapacityUpgradeLevel();

        GameManager.instance._PlayerController.moveSpeed = currentMovementSpeed;
        GameManager.instance._PlayerController.MaxCarryCount = currentCarryCapacity;

        // Customer
        GameManager.instance._CustomerManager.MaxCustomerLimit = currentCustomerCapacity;
    }

    public void ShowUpgradeWindow(bool show)
    {
        upgradeWindow.SetActive(show);

        if (show)
        {
            UpdatePanel();
        }
        else
        {
            GameManager.instance.PlaySound(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (!upgradeWindow.activeInHierarchy)
        {
            ShowUpgradeWindow(true);         
        }
    }

    void UpdatePanel()
    {
        // Movement Speed     
        if (movementSpeedUpgradeLevel == 6)
        {
            movementSpeedUpgradeButton.interactable = false;
            movementSpeedUpgradeCostText.text = "MAX";
            nextMovementSpeedText.text = "MAX";
            currentMovementSpeedText.text = currentMovementSpeed.ToString();
        }
        else
        {
            movementSpeedUpgradeCost = 3150 * movementSpeedUpgradeLevel;
            movementSpeedUpgradeCostText.text = movementSpeedUpgradeCost.ToString();

            currentMovementSpeedText.text = currentMovementSpeed.ToString();
            nextMovementSpeedText.text = (currentMovementSpeed + 1).ToString();

            if (movementSpeedUpgradeCost <= GameManager.instance.Money)
            {
                movementSpeedUpgradeButton.interactable = true;
            }
            else
            {
                movementSpeedUpgradeButton.interactable = false;
            }
        }


        // Player Carry Capacity
        if (carryCapacityUpgradeLevel == 26)
        {
            carryCapacityUpgradeButton.interactable = false;
            carryCapacityUpgradeCostText.text = "MAX";
            nextCarryCapacityText.text = "MAX";
            currentCarryCapacityText.text = currentCarryCapacity.ToString();
        }
        else
        {
            carryCapacityUpgradeCost = 4050 * carryCapacityUpgradeLevel;
            carryCapacityUpgradeCostText.text = carryCapacityUpgradeCost.ToString();

            currentCarryCapacityText.text = currentCarryCapacity.ToString();
            nextCarryCapacityText.text = (currentCarryCapacity + 1).ToString();

            if (carryCapacityUpgradeCost <= GameManager.instance.Money)
            {
                carryCapacityUpgradeButton.interactable = true;
            }
            else
            {
                carryCapacityUpgradeButton.interactable = false;
            }
        }


        // Customer Capacity
        if (customerCapacityUpgradeLevel == 11)
        {
            customerCapacityUpgradeButton.interactable = false;
            customerCapacityUpgradeCostText.text = "MAX";
            nextCustomerCapacityText.text = "MAX";
            currentCustomerCapacityText.text = currentCustomerCapacity.ToString();
        }
        else
        {
            customerCapacityUpgradeCost = 6350 * customerCapacityUpgradeLevel;
            customerCapacityUpgradeCostText.text = customerCapacityUpgradeCost.ToString();

            currentCustomerCapacityText.text = currentCustomerCapacity.ToString();
            nextCustomerCapacityText.text = (currentCustomerCapacity + 1).ToString();

            if (customerCapacityUpgradeCost <= GameManager.instance.Money)
            {
                customerCapacityUpgradeButton.interactable = true;
            }
            else
            {
                customerCapacityUpgradeButton.interactable = false;
            }
        }
    }

    public void UpgradeMovementSpeed()
    {
        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;

        currentMovementSpeed++;
        movementSpeedUpgradeLevel++;
        GameManager.instance.UpdateMoney(-movementSpeedUpgradeCost);
        GameManager.instance._PlayerController.moveSpeed = currentMovementSpeed;

        saveSystem.SetPlayerMovementSpeed(currentMovementSpeed);
        saveSystem.SetPlayerMovementSpeedUpgradeLevel(movementSpeedUpgradeLevel);

        GameManager.instance.PlaySound(4);
        UpdatePanel();
    }

    public void UpgradeCarryCapacity()
    {
        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;

        currentCarryCapacity++;
        carryCapacityUpgradeLevel++;
        GameManager.instance.UpdateMoney(-carryCapacityUpgradeCost);
        GameManager.instance._PlayerController.MaxCarryCount = currentCarryCapacity;

        saveSystem.SetPlayerCarryCapacity(currentCarryCapacity);
        saveSystem.SetPlayerCarryCapacityUpgradeLevel(carryCapacityUpgradeLevel);

        if (GameManager.instance._PlayerController.MaxSprite.gameObject.activeInHierarchy)
        {
            GameManager.instance._PlayerController.MaxSprite.gameObject.SetActive(false);
        }

        GameManager.instance.PlaySound(4);
        UpdatePanel();
    }

    public void UpgradeCustomerCapacity()
    {
        GameSaveSystem saveSystem = GameManager.instance._GameSaveSystem;

        currentCustomerCapacity++;
        customerCapacityUpgradeLevel++;
        GameManager.instance.UpdateMoney(-customerCapacityUpgradeCost);
        GameManager.instance._CustomerManager.MaxCustomerLimit = currentCustomerCapacity;

        saveSystem.SetCustomerCapacity(currentCustomerCapacity);
        saveSystem.SetCustomerCapacityUpgradeLevel(customerCapacityUpgradeLevel);

        GameManager.instance.PlaySound(4);
        UpdatePanel();
    }


}
