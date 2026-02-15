using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MobileTycoon
{
    [System.Serializable]
    public class TreeFruit
    {
        public GameObject Fruit;
        public Transform SpawnPoint;
        public float GrowthDuration;
        public bool IsGrown;
    }

    [System.Serializable]
    public class CarrySlots
    {
        public Transform Slot;
        public GameObject CarriedObject;
        public ItemType CarriedItemType; // To separate fruits and cups
        public bool IsFull;
    }

    public enum ItemType
    {
        None        = 0,
        Apple       = 1,
        Lemon       = 2,
        Orange      = 3,
        AppleCup    = 4,
        LemonCup    = 5,
        OrangeCup   = 6
    }

    [System.Serializable]
    public class MachineSlots
    {
        public Transform CupSlot;
        public GameObject CarriedObject;
        public bool IsSlotOccupied;
    }

    [System.Serializable]
    public class CustomerCarrySlots
    {
        public Transform Slot;
        public GameObject CarriedObject;      
        public bool IsFull;
    }

    [System.Serializable]
    public class MoneySlot
    {
        public Transform SlotPoint;
        public GameObject MoneyObject;
        public bool IsSlotOccupied;
    }

    // ----------------------------------------------------------------------------------------------
    // Data Save System
    // ----------------------------------------------------------------------------------------------

    // Field
    [System.Serializable]
    public class FieldMainList
    {
        public List<FieldSaveData> FieldsSavedData;
    }

    [System.Serializable]
    public class FieldSaveData
    {
        public List<SlotData> SlotDataList;
        public List<FieldManagerAndUpdateData> FieldManagerDataList;
    }

    [System.Serializable]
    public class SlotData
    {
        public int FieldID;
        public int SlotID;
        public int SetupCost;
        public bool IsOpen;
    }

    [System.Serializable]
    public class FieldManagerAndUpdateData
    {
        public float FieldProductionTime;
        public int FieldUpgradeLevel;
    }

    [System.Serializable]
    public class MachineMainList
    {
        public List<MachineSaveData> MachinesSavedData;
    }

    [System.Serializable]
    public class MachineSaveData
    {
        public List<MachineData> MachineDataList;
        public List<MachineManagerAndUpdateData> MachineManagerDataList;
    }

    [System.Serializable]
    public class MachineData
    {
        public int CurrentFruitCount;
        public int CurrentCupCount;      
    }

    [System.Serializable]
    public class MachineManagerAndUpdateData
    {
        public float CurrentProductionSpeed;
        public int ProductionSpeedUpgradeLevel;

        public int CurrentFruitCapacity;
        public int FruitCapacityUpgradeLevel;

        public int CurrentCupCapacity;
        public int CupCapacityUpgradeLevel;
    }

    // Player Upgrade 
    [System.Serializable]
    public class PlayerData
    {
        public int CurrentMoveSpeed;
        public int MoveSpeedUpgradeLevel;

        public int CurrentCarryCapacity;
        public int CurrentCarryCapacityUpgradeLevel;
    }

    // Customer Upgrade 
    [System.Serializable]
    public class CustomerData
    {
        public int CurrentCustomerCount;
        public int CustomerCountUpgradeLevel;      
    }

    // General Data (Money etc.)
    [System.Serializable]
    public class GeneralData
    {
        public int Money;      
    }

    // All Saved Data
    [System.Serializable]
    public class AllData
    {
        public List<FieldMainList> Fields;
        public List<MachineMainList> Machines;      
        public List<CustomerData> Customers;
        public PlayerData Player;
        public GeneralData General;
    }

    // Game Manager etc. Button & UI Action
    public enum UIAction
    {
        None             = 0,
        StartGame        = 1,
        Quit             = 2,
        ToggleSettings   = 3,
        ToggleGameSound  = 4,
        ToggleSfxSound   = 5
    }


}
