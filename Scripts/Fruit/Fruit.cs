using MobileTycoon;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public bool CanMove;
    public bool IsMovingToMachine;
    public Transform Target;
    public ItemType FruitType;

    float movementSmoothness = 100f;

    bool hasReachedTarget;

    void Update()
    {
        UpdateMovement();
    }

    
    void UpdateMovement()
    {
        if (!CanMove || Target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, Target.position, Time.deltaTime * movementSmoothness);

        if (Vector3.Distance(transform.position, Target.position) <= 0.12f)
        {
            if (IsMovingToMachine)
            {
                ClearData();
            }
            else
            {
                OnReachedTarget();
            }          
        }
    }

    void OnReachedTarget()
    {
        hasReachedTarget = true;
        CanMove = false;
        transform.position = Target.position;

        GameManager.instance._PlayerController.OnFruitAddedToCarry();
    }


    public void ResetFruit()
    {
        hasReachedTarget = false;
        Target = null;
        CanMove = false;
    }

    public void MoveToMachine(Transform targetPos)
    {
        Target = targetPos;
        CanMove = true;
        IsMovingToMachine = true;
    }

    public void ClearData()
    {     
        switch (FruitType)
        {
            case ItemType.Apple:
                transform.localScale = new Vector3(5, 5, 5);
                transform.parent = GameManager.instance.ApplePoolMainObject.transform;
                break;
            case ItemType.Lemon:
                transform.localScale = new Vector3(16, 16, 16);
                transform.parent = GameManager.instance.LemonPoolMainObject.transform;
                break;
            case ItemType.Orange:
                transform.localScale = new Vector3(5, 5, 5);
                transform.parent = GameManager.instance.OrangePoolMainObject.transform;
                break;
        }

        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Target = null;
        IsMovingToMachine = false;
        CanMove = false;
        gameObject.SetActive(false);
    }
}
