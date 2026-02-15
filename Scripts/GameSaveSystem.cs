using MobileTycoon;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameSaveSystem : MonoBehaviour
{
    public List<AllData> _AllData;
    string filePath;
    BinaryFormatter formatter = new();
    FileStream fileStream;

    // Loading Panel
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Slider loadingSlider;
    float loadingSmoothness;
    float loadingSliderValue;

    bool isAppPaused;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/GameData.gd";     
        if (!PlayerPrefs.HasKey("IsFirstSetup"))
        {
            // First time opening
            CreateFile();
            PlayerPrefs.SetInt("IsFirstSetup", 1);
            PlayerPrefs.SetInt("GameVolume", 1);
            PlayerPrefs.SetInt("SfxVolume", 1);
        }
        else
        {
            // Reading values
            LoadSavedData();
            StartCoroutine(StartLoadingSlider());
        }
    }

    IEnumerator StartLoadingSlider()
    {
        loadingPanel.SetActive(true);
        while (true)
        {
            if (loadingSliderValue != 1)
            {
                float smoothValue = Mathf.SmoothDamp(loadingSlider.value, loadingSliderValue, ref loadingSmoothness, 50 * Time.deltaTime);
                loadingSlider.value = smoothValue;
                if (loadingSlider.value >= 1f)
                {
                    loadingPanel.SetActive(false);
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }

    [ContextMenu("SAVE DATA")]
    public void SaveAllData()
    {
        if (File.Exists(filePath))
        {
            fileStream = File.OpenWrite(filePath);
            formatter.Serialize(fileStream, _AllData);
            fileStream.Close();
        }
    }

    void CreateFile()
    {
        if (!File.Exists(filePath))
        {                                
            fileStream = File.Create(filePath);
            formatter.Serialize(fileStream, _AllData);
            fileStream.Close();

            LoadSavedData();
            StartCoroutine(StartLoadingSlider());
        }
    }

    // 3 field 3 machine 1 customer 1 general 100 / 8 ~ 0.126f
    async Task LoadAllFieldData()
    {
        await Task.Delay(1000);

        foreach (var item in GameManager.instance._FieldUpgrades)
        {
            item.LoadFieldData();         
            loadingSliderValue += 0.126f;
        }
        await Task.Delay(500);
    }

    async Task LoadAllMachineData()
    {
        foreach (var item in GameManager.instance._MachineUpgrade)
        {
            item.LoadMachineData();          
            loadingSliderValue += 0.126f;
        }
        await Task.Delay(500);
    }

    async Task LoadPlayerAndCustomersData()
    {
        GameManager.instance._OfficeManager.LoadOfficeManagerData();
        loadingSliderValue += 0.126f;
        await Task.Delay(500);
    }

    async Task LoadGeneralData()
    {
        GameManager.instance.LoadGeneralData();
        loadingSliderValue += 0.126f;
        await Task.Delay(500);
    }

    async void LoadSavedData()
    {
        if (File.Exists(filePath))
        {
            fileStream = File.Open(filePath, FileMode.Open);
            _AllData = (List<AllData>) formatter.Deserialize(fileStream);
            fileStream.Close();

            await LoadAllFieldData();
            await LoadAllMachineData();
            await LoadPlayerAndCustomersData();
            await LoadGeneralData();
        }
    }


    /* Save system testing is currently disabled
    private void OnApplicationFocus(bool focus)
    {
        isAppPaused = !focus;
        if (isAppPaused)
        {
            SaveAllData();
        }
    }

    // True ise arka plandan döner
    private void OnApplicationPause(bool pause)
    {
        isAppPaused = pause;
        if (!isAppPaused)
        {
            SceneManager.LoadScene(1); 
        }
    }
    */


    //  -------------------------------- Getter --------------------------------
    //Field
    public float GetFieldProductionTime(int FieldID)
    {
       return _AllData[0].Fields[FieldID].FieldsSavedData[0].FieldManagerDataList[0].FieldProductionTime;
    }

    public int GetFieldUpgradeLevel(int FieldID)
    {
        return _AllData[0].Fields[FieldID].FieldsSavedData[0].FieldManagerDataList[0].FieldUpgradeLevel;
    }

    public int GetFieldSlotLength(int FieldID)
    {
        return _AllData[0].Fields[FieldID].FieldsSavedData[0].SlotDataList.Count;
    }

    public int GetFieldSlotSetupCost(int FieldID, int SlotID)
    {
        return _AllData[0].Fields[FieldID].FieldsSavedData[0].SlotDataList[SlotID].SetupCost;
    }

    public bool IsFieldSlotOpen(int FieldID, int SlotID)
    {
        return _AllData[0].Fields[FieldID].FieldsSavedData[0].SlotDataList[SlotID].IsOpen;
    }

    // Machine
    public float GetMachineProductionSpeed(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CurrentProductionSpeed;
    }

    public int GetMachineProductionSpeedUpgradeLevel(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].ProductionSpeedUpgradeLevel;
    }

    public int GetMachineCurrentFruitCapacity(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CurrentFruitCapacity;
    }

    public int GetMachineCurrentFruitCount(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineDataList[0].CurrentFruitCount;
    }

    public int GetMachineCurrentFruitCapacityUpgradeLevel(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].FruitCapacityUpgradeLevel;
    }

    public int GetMachineCurrentCupCapacity(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CurrentCupCapacity;
    }

    public int GetMachineCurrentCupCount(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineDataList[0].CurrentCupCount;
    }

    public int GetMachineCurrentCupCapacityUpgradeLevel(int MachineID)
    {
        return _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CupCapacityUpgradeLevel;
    }

    // Office Manager - Player
    public int GetCurrentMovementSpeed()
    {
        return _AllData[0].Player.CurrentMoveSpeed;
    }

    public int GetCurrentMovementSpeedUpgradeLevel()
    {
        return _AllData[0].Player.MoveSpeedUpgradeLevel;
    }

    public int GetCurrentCarryCapacity()
    {
        return _AllData[0].Player.CurrentCarryCapacity;
    }

    public int GetCurrentCarryCapacityUpgradeLevel()
    {
        return _AllData[0].Player.CurrentCarryCapacityUpgradeLevel;
    }

    // Office Manager - Customer
    public int GetCurrentCustomerCapacity()
    {
        return _AllData[0].Customers[0].CurrentCustomerCount;
    }

    public int GetCurrentCustomerCapacityUpgradeLevel()
    {
        return _AllData[0].Customers[0].CustomerCountUpgradeLevel;
    }

    //  -------------------------------- Setter --------------------------------
    // Field
    public void SetFieldProductionTime(int FieldID, float NewProductionTime)
    {
        _AllData[0].Fields[FieldID].FieldsSavedData[0].FieldManagerDataList[0].FieldProductionTime = NewProductionTime;
    }

    public void SetFieldUpgradeLevel(int FieldID, int NewUpgradeLevel)
    {
        _AllData[0].Fields[FieldID].FieldsSavedData[0].FieldManagerDataList[0].FieldUpgradeLevel = NewUpgradeLevel;
    }

    public void SetFieldIsOpen(int FieldID, int SlotID, bool IsOpen)
    {
        _AllData[0].Fields[FieldID].FieldsSavedData[0].SlotDataList[SlotID].IsOpen = IsOpen;
    }

    // Machine
    public void SetMachineProductionSpeed(int MachineID, float NewSpeed)
    {
        _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CurrentProductionSpeed = NewSpeed;
    }

    public void SetMachineProductionSpeedUpgradeLevel(int MachineID, int NewUpgradeLevel)
    {
        _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].ProductionSpeedUpgradeLevel = NewUpgradeLevel;
    }

    public void SetMachineCurrentFruitCapacity(int MachineID, int NewFruitCapacity)
    {
        _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CurrentFruitCapacity = NewFruitCapacity;
    }

    public void SetMachineCurrentFruitCapacityUpgradeLevel(int MachineID, int NewUpgradeLevel)
    {
        _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].FruitCapacityUpgradeLevel = NewUpgradeLevel;
    }

    public void SetMachineCurrentCupCapacity(int MachineID, int NewCupCapacity)
    {
        _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CurrentCupCapacity = NewCupCapacity;
    }

    public void SetMachineCurrentCupCapacityUpgradeLevel(int MachineID, int NewUpgradeLevel)
    {
        _AllData[0].Machines[MachineID].MachinesSavedData[0].MachineManagerDataList[0].CupCapacityUpgradeLevel = NewUpgradeLevel;
    }

    // Office Manager - Player
    public void SetPlayerMovementSpeed(int NewSpeed)
    {
        _AllData[0].Player.CurrentMoveSpeed = NewSpeed;
    }

    public void SetPlayerMovementSpeedUpgradeLevel(int NewLevel)
    {
        _AllData[0].Player.MoveSpeedUpgradeLevel = NewLevel;
    }

    public void SetPlayerCarryCapacity(int NewCapacity)
    {
        _AllData[0].Player.CurrentCarryCapacity = NewCapacity;
    }

    public void SetPlayerCarryCapacityUpgradeLevel(int NewLevel)
    {
        _AllData[0].Player.CurrentCarryCapacityUpgradeLevel = NewLevel;
    }

    // Office Manager - Customer
    public void SetCustomerCapacity(int NewCapacity)
    {
        _AllData[0].Customers[0].CurrentCustomerCount = NewCapacity;
    }

    public void SetCustomerCapacityUpgradeLevel(int NewLevel)
    {
        _AllData[0].Customers[0].CustomerCountUpgradeLevel = NewLevel;
    }

}
