using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherController : MonoBehaviour
{
    public float speed;
    public float accel;
    public float bound;
    public float checkRectX, checkRectY;

    public GameObject crushCheck;
    public GameObject player;
    public GameObject leftHand;
    public GameObject rightHand;

    private Vector2 crusherPosition;
    private Vector2 origin;

    private bool isActivated;
    private bool isGoingUp;
    private bool isPlayerCrushed;
    private bool isLeftHandCrushed;
    private bool isRightHandCrushed;
    private bool isObjectCrushed;


    private void Start()
    {
        crusherPosition = transform.position;
        origin          = crusherPosition;
        isGoingUp       = false;
        isActivated     = true;
    }

    private void Update()
    {
        TriggerCheck();
        OperateCrusher();
    }

    private void TriggerCheck()
    {
        isPlayerCrushed     = Physics2D.OverlapBox(crushCheck.transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Player"));
        isLeftHandCrushed   = Physics2D.OverlapBox(crushCheck.transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Left Hand"));
        isRightHandCrushed  = Physics2D.OverlapBox(crushCheck.transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Right Hand"));
        isObjectCrushed     = Physics2D.OverlapBox(crushCheck.transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Physical Object"));
    }

    private void OperateCrusher()
    {
        crusherPosition = transform.position;

        if (isActivated)
        {
            if (isGoingUp)
            {
                if (crusherPosition.y < origin.y + bound)
                {
                    transform.Translate(new Vector2(0, speed) * Time.deltaTime);
                }
                else
                {
                    isGoingUp = false;
                }
            }
            else
            {
                if (crusherPosition.y > origin.y)
                {
                    transform.Translate(new Vector2(0, -1 * Mathf.Pow(accel ,speed * Time.deltaTime)));
                }
                else
                {
                    isGoingUp = true;
                }
            }
        }
    }

    private void crushPlayer()
    {
        if (isPlayerCrushed)
        {
            // player crushed
        }
        else if (isLeftHandCrushed)
        {
            // left hand crushed
        }
        else if (isRightHandCrushed)
        {
            // right hand crushed
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isObjectCrushed)
        {
            if (collision.collider.CompareTag("Physical Object"))
            {
                PhysicalObject crushedObject = collision.gameObject.GetComponent<PhysicalObject>();
                if (!crushedObject.GetDestroyed())
                {
                    crushedObject.SetDestroyed(true);
                }
            }
        }
    }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(crushCheck.transform.position, new Vector2(checkRectX, checkRectY)); }
}