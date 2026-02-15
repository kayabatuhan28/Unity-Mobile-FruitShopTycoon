using System.Collections;
using UnityEngine;
using MobileTycoon;

public class TrashBin : MonoBehaviour
{
    [SerializeField] Transform trashDisposePoint;

    Coroutine IE_DisposeItemsRoutine;

    IEnumerator DisposeItemsRoutine()
    {
        while (true)
        {
            PlayerController pc = GameManager.instance._PlayerController;
            int topItemIndex = pc.CurrentCarryCount - 1;
            if (topItemIndex != -1)
            {
                //Debug.Log("CarriedItemType: " + pc._CarrySlots[topItemIndex].CarriedItemType);
                switch (pc._CarrySlots[topItemIndex].CarriedItemType)
                {
                    case ItemType.Apple:                      
                    case ItemType.Lemon:                       
                    case ItemType.Orange:
                        pc._CarrySlots[topItemIndex].CarriedObject.GetComponent<Fruit>().MoveToMachine(trashDisposePoint);
                        break;
                    case ItemType.AppleCup:
                    case ItemType.LemonCup:
                    case ItemType.OrangeCup:
                        pc._CarrySlots[topItemIndex].CarriedObject.GetComponent<FruitJuice>().MoveCupsToTrashBin(trashDisposePoint);
                        break;
                }

                GameManager.instance.PlaySound(1);

                // Karakter Ýþlemleri-
                pc.ClearSlot(topItemIndex);
                if (pc.CurrentCarryCount == 0)
                {
                    StopDisposeRoutine();
                }
            }
            else
            {
                StopDisposeRoutine();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        PlayerController pc = GameManager.instance._PlayerController;
        if (pc == null) return;

        if (pc.CurrentCarryCount != 0)
        {
            IE_DisposeItemsRoutine = StartCoroutine(DisposeItemsRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        StopDisposeRoutine();
    }

    void StopDisposeRoutine()
    {
        if (IE_DisposeItemsRoutine != null)
        {
            StopCoroutine(IE_DisposeItemsRoutine);
            IE_DisposeItemsRoutine = null;
        }
    }

}
