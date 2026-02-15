using MobileTycoon;
using Unity.VisualScripting;
using UnityEngine;

public class FruitJuice : MonoBehaviour
{
    bool canMove;


    [Header("------ General ------")]
    public bool CanMoveOnce;
    private bool destroyOnArrival;

    public Transform TransferPosition;      
    public Transform CurrentTarget;         
    public ItemType FruitJuiceType;                
    public int OwnerMachineID;              

    float ProductionMoveSmoothness = 5f;    
    float MoveSmoothness = 20f;              

    bool IsFinalTarget;                     
    Vector3 Position;                       

  
    void Update()
    {
        CheckCanMove();
        CheckCanMoveOnce();                 
    }
 
    void CheckCanMove()
    {
        if (canMove)
        {
            Position = CurrentTarget.position - transform.position;
            transform.Translate(ProductionMoveSmoothness * Time.deltaTime * Position.normalized, Space.World);

            if (Vector3.Distance(transform.position, CurrentTarget.position) <= 0.12f)
            {
                transform.position = CurrentTarget.position;
                if (!IsFinalTarget)
                {
                    GetNextPosition();
                }
                else
                {
                    canMove = false;
                }
            }
        }
    }

    void CheckCanMoveOnce()
    {
        if (CanMoveOnce)
        {
            transform.position = Vector3.Lerp(transform.position, CurrentTarget.position, Time.deltaTime * MoveSmoothness);

            if (Vector3.Distance(transform.position, CurrentTarget.position) <= 0.12f)
            {
                transform.position = CurrentTarget.position;
                if (destroyOnArrival)
                {
                    ResetAndReturnToPool();
                }
                CanMoveOnce = false;
            }
        }
    }

    public void MoveCupsToTrashBin(Transform Position)
    {       
        transform.SetParent(null, false);
        CurrentTarget = Position;
        CanMoveOnce = true;
        destroyOnArrival = true;
    }

    void GetNextPosition()
    {
        TransferPosition = GameManager.instance.Machines[OwnerMachineID].GetAvailableSlotPosition(gameObject);
        CurrentTarget = TransferPosition.transform;
        IsFinalTarget = true;
    }

    public void GetMachineInfo(Transform StartPos, Transform FirstTarget)
    {
        transform.position = StartPos.position;
        CurrentTarget = FirstTarget.transform;
        canMove = true;
        gameObject.SetActive(true);
    }

    public void GetOrderTableInfo(Transform Target, Transform ParentObject)
    {
        CurrentTarget = Target;
        CanMoveOnce = true;
        transform.parent = ParentObject;

        GameManager gm = GameManager.instance;

        // Scale offset
        switch (FruitJuiceType)
        {
            case ItemType.AppleCup:
                transform.localScale = new(0.12f, 0.056f, 1.081f);
                break;
            case ItemType.LemonCup:
                transform.localScale = new(0.071f, 0.032f, 1.01f);
                break;
            case ItemType.OrangeCup:
                transform.localScale = new(0.071f, 0.032f, 1.01f);
                break;
        }

        // Rotation offset
        transform.localRotation = Quaternion.identity;
    }

    public void ResetAndReturnToPool()
    {            
        TransferPosition = null;
        CurrentTarget = null;
        destroyOnArrival = false;
        CanMoveOnce = false;

        GameManager gm = GameManager.instance;

        switch (FruitJuiceType)
        {
            case ItemType.AppleCup:
                transform.parent = gm.AppleJuicePoolMainObject.transform;
                break;
            case ItemType.LemonCup:
                transform.parent = gm.LemonJuicePoolMainObject.transform;
                break;
            case ItemType.OrangeCup:
                transform.parent = gm.OrangeJuicePoolMainObject.transform;
                break;
        }

        transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(-90, 0, -90));
        gameObject.SetActive(false);
    }


}
