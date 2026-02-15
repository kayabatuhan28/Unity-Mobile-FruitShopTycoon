using MobileTycoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public GameObject[] Customers;
    int customerPoolIndex;

    public int CurrentCustomerCount;
    public int MaxCustomerLimit = 10;

    public Transform orderPoint;
    public Transform returnPoint;

    public List<Transform> ActiveCustomers;

    public Sprite[] OrderIcons;
    public ItemType[] JuiceTypes;
    int OrderIndex;

    void Start()
    {
        StartCoroutine(SendCustomerToOrder());
    }

    IEnumerator SendCustomerToOrder()
    {
        while (true)
        {
            if (CurrentCustomerCount < MaxCustomerLimit)
            {
                ActiveCustomers.Add(Customers[customerPoolIndex].transform);
                Customer customer = Customers[customerPoolIndex].GetComponent<Customer>();
                int indexInList = ActiveCustomers.IndexOf(Customers[customerPoolIndex].transform);
                if (indexInList != 0) // Not the first customer in queue
                {
                    // Target the next customer
                    customer.TargetPosition = ActiveCustomers[indexInList - 1].transform;
                    customer.FollowTarget = ActiveCustomers[indexInList - 1].gameObject;
                }
                else
                {
                    customer.TargetPosition = orderPoint;
                    customer.agent.stoppingDistance = 0;
                }

                customer.GetOrderDetails(JuiceTypes[OrderIndex], OrderIcons[OrderIndex]);
                if (OrderIndex >= 2)
                {
                    OrderIndex = 0;
                }
                else
                {
                    OrderIndex++;
                }
               
                if (customerPoolIndex != Customers.Length - 1)
                {
                    customerPoolIndex++;
                }
                else
                {
                    customerPoolIndex = 0;
                }

                CurrentCustomerCount++;
                yield return new WaitForSeconds(Random.Range(2,5));
            }
            else
            {
                yield return new WaitForSeconds(5);
            }
        }
    }

    public void HandleCustomerExit(Transform customer)
    {
        ActiveCustomers.Remove(customer);
    }


}
