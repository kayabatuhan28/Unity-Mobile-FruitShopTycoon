using MobileTycoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceOrderTable : MonoBehaviour
{
    public List<MachineSlots> AppleCupSlots;
    public List<MachineSlots> LemonCupSlots;
    public List<MachineSlots> OrangeCupSlots;

    public GameObject AppleCupPoolMainObject;
    public GameObject LemonCupPoolMainObject;
    public GameObject OrangeCupPoolMainObject;

    Coroutine IE_MoveCubToPool;
    int SlotID;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = GameManager.instance._PlayerController;
        if (pc == null) return;

        int id = pc.CurrentCarryCount - 1;
        if (pc.IsItemAtIndexCup(id))
        {
            IE_MoveCubToPool = StartCoroutine(MoveCubToPool());           
        }

    }

    int GetAvailableIndexFromPool(List<MachineSlots> SelectedList)
    {
        SlotID = -1;
        foreach(var item in SelectedList)
        {
            if (!item.IsSlotOccupied)
            {
                SlotID = SelectedList.IndexOf(item);
                break;
            }
        }
        return SlotID;
    }

    IEnumerator MoveCubToPool()
    {
        while(true)
        {
            PlayerController pc = GameManager.instance._PlayerController;
            
            int topId = pc.CurrentCarryCount - 1;
            if (topId == -1) // No available space
            {
                if (IE_MoveCubToPool != null)
                {
                    StopCoroutine(IE_MoveCubToPool);
                    IE_MoveCubToPool = null;
                }
                break;
            }

            GameManager.instance.PlaySound(1);

            // There is a cup in the slot
            switch (pc._CarrySlots[topId].CarriedItemType)
            {
                case ItemType.AppleCup:
                    GetAvailableIndexFromPool(AppleCupSlots);
                    if (SlotID != -1)
                    {
                        // Cup
                        Transform availableSlot = AppleCupSlots[SlotID].CupSlot;
                        pc._CarrySlots[topId].CarriedObject.GetComponent<FruitJuice>().GetOrderTableInfo(availableSlot, AppleCupPoolMainObject.transform);

                        // Slot
                        AppleCupSlots[SlotID].CarriedObject = pc._CarrySlots[topId].CarriedObject;
                        AppleCupSlots[SlotID].IsSlotOccupied = true;
                        
                        // Character
                        pc.ClearSlot(topId);
                    }
                    break;
                case ItemType.LemonCup:
                    GetAvailableIndexFromPool(LemonCupSlots);
                    if (SlotID != -1)
                    {                       
                        Transform availableSlot = LemonCupSlots[SlotID].CupSlot;
                        pc._CarrySlots[topId].CarriedObject.GetComponent<FruitJuice>().GetOrderTableInfo(availableSlot, LemonCupPoolMainObject.transform);
                        LemonCupSlots[SlotID].CarriedObject = pc._CarrySlots[topId].CarriedObject;
                        LemonCupSlots[SlotID].IsSlotOccupied = true;
                        pc.ClearSlot(topId);
                    }
                    break;
                case ItemType.OrangeCup:
                    GetAvailableIndexFromPool(OrangeCupSlots);
                    if (SlotID != -1)
                    {
                        Transform availableSlot = OrangeCupSlots[SlotID].CupSlot;
                        pc._CarrySlots[topId].CarriedObject.GetComponent<FruitJuice>().GetOrderTableInfo(availableSlot, OrangeCupPoolMainObject.transform);
                        OrangeCupSlots[SlotID].CarriedObject = pc._CarrySlots[topId].CarriedObject;
                        OrangeCupSlots[SlotID].IsSlotOccupied = true;
                        pc.ClearSlot(topId);
                    }
                    break;
            }

            // If the cups are depleted
            if (pc.CurrentCarryCount == 0)
            {
                if (IE_MoveCubToPool != null)
                {
                    StopCoroutine(IE_MoveCubToPool);
                    IE_MoveCubToPool = null;
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (IE_MoveCubToPool != null)
        {
            StopCoroutine(IE_MoveCubToPool);
            IE_MoveCubToPool = null;
        }
    }

}
