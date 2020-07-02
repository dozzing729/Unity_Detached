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
    private bool            isLeftPlugged, isRightPlugged;
    private bool            isLeftHandAround, isRightHandAround;

    void Start()
    {
        isLeftHandAround  = false;
        isRightHandAround = false;
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
    }

    private void SpriteControl()
    {
        if (isLeftPlugged && !isLeftHandAround)
        {
            isLeftPlugged = false;
            door.SetActive(true);
        }    
        if (isRightPlugged && !isRightHandAround) 
        {
            isRightPlugged = false;
            door.SetActive(true);
        }

        if (isLeftPlugged || isRightPlugged)
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

    public bool getLeftPlugged() { return isLeftPlugged; }

    public bool getRightPlugged() { return isRightPlugged; }

    public void setPlugged(bool plugged)
    {
        this.isLeftPlugged = plugged;
        this.isRightPlugged = plugged;
    }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(handCheck.transform.position, new Vector3(handCheckRectX, handCheckRectY, 0)); }
}