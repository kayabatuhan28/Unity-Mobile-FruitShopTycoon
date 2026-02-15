using UnityEngine;
using UnityEngine.UI;
using MobileTycoon;
using System.Collections.Generic;
using System.Collections;



public class Tree : MonoBehaviour
{
    public float growthDuration = 5;
    [SerializeField] int FieldID;
    [SerializeField] ItemType ProducedItemType;  
    [SerializeField] Image harvestProgressImage;

    public List<TreeFruit> TreeFruits = new List<TreeFruit>();

    public int FruitsOnTree;
    int maxFruitCapacity = 10;

    Vector3 growthScale;

    Coroutine IE_GrowthFruit;
    Coroutine IE_GiveFruitToPlayer;

    bool playerInRange;

    void Start()
    {
        growthDuration = GameManager.instance._FieldUpgrades[FieldID].ProductionDuration;

        TryStartGrowth();
    }


    IEnumerator GrowthFruit()
    {
        growthScale = new Vector3(10, 10, 10) / growthDuration;

        while (true)
        {
            yield return new WaitForSeconds(growthDuration / 5f);

            bool hasAnyGrowingOrEmptySlot = false;

            for (int i = 0; i < TreeFruits.Count; i++)
            {
                var slot = TreeFruits[i];

                if (slot.Fruit == null)
                {
                    GameObject fruitObj = GameManager.instance.GetAvailableFruitFromPool(ProducedItemType);


                    fruitObj.transform.position = slot.SpawnPoint.position;
                    /* 
                    if (productionFruitType == ProductionFruitType.Lemon)
                    {
                        fruitObj.transform.localScale = new Vector3(16f, 16f, 16f);
                    }
                    else
                    {
                        fruitObj.transform.localScale = new Vector3(5f, 5f, 5f);
                    }
                    */
                    
                    fruitObj.SetActive(true);

                    slot.Fruit = fruitObj;
                    slot.GrowthDuration = 0;
                    slot.IsGrown = false;

                    hasAnyGrowingOrEmptySlot = true;
                }
            }

            // Grow all fruits simultaneously
            for (int i = 0; i < TreeFruits.Count; i++)
            {
                var slot = TreeFruits[i];

                if (slot.Fruit == null || slot.IsGrown)
                    continue;

                slot.GrowthDuration += growthDuration / 5;
                slot.Fruit.transform.localScale += growthScale;
                             
                hasAnyGrowingOrEmptySlot = true;

                if (slot.GrowthDuration >= growthDuration)
                {
                    slot.IsGrown = true;
                    FruitsOnTree++;
                    UpdateHarvestProgressUI();
                }
            }

            // Stop if there is nothing left to grow or spawn
            if (!hasAnyGrowingOrEmptySlot)
            {
                IE_GrowthFruit = null;
                yield break;
            }
        }
    }

    void TryStartGrowth()
    {
        if (IE_GrowthFruit == null)
        {
            IE_GrowthFruit = StartCoroutine(GrowthFruit());
        }
    }

    void UpdateHarvestProgressUI()
    {
        harvestProgressImage.fillAmount = FruitsOnTree / (float)maxFruitCapacity;
    }


    // Character interaction section 
    IEnumerator GiveFruitsToPlayer()
    {
        foreach (var item in TreeFruits)
        {
            if (!playerInRange)
                yield break;

            if (!item.IsGrown)
                continue;

            if (!GameManager.instance._PlayerController.HasAvailableCarrySlot())
                yield break;

            if (GameManager.instance._PlayerController.CurrentCarryCount > 0 &&
                !GameManager.instance._PlayerController.IsTopObjectFruit())
                yield break;

            Fruit fruit = item.Fruit.GetComponent<Fruit>();
            fruit.Target = GameManager.instance._PlayerController.GetAvailableCarrySlot(fruit.FruitType, item.Fruit);
            fruit.CanMove = true;

            FruitsOnTree--;
            UpdateHarvestProgressUI();
            GameManager.instance.PlaySound(5);

            item.Fruit = null;
            item.GrowthDuration = 0;
            item.IsGrown = false;

            yield return new WaitForSeconds(0.2f);
        }
        IE_GiveFruitToPlayer = null;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (IE_GiveFruitToPlayer == null)
        {
            IE_GiveFruitToPlayer = StartCoroutine(GiveFruitsToPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (IE_GiveFruitToPlayer != null)
        {
            StopCoroutine(IE_GiveFruitToPlayer);
            IE_GiveFruitToPlayer = null;
        }

        CancelFlyingFruits();
        TryStartGrowth();
    }

    void CancelFlyingFruits()
    {
        foreach (var slot in TreeFruits)
        {
            if (slot.Fruit == null)
                continue;

            Fruit fruit = slot.Fruit.GetComponent<Fruit>();

            if (fruit.CanMove)
            {
                fruit.CanMove = false;
                fruit.Target = null;
            }
        }
    }



}
