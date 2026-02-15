using MobileTycoon;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int Money;
    [SerializeField] TextMeshProUGUI moneyText;


    GameObject AvailableFruit;
    [Header("-------- Fruit Pool --------")]
    public GameObject ApplePoolMainObject; // Allows the fruit to return to the pool after being taken and used
    public GameObject[] ApplePool;
    [HideInInspector] public int ApplePoolIndex;

    public GameObject LemonPoolMainObject; 
    public GameObject[] LemonPool;
    [HideInInspector] public int LemonPoolIndex;

    public GameObject OrangePoolMainObject; 
    public GameObject[] OrangePool;
    [HideInInspector] public int OrangePoolIndex;


    GameObject AvailableFruitJuice;
    [Header("-------- Fruit Juice Pool --------")]
    public GameObject AppleJuicePoolMainObject; 
    public GameObject[] AppleJuicePool;
    [HideInInspector] public int AppleJuicePoolIndex;

    public GameObject LemonJuicePoolMainObject;
    public GameObject[] LemonJuicePool;
    [HideInInspector] public int LemonJuicePoolIndex;

    public GameObject OrangeJuicePoolMainObject;
    public GameObject[] OrangeJuicePool;
    [HideInInspector] public int OrangeJuicePoolIndex;


    GameObject AvailableMoney;
    [Header("-------- Money Pool --------")]
    public GameObject MoneyPoolMainObject; 
    public GameObject[] MoneyPool;
    [HideInInspector] public int MoneyPoolIndex;


    [Header("-------- Scripts --------")]
    public PlayerController _PlayerController;
    public List<FruitProductionMachine> Machines;
    public List<FieldUpgrade> _FieldUpgrades;
    public List<MachineUpgrade> _MachineUpgrade;
    public CustomerManager _CustomerManager;
    public JuiceOrderTable _JuiceOrderTable;
    public CollectMoneyZone _CollectMoneyZone;
    public GameSaveSystem _GameSaveSystem;
    public OfficeManager _OfficeManager;

    // Settings
    [SerializeField] GameObject SettingsPanel;
    [SerializeField] Button GameSoundButton;
    [SerializeField] Button SfxSoundsButton;
    bool isSettingsOpen;

    // Sound
    [SerializeField] AudioSource GameBackgroundSound;
    [SerializeField] AudioSource[] SfxSounds;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);        
        }
    }

    void Start()
    {
        ApplySoundSettings();      
    }

    void SetButtonAlpha(float alpha, Button selectedButton)
    {
        Color c = selectedButton.image.color;
        c.a = alpha;
        selectedButton.image.color = c;
    }

    public void LoadGeneralData()
    {
        Money = _GameSaveSystem._AllData[0].General.Money;
        moneyText.text = Money.ToString();
    }

    public void UpdateMoney(int Amount)
    {
        Money += Amount;
        _GameSaveSystem._AllData[0].General.Money = Money;
        moneyText.text = Money.ToString();
    }

    public GameObject GetAvailableFruitFromPool(ItemType produceType)
    {
        switch (produceType)
        {
            case ItemType.Apple:
                AvailableFruit = ApplePool[ApplePoolIndex];
                ApplePoolIndex++;
                if (ApplePoolIndex == ApplePool.Length - 1)
                {
                    ApplePoolIndex = 0;
                }
                break;
            case ItemType.Lemon:
                AvailableFruit = LemonPool[LemonPoolIndex];
                LemonPoolIndex++;
                if (LemonPoolIndex == LemonPool.Length - 1)
                {
                    LemonPoolIndex = 0;
                }
                break;
            case ItemType.Orange:
                AvailableFruit = OrangePool[OrangePoolIndex];
                OrangePoolIndex++;
                if (OrangePoolIndex == OrangePool.Length - 1)
                {
                    OrangePoolIndex = 0;
                }
                break;
        }

        return AvailableFruit;
    }

    public GameObject GetAvailableFruitJuiceFromPool(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Apple:
                AvailableFruitJuice = AppleJuicePool[AppleJuicePoolIndex];
                AppleJuicePoolIndex++;
                if (AppleJuicePoolIndex == AppleJuicePool.Length - 1)
                {
                    AppleJuicePoolIndex = 0;
                }
                break;
            case ItemType.Lemon:
                AvailableFruitJuice = LemonJuicePool[LemonJuicePoolIndex];
                LemonJuicePoolIndex++;
                if (LemonJuicePoolIndex == LemonJuicePool.Length - 1)
                {
                    LemonJuicePoolIndex = 0;
                }
                break;
            case ItemType.Orange:
                AvailableFruitJuice = OrangeJuicePool[OrangeJuicePoolIndex];
                OrangeJuicePoolIndex++;
                if (OrangeJuicePoolIndex == OrangeJuicePool.Length - 1)
                {
                    OrangeJuicePoolIndex = 0;
                }
                break;
        }

        return AvailableFruitJuice;
    }

    public GameObject GetAvailableMoneyFromPool()
    {
        AvailableMoney = MoneyPool[MoneyPoolIndex];

        if (AvailableMoney.activeInHierarchy)
        {
            AvailableMoney = null;
        }
        MoneyPoolIndex++;
        
        if (MoneyPoolIndex == MoneyPool.Length - 1)
        {
            MoneyPoolIndex = 0;
        }
        return AvailableMoney;
    }

    // Settings & Sound
    void ApplySoundSettings()
    {
        if (PlayerPrefs.GetInt("GameVolume") == 0)
        {
            SetButtonAlpha(0.5f, GameSoundButton);
            GameBackgroundSound.mute = true;
        }
        else
        {
            SetButtonAlpha(1f, GameSoundButton);
            GameBackgroundSound.mute = false;
        }

        if (PlayerPrefs.GetInt("SfxVolume") == 0)
        {
            SetButtonAlpha(0.5f, SfxSoundsButton);
            foreach(var item in SfxSounds)
            {
                item.mute = true;
            }         
        }
        else
        {
            SetButtonAlpha(1f, SfxSoundsButton);
            foreach (var item in SfxSounds)
            {
                item.mute = false;
            }
        }
    }

    // Button & UI Action
    public void HandleButtonAction(int actionIndex)
    {
        PlaySound(0); // Button Click sound
        UIAction action = (UIAction)actionIndex;
        switch (action)
        {
            case UIAction.ToggleSettings:
                ToggleSettings();
                break;
            case UIAction.ToggleGameSound:
                ToggleGameSound();
                break;
            case UIAction.ToggleSfxSound:
                ToggleSfxSound();
                break;

        }
    }

    void ToggleSettings()
    {       
        if (isSettingsOpen)
        {
            isSettingsOpen = false;
            Time.timeScale = 1;
        }
        else
        {
            isSettingsOpen = true;
            Time.timeScale = 0;
        }
        SettingsPanel.SetActive(isSettingsOpen);
    }

    void ToggleGameSound()
    {
        if (PlayerPrefs.GetInt("GameVolume") == 0)
        {
            PlayerPrefs.SetInt("GameVolume", 1);
            SetButtonAlpha(1f, GameSoundButton);
            GameBackgroundSound.mute = false;
        }
        else
        {
            PlayerPrefs.SetInt("GameVolume", 0);
            SetButtonAlpha(0.4f, GameSoundButton);
            GameBackgroundSound.mute = true;
        }
    }

    void ToggleSfxSound()
    {
        if (PlayerPrefs.GetInt("SfxVolume") == 0)
        {
            PlayerPrefs.SetInt("SfxVolume", 1);
            SetButtonAlpha(1f, SfxSoundsButton);
            foreach (var item in SfxSounds)
            {
                item.mute = false;
            }
        }
        else
        {
            PlayerPrefs.SetInt("SfxVolume", 0);
            SetButtonAlpha(0.4f, SfxSoundsButton);
            foreach (var item in SfxSounds)
            {
                item.mute = true;
            }
        }
    }

    // Sfx Play 
    public void PlaySound(int SfxID)
    {
        SfxSounds[SfxID].Play();
    }

}
