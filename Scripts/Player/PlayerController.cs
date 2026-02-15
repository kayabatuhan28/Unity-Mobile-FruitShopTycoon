using MobileTycoon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    Animator animator;
    public float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;
    InputAction runAction;

    // Carry
    public Transform CarrySlotsParent; 
    public List<CarrySlots> _CarrySlots;
    public int CurrentCarryCount;
    public int MaxCarryCount;
    public Transform MaxSprite;
    int availableSlotIndex;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        runAction = InputSystem.actions.FindAction("Navigate");
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        Vector2 v2 = runAction.ReadValue<Vector2>();

        if (runAction.IsPressed())
        {
            var pos = new Vector3(v2.x, 0f, v2.y);
            characterController.SimpleMove(pos * moveSpeed);

            // Is moving
            if (pos.sqrMagnitude <= 0)
            {
                animator.SetBool("Run", false);
            }          
            animator.SetBool("Run", true);

            // Rotation
            var targetPosition = Vector3.RotateTowards(characterController.transform.forward, pos, rotationSpeed * Time.deltaTime, 0f);
            characterController.transform.rotation = Quaternion.LookRotation(targetPosition);
        }

        if (runAction.WasReleasedThisFrame())
        {
            animator.SetBool("Run", false);
        }

        if (CurrentCarryCount == MaxCarryCount)
        {
            MaxSprite.transform.LookAt(MaxSprite.transform.position + Camera.main.transform.forward);
        }
    }

    public Transform GetAvailableCarrySlot(ItemType itemType, GameObject requestedCarryObject)
    {
        for (int i = 0; i < _CarrySlots.Count; i++)
        {
            if (!_CarrySlots[i].IsFull)
            {
                _CarrySlots[i].CarriedObject = requestedCarryObject;
                _CarrySlots[i].IsFull = true;
                _CarrySlots[i].CarriedItemType = itemType;

                requestedCarryObject.transform.parent = CarrySlotsParent;
                return _CarrySlots[i].Slot;
            }
        }

        return null;
    }

    public void OnFruitAddedToCarry()
    {
        CurrentCarryCount++;

        if (CurrentCarryCount >= MaxCarryCount)
        {
            CurrentCarryCount = MaxCarryCount;
            MaxSprite.gameObject.SetActive(true);
        }
    }

    public bool HasAvailableCarrySlot()
    {
        return CurrentCarryCount < MaxCarryCount;
    }

    public bool IsTopObjectFruit()
    {
        ItemType TopObjectType = _CarrySlots[CurrentCarryCount - 1].CarriedItemType;
        if (TopObjectType == ItemType.Orange || TopObjectType == ItemType.Lemon || TopObjectType == ItemType.Apple)
        {
            return true;
        }
        return false;
    }

    public bool IsItemAtIndexCup(int id)
    {
        if (_CarrySlots[id].CarriedItemType == ItemType.AppleCup ||
            _CarrySlots[id].CarriedItemType == ItemType.LemonCup ||
            _CarrySlots[id].CarriedItemType == ItemType.OrangeCup)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ClearSlot(int slotID)
    {
        CurrentCarryCount--;
        if (MaxSprite.gameObject.activeInHierarchy)
        {
            MaxSprite.gameObject.SetActive(false);
        }

        _CarrySlots[slotID].CarriedObject = null;
        _CarrySlots[slotID].CarriedItemType = ItemType.None;
        _CarrySlots[slotID].IsFull = false;      
    }

}
