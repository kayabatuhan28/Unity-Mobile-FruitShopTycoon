using MobileTycoon;
using System.Collections.Generic;
using UnityEngine;

public class CollectMoneyZone : MonoBehaviour
{
    
    public List<MoneySlot> _MoneySlots;
    int SlotID = -1;

    bool IsTransferring;

    // Represents the remaining collectible money if all money slots are full
    public int RemainingMoney;

    private void Update()
    {
        if (IsTransferring)
        {
            foreach(var item in _MoneySlots)
            {
                if (item.IsSlotOccupied)
                {
                    if (item.MoneyObject != null)
                    {
                        item.MoneyObject.GetComponent<Money>().SetMoneyPosition(transform, GameManager.instance._PlayerController.transform, true);
                        item.IsSlotOccupied = false;
                        item.MoneyObject = null;

                        GameManager.instance.PlaySound(3);
                        GameManager.instance.UpdateMoney(50);
                    }
                    else // No money in the money slots
                    {
                        break;
                    }                 
                }
            }
        }
    }

    public int GetAvailableSlot()
    {
        SlotID = -1;
        foreach (var item in _MoneySlots)
        {
            if (!item.IsSlotOccupied)
            {
                SlotID = _MoneySlots.IndexOf(item);
                break;
            }
        }

        return SlotID;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        IsTransferring = true;
        GameManager.instance.UpdateMoney(RemainingMoney);
        RemainingMoney = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        IsTransferring = false;
    }


}
