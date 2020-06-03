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
    private bool            leftPlugged, rightPlugged;
    private bool            leftHandAround, rightHandAround;

    void Start()
    {
        leftHandAround  = false;
        rightHandAround = false;
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
        leftHandAround  = Physics2D.OverlapBox(handCheck.transform.position, new Vector2(handCheckRectX, handCheckRectY), 0.0f, LayerMask.GetMask("Left Hand"));
        rightHandAround = Physics2D.OverlapBox(handCheck.transform.position, new Vector2(handCheckRectX, handCheckRectY), 0.0f, LayerMask.GetMask("Right Hand"));
    }

    private void ActivateSwitch() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Plugging into switch
            if (leftHandAround && !leftPlugged && leftHand.getControlling())
            {
                leftPlugged = true;
                leftHand.SetStateAfterPlugIn();
                door.SetActive(false);
                return;
            }
            if (rightHandAround && !rightPlugged && rightHand.getControlling())
            {
                rightPlugged = true;
                rightHand.SetStateAfterPlugIn();
                door.SetActive(false);
                return;
            }
        }
    }

    private void SpriteControl()
    {
        if (leftPlugged && !leftHandAround)
        {
            leftPlugged = false;
            door.SetActive(true);
        }    
        if (rightPlugged && !rightHandAround) 
        {
            rightPlugged = false;
            door.SetActive(true);
        }

        if (leftPlugged || rightPlugged)
        {
            plugged_green.SetActive(true);
            unplugged_switch.SetActive(false);
        }
        else
        {
            plugged_green.SetActive(false);
            unplugged_switch.SetActive(true);
        }
    }

    public bool getLeftPlugged() { return leftPlugged; }

    public bool getRightPlugged() { return rightPlugged; }

    public void setPlugged(bool plugged)
    {
        this.leftPlugged = plugged;
        this.rightPlugged = plugged;
    }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(handCheck.transform.position, new Vector3(handCheckRectX, handCheckRectY, 0)); }
}