using MobileTycoon;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class FruitProductionMachine : MonoBehaviour
{
    public int MachineID;
    public ItemType AcceptedItemType;
  
    public bool IsActive;  // Used to start/stop the machine when there are not enough fruits to produce juice
    private bool AreSlotsFull;
    private int SlotID = -1;
    private int lastSlotID;

    public int CurrentFruitCount;     // How many fruits are currently in the machine
    public int CurrentCupCount;       // Input: fruit - Output: cup

    public int maxCupCapacity;
    public float ProductionDuration;
    public int totalFruitCapacity;

    [SerializeField] private Renderer conveyorBelt; 

    private Coroutine IE_CollectFruits;
    private Coroutine IE_ProduceJuice;
    private Coroutine IE_TransferCupsToPlayer;

    public List<MachineSlots> MachineSlots;
    public GameObject[] TransferPoints;

    public GameObject NoStockObject;
    public Image FruitFillBar;
    public Image CupFillBar;

    [HideInInspector] public TextMeshProUGUI currentFruitCountText;
    [HideInInspector] public TextMeshProUGUI maxFruitCapacityText;

    [HideInInspector] public TextMeshProUGUI currentCupCountText;
    [HideInInspector] public TextMeshProUGUI maxCupCapacityText;



    void Start()
    {
        CurrentFruitCount = GameManager.instance._GameSaveSystem.GetMachineCurrentFruitCapacity(MachineID);
        CurrentCupCount = GameManager.instance._GameSaveSystem.GetMachineCurrentCupCapacity(MachineID);           
    }

    public void StartMachine()
    {
        if (CurrentFruitCount != 0)
        {
            IE_ProduceJuice ??= StartCoroutine(ProduceJuice());
        }

        if (CurrentFruitCount == 0)
        {
            NoStockObject.SetActive(true);
            IsActive = false;
        }
    }

    private void Update()
    {
        if (IsActive)
        {
            conveyorBelt.material.SetTextureOffset("_BaseMap", new Vector2(0.16f, -Time.time * 2f));
        }
    }

    IEnumerator ProduceJuice()
    {
        IsActive = true;
        NoStockObject.SetActive(false);

        while (true)
        {
            if (CurrentFruitCount != 0)
            {
                // All produced juice slots are full, stop production.
                if (GetAvailableMachineSlot() == -1)
                {
                    if (IE_ProduceJuice != null)
                    {
                        StopCoroutine(IE_ProduceJuice);
                        IE_ProduceJuice = null;
                        IsActive = false;
                        AreSlotsFull = true;
                        break;
                    }
                }

                // Fruit state check
                CurrentFruitCount--;
                GameManager.instance._GameSaveSystem.SetMachineCurrentFruitCapacity(MachineID, CurrentFruitCount);

                UpdateFruipFillBar();

                if (CurrentFruitCount <= 0)
                {
                    if (IE_ProduceJuice != null)
                    {
                        StopCoroutine(IE_ProduceJuice);
                        IE_ProduceJuice = null;
                        IsActive = false;
                        NoStockObject.SetActive(true);
                    }
                }

                // Production is possible
                GameObject Juice = GameManager.instance.GetAvailableFruitJuiceFromPool(AcceptedItemType);
                Juice.GetComponent<FruitJuice>().GetMachineInfo(TransferPoints[1].transform, TransferPoints[2].transform);
                
                yield return new WaitForSeconds(3f);

                CurrentCupCount++;
                GameManager.instance._GameSaveSystem.SetMachineCurrentCupCapacity(MachineID, CurrentCupCount);
                UpdateCupFillBar();

                // Cup state check
                if (CurrentCupCount == maxCupCapacity)
                {
                    //Debug.Log("Cup Capacity is full!!");
                    if (IE_ProduceJuice != null)
                    {
                        StopCoroutine(IE_ProduceJuice);
                        IE_ProduceJuice = null;
                        IsActive = false;
                    }

                    if (GetAvailableMachineSlot() == -1)
                    {
                        AreSlotsFull = true;
                    }
                }               
            }
            else
            {
                if (IE_ProduceJuice != null)
                {
                    StopCoroutine(IE_ProduceJuice);
                    IE_ProduceJuice = null;
                    IsActive = false;
                    NoStockObject.SetActive(true);
                }
            }

            yield return new WaitForSeconds(ProductionDuration);
        }
    }

    public Transform GetAvailableSlotPosition(GameObject Cup)
    {
        GetAvailableMachineSlot();
        MachineSlots[SlotID].CarriedObject = Cup;
        MachineSlots[SlotID].IsSlotOccupied = true;

        return MachineSlots[SlotID].CupSlot;
    }

    int GetAvailableMachineSlot()
    {
        SlotID = -1;
        foreach(var item in MachineSlots)
        {
            if (!item.IsSlotOccupied)
            {
                SlotID = MachineSlots.IndexOf(item);
                break;
            }
        }

        return SlotID;
    }
    
    IEnumerator CollectFruits()
    {
        for (int i = GameManager.instance._PlayerController.CurrentCarryCount - 1; i != -1; i--)
        {
            if (GameManager.instance._PlayerController._CarrySlots[i].CarriedItemType == AcceptedItemType)
            {
                GameManager.instance._PlayerController._CarrySlots[i].CarriedObject.GetComponent<Fruit>().MoveToMachine(TransferPoints[0].transform);
                GameManager.instance._PlayerController.ClearSlot(i);

                // Machine
                CurrentFruitCount++;
                GameManager.instance._GameSaveSystem.SetMachineCurrentFruitCapacity(MachineID, CurrentFruitCount);
                UpdateFruipFillBar();
                GameManager.instance.PlaySound(5);

                // Check and start production
                if (!AreSlotsFull)
                {
                    IE_ProduceJuice ??= StartCoroutine(ProduceJuice());                  
                }

                // Stop production
                if (GameManager.instance._PlayerController.CurrentCarryCount == 0)
                {                  
                    if (IE_CollectFruits != null)
                    {
                        StopCoroutine(IE_CollectFruits);
                        IE_CollectFruits = null;
                    }
                }

                if (CurrentFruitCount >= totalFruitCapacity)
                {
                    CurrentFruitCount = totalFruitCapacity;
                    if (IE_CollectFruits != null)
                    {
                        StopCoroutine(IE_CollectFruits);
                        IE_CollectFruits = null;
                    }
                }

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                if (IE_CollectFruits != null)
                {
                    StopCoroutine(IE_CollectFruits);
                    IE_CollectFruits = null;
                }
            }
        }  
    }

    // Transfer cup to player
    public void GiveCupsToPlayer(bool isActive = true)
    {
        if (isActive)
        {
            IE_TransferCupsToPlayer ??= StartCoroutine(TransferCupsToPlayer());
        }
        else
        {
            if (IE_TransferCupsToPlayer != null)
            {
                StopCoroutine(IE_TransferCupsToPlayer);
                IE_TransferCupsToPlayer = null;
            }
        }
    }

    int GetLastCupIndex()
    {
        foreach(var item in MachineSlots)
        {
            if (item.IsSlotOccupied)
            {
                lastSlotID = MachineSlots.IndexOf(item);              
            }
            else
            {
                break;
            }
        }

        return lastSlotID;
    }

    IEnumerator TransferCupsToPlayer()
    {
        while (true)
        {
            GetLastCupIndex();
            if (MachineSlots[lastSlotID].IsSlotOccupied)
            {
                FruitJuice fj = MachineSlots[lastSlotID].CarriedObject.GetComponent<FruitJuice>();
                PlayerController pc = GameManager.instance._PlayerController;

                // Character operations
                fj.CurrentTarget = pc.GetAvailableCarrySlot(fj.FruitJuiceType, MachineSlots[lastSlotID].CarriedObject);
                fj.CanMoveOnce = true;
              
                pc.CurrentCarryCount++;
                if(pc.CurrentCarryCount >= pc.MaxCarryCount)
                {
                    pc.CurrentCarryCount = pc.MaxCarryCount;
                    pc.MaxSprite.gameObject.SetActive(true);
                    GiveCupsToPlayer(false);
                }

                // Machine operations
                CurrentCupCount--;
                GameManager.instance._GameSaveSystem.SetMachineCurrentCupCapacity(MachineID, CurrentCupCount);
                UpdateCupFillBar();
                GameManager.instance.PlaySound(1);

                if (CurrentCupCount == 0)
                {
                    GiveCupsToPlayer(false);
                }

                // Clear
                MachineSlots[lastSlotID].CarriedObject = null;
                MachineSlots[lastSlotID].IsSlotOccupied = false;

            }
            yield return new WaitForSeconds(0.2f);
        }     
    }


    // UI Update
    public void UpdateFruipFillBar()
    {
        FruitFillBar.fillAmount = Mathf.Clamp01((float)CurrentFruitCount / totalFruitCapacity);
        UpdateFruitText();

    }
    public void UpdateCupFillBar()
    {
        CupFillBar.fillAmount = Mathf.Clamp01((float)CurrentCupCount / maxCupCapacity);
        UpdateCupText();
    }
     void UpdateFruitText()
    {
        currentFruitCountText.text = CurrentFruitCount.ToString();
        maxFruitCapacityText.text = totalFruitCapacity.ToString();
    }
    public void UpdateCupText()
    {
        currentCupCountText.text = CurrentCupCount.ToString();
        maxCupCapacityText.text = maxCupCapacity.ToString();
    }

    public void CheckProduction()
    {
        if (CurrentFruitCount != 0)
        {
            IE_ProduceJuice ??= StartCoroutine(ProduceJuice());
        }
    }

    public void CheckProductionAndCups()
    {
        CheckProduction();
        
        if (CurrentCupCount != 0)
        {
            for (int i = 0; i < CurrentCupCount; i++)
            {
                GameObject Cup = GameManager.instance.GetAvailableFruitJuiceFromPool(AcceptedItemType);
                Cup.transform.position = GetAvailableSlotPosition(Cup).position;
                Cup.SetActive(true);
            }
        }          
    }

    // Trigger Collider
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        PlayerController pc = GameManager.instance._PlayerController;
        if (pc.CurrentCarryCount == 0) return;
                                             
        if (pc._CarrySlots[pc.CurrentCarryCount - 1].CarriedItemType != AcceptedItemType) return;

        if (CurrentFruitCount == totalFruitCapacity) return;
       
        IE_CollectFruits = StartCoroutine(CollectFruits());      
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (IE_CollectFruits != null)
        {
            StopCoroutine(IE_CollectFruits);
            IE_CollectFruits = null;
        }

    }


    
   






}
