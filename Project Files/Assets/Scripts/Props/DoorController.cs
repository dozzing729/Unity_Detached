using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject       unplugged_switch, plugged_green;
    public GameObject       handCheck;
    public GameObject       door;
    public HandController   leftHand, rightHand;
    public float            handCheckRectX, handCheckRectY;
    public int              waitToPlugOut;
    private int             counter;
    private bool            isLeftPlugged, isRightPlugged;
    private bool            isLeftHandAround, isRightHandAround;
    private bool            isPlugOutEnabled;

    void Start()
    {
        isLeftHandAround    = false;
        isRightHandAround   = false;
        isPlugOutEnabled    = false;
        counter             = waitToPlugOut;
        plugged_green   .SetActive(false);
    }

    void Update()
    {
        HandCheck();
        ActivateSwitch();
        SpriteControl();   
    }

    private void HandCheck()
    {
        isLeftHandAround  = Physics2D.OverlapBox(handCheck.transform.position, new Vector2(handCheckRectX, handCheckRectY), 0.0f, LayerMask.GetMask("Left Hand"));
        isRightHandAround = Physics2D.OverlapBox(handCheck.transform.position, new Vector2(handCheckRectX, handCheckRectY), 0.0f, LayerMask.GetMask("Right Hand"));
    }

    private void ActivateSwitch() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Plugging into switch
            if (isLeftHandAround && !isLeftPlugged && leftHand.getControlling())
            {
                isLeftPlugged = true;
                leftHand.SetStateAfterPlugIn();
                door.SetActive(false);
                return;
            }
            if (isRightHandAround && !isRightPlugged && rightHand.getControlling())
            {
                isRightPlugged = true;
                rightHand.SetStateAfterPlugIn();
                door.SetActive(false);
                return;
            }
        }
        if (Input.GetKey(KeyCode.Q) && isPlugOutEnabled)
        {
            if (isLeftPlugged && leftHand.getControlling())
            {
                if (counter++ > waitToPlugOut)
                {
                    isLeftPlugged = false;
                    leftHand.SetStateAfterPlugOut();
                    isPlugOutEnabled = false;
                }
            }
            if (isRightPlugged && rightHand.getControlling())
            {
                if (counter++ > waitToPlugOut)
                {
                    isRightPlugged = false;
                    rightHand.SetStateAfterPlugOut();
                    isPlugOutEnabled = false;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            counter = 0;
            if ((isLeftPlugged && leftHand.getControlling()) || (isRightPlugged && rightHand.getControlling()))
            {
                isPlugOutEnabled = true;
            }
        }
    }

    private void SpriteControl()
    {
        if (!isLeftHandAround)
        {
            isLeftPlugged = false;
        }
        if (!isRightHandAround)
        {
            isRightPlugged = false;
        }
        
        if (isLeftPlugged || isRightPlugged)
        {
            plugged_green.SetActive(true);
            unplugged_switch.SetActive(false);
            door.SetActive(false);
        }
        else
        {
            plugged_green.SetActive(false);
            unplugged_switch.SetActive(true);
            door.SetActive(true);
        }
    }

    public bool getLeftPlugged() { return isLeftPlugged; }

    public bool getRightPlugged() { return isRightPlugged; }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(handCheck.transform.position, new Vector3(handCheckRectX, handCheckRectY, 0)); }
}