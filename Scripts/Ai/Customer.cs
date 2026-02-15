using MobileTycoon;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
    public Transform StartPosition;
    public Transform TargetPosition;
    public GameObject FollowTarget;

    public NavMeshAgent agent;
    public Animator animator;
   
    Coroutine IE_WaitOrder;

    public bool IsFirstCustomer;
    [HideInInspector] public bool hasReceivedOrder;
    public bool canMove;

    // Order Operations
    ItemType juiceType;
    public GameObject orderMainIcon;
    public Image orderIcon;
    public TextMeshProUGUI orderQuantityText;
    int orderQuantity;
    int moneyAmount;

    int listIndex;

    // Operations after receiving the order
    bool isOrderTaken;
    bool canReturn;

    // Slot operations
    public Transform SlotsRoot;              
    public List<CustomerCarrySlots> _CustomerCarrySlots;
    int availableSlotIndex;

    List<MachineSlots> SlotsToCheck;           
    GameObject CupPoolRoot;                   
    int LastSlotID;                           

    void Start()
    {      
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        if (canMove && agent.enabled)
        {
            SetMovementEnabled(true);

            if (IsFirstCustomer || (FollowTarget != null && !FollowTarget.GetComponent<Customer>().hasReceivedOrder))
            {
                agent.SetDestination(TargetPosition.position);
                if (!hasReceivedOrder)
                {
                    // Customer In Front
                    if (agent.remainingDistance < 3f && TargetPosition != GameManager.instance._CustomerManager.orderPoint.transform)
                    {
                        SetMovementEnabled(false);
                    }
                    else
                    {
                        SetMovementEnabled(true);
                    }
                }
            }
            else
            {
                agent.isStopped = false;
                listIndex = GameManager.instance._CustomerManager.ActiveCustomers.IndexOf(transform);

                if (listIndex != 0) // Not The First Customer In Queue
                {
                    TargetPosition = GameManager.instance._CustomerManager.ActiveCustomers[listIndex - 1].transform;
                }
                else
                {
                    agent.stoppingDistance = 0;
                    TargetPosition = GameManager.instance._CustomerManager.orderPoint;
                    FollowTarget = null;
                }
            }
        }

        if (canReturn)
        {
            agent.SetDestination(TargetPosition.position);
        }

       
    }

    void SetMovementEnabled(bool enabled)
    {
        if (enabled)
        {
            agent.isStopped = false;
            animator.SetBool("Walk", true);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("Walk", false);
        }
    }

    public void GetOrderDetails(ItemType JuiceType, Sprite OrderIcon)
    {
        juiceType = JuiceType;
        orderIcon.sprite = OrderIcon;
        gameObject.SetActive(true);
        InitializeCustomerForOrder();
    }

    public void InitializeCustomerForOrder()
    {
        transform.position = StartPosition.position;
        orderQuantity = Random.Range(1, 5);
        moneyAmount = orderQuantity;
        
        agent.enabled = true;
        SetMovementEnabled(true);

        orderQuantityText.text = orderQuantity.ToString();

        AssignSlotsToCheck();
        canMove = true;
    }

    IEnumerator WaitOrder()
    {
        while (true)
        {
            foreach(var item in SlotsToCheck)
            {
                if (item.IsSlotOccupied)
                {
                    LastSlotID = SlotsToCheck.IndexOf(item);
                }
                else
                {
                    break;
                }
            }

            if (SlotsToCheck[LastSlotID].IsSlotOccupied)
            {
                // Cup operations
                SlotsToCheck[LastSlotID].CarriedObject.GetComponent<FruitJuice>().CurrentTarget = GetAvailableCarrySlot(SlotsToCheck[LastSlotID].CarriedObject);              
                SlotsToCheck[LastSlotID].CarriedObject.GetComponent<FruitJuice>().CanMoveOnce = true;
                orderQuantity--;
                orderQuantityText.text = orderQuantity.ToString();

                if (orderQuantity <= 0)
                {
                    hasReceivedOrder = true;                  

                    TargetPosition = GameManager.instance._CustomerManager.returnPoint;
                    GameManager.instance._CustomerManager.HandleCustomerExit(transform);
                    canReturn = true;
                    SetMovementEnabled(true);
                    orderMainIcon.SetActive(false);
                    GameManager.instance._CustomerManager.CurrentCustomerCount--;

                    // Money Transfer
                    for(int i = 0; i < moneyAmount; i++)
                    {
                        int availableIndex = GameManager.instance._CollectMoneyZone.GetAvailableSlot();  
                        if (availableIndex != -1) // If An Available Slot Exists
                        {
                            var slot = GameManager.instance._CollectMoneyZone._MoneySlots[availableIndex];
                            GameObject AvailableMoney = GameManager.instance.GetAvailableMoneyFromPool();

                            if (AvailableMoney != null)
                            {                                                             
                                slot.MoneyObject = AvailableMoney;
                                slot.IsSlotOccupied = true;
                                AvailableMoney.transform.position = transform.position;                               
                                AvailableMoney.GetComponent<Money>().SetMoneyPosition(transform, slot.SlotPoint);
                                AvailableMoney.SetActive(true);
                            }                                                  
                        }
                        else
                        {
                            GameManager.instance._CollectMoneyZone.RemainingMoney += moneyAmount * 50;
                        }
                    }

                    if (IE_WaitOrder != null)
                    {
                        StopCoroutine(IE_WaitOrder);
                        IE_WaitOrder = null;
                    }
                }

                SlotsToCheck[LastSlotID].CarriedObject = null;
                SlotsToCheck[LastSlotID].IsSlotOccupied = false;
            }
            
            yield return new WaitForSeconds(1);
        }
    }

    public Transform GetAvailableCarrySlot(GameObject requestedCarryObject)
    {
        foreach (var item in _CustomerCarrySlots)
        {
            if (!item.IsFull)
            {
                availableSlotIndex = _CustomerCarrySlots.IndexOf(item);
                _CustomerCarrySlots[availableSlotIndex].CarriedObject = requestedCarryObject;
                _CustomerCarrySlots[availableSlotIndex].IsFull = true;

                requestedCarryObject.transform.parent = SlotsRoot.transform;
                break;
            }
        }

        return _CustomerCarrySlots[availableSlotIndex].Slot;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("OrderPoint"))
        {
            canMove = false;           
            transform.position = TargetPosition.position;
            SetMovementEnabled(false);

            orderMainIcon.SetActive(true);
            IE_WaitOrder = StartCoroutine(WaitOrder());
        }

        if (other.gameObject.CompareTag("ReturnPoint"))
        {
            OnCustomerReachReturnPoint();
        }


    }

    // Resets customer data
    void OnCustomerReachReturnPoint()
    {
        canMove = false;
        agent.isStopped = true;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        animator.SetBool("Walk", false);
        agent.stoppingDistance = 3;
        agent.enabled = false;
        canReturn = false;

        if (IsFirstCustomer)
        {
            IsFirstCustomer = false;
        }
       
        ReturnCarriedCupsToPool();

        hasReceivedOrder = false;
        gameObject.SetActive(false);

    }

    void ReturnCarriedCupsToPool()
    {
        foreach(var item in _CustomerCarrySlots)
        {
            if (item.IsFull)
            {
                item.IsFull = false;
                item.CarriedObject.GetComponent<FruitJuice>().ResetAndReturnToPool();
                item.CarriedObject = null;
            }
        }
    }

    void AssignSlotsToCheck()
    {
        switch (juiceType)
        {
            case ItemType.AppleCup:
                SlotsToCheck = GameManager.instance._JuiceOrderTable.AppleCupSlots;              
                break;
            case ItemType.LemonCup:
                SlotsToCheck = GameManager.instance._JuiceOrderTable.LemonCupSlots;                
                break;
            case ItemType.OrangeCup:
                SlotsToCheck = GameManager.instance._JuiceOrderTable.OrangeCupSlots;               
                break;
        }
    }




}
