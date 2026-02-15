using MobileTycoon;
using UnityEngine;

public class CupCollectionZone : MonoBehaviour
{
    public FruitProductionMachine OwnerMachine;


    // Trigger Collider
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        PlayerController pc = GameManager.instance._PlayerController;
        if (pc.CurrentCarryCount >= pc.MaxCarryCount) return;

        if (pc.CurrentCarryCount != 0)
        {
            // Hands are full               
            if (!pc.IsTopObjectFruit())
            {
                OwnerMachine.GiveCupsToPlayer();
            }
        }
        else
        {
            // Hands are empty
            OwnerMachine.GiveCupsToPlayer();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        PlayerController pc = GameManager.instance._PlayerController;
        if (pc.CurrentCarryCount >= pc.MaxCarryCount) return;

        if (pc.CurrentCarryCount != 0)
        {               
            if (!pc.IsTopObjectFruit())
            {
                OwnerMachine.GiveCupsToPlayer();
            }
        }
        else
        {
            OwnerMachine.GiveCupsToPlayer();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        OwnerMachine.GiveCupsToPlayer(false);

    }


}
