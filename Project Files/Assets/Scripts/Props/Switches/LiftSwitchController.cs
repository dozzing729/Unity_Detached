using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftSwitchController : SwitchController
{
    public GameObject   maxHeightCheck;
    private Vector3     targetPosition;
    public float        speed;
    private float       maxHeight;
    private float       minHeight;

    protected override void Start()
    {
        base.Start();
        targetPosition  = target.transform.position;
        maxHeight       = maxHeightCheck.transform.localPosition.y;
        minHeight       = targetPosition.y;
    }

    protected override void Update()
    {
        base.Update();
        Operate();
    }
    private void Operate()
    {
        targetPosition = target.transform.position;

        if (isLeftPlugged || isRightPlugged)
        {
            MoveUp();
        }
        else
        {
            Debug.Log("GO DOWN");
            MoveDown();
        }
    }

    private void MoveUp()
    {
        if (targetPosition.y <= maxHeight)
        {
            Move(1);
        }
    }

    private void MoveDown()
    {
        if (targetPosition.y >= minHeight)
        {
            Debug.Log("GOING DOWN");
            Move(-2);
        }
    }

    private void Move(short dir)
    {
        target.transform.Translate(new Vector3(0.0f, speed * dir, 0.0f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(maxHeightCheck.transform.position, 1f);
    }
}
