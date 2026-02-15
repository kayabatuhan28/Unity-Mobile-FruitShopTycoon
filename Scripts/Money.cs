using UnityEngine;

public class Money : MonoBehaviour
{
    public bool CanMove;
    public bool CanReset;

    private Transform target;
    private float movementSmoothness = 20f;


    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        // Transfer money from customer to cash desk
        if (CanMove)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * movementSmoothness);
            
            if (Vector3.Distance(transform.position, target.position) <= 0.2f)
            {
                transform.position = target.position;

                if (CanReset)
                {
                    ResetData();
                }             
            }
        }
    }

    public void SetMoneyPosition(Transform StartPos, Transform TargetPos, bool _Reset = false)
    {
        transform.position = StartPos.position;
        target = TargetPos;
        CanMove = true;
        CanReset = _Reset;
    }

    // Return money to pool
    void ResetData()
    {
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        target = null;
        CanReset = false;
        CanMove = false;
        gameObject.SetActive(false);
    }



}
