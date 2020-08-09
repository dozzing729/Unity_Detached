using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadSwitchController : MonoBehaviour
{
    public GameObject       unplugged_switch, plugged_green, plugged_red;
    public GameObject       handCheck;
    public ArmController   leftHand, rightHand;
    public float            handCheckRectX, handCheckRectY;
    public int              waitToPlugOut;
    private int             counter;
    private bool            isLeftPlugged, isRightPlugged;
    private bool            isLeftHandAround, isRightHandAround;
    private bool            isActivated;
    private bool            isPlugOutEnabled;

    void Start()
    {
        isLeftHandAround    = false;
        isRightHandAround   = false;
        isActivated         = false;
        isPlugOutEnabled    = false;
        counter             = 0;
        plugged_green   .SetActive(false);
        plugged_red     .SetActive(false);
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

    private void ActivateSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // If this switch is plugged in by eiher right or left
            if (isLeftPlugged || isRightPlugged)
            {
                // Activating switch
                if (!isActivated)
                {
                    // Activate only when the plugged hand is being controlled
                    if ((isLeftPlugged    &&  leftHand.GetControl()) ||
                        (isRightPlugged   &&  rightHand.GetControl()))
                    {
                        isActivated = true;
                        Invoke("Deactivate", 0.5f);
                    }
                }
            }
            else
            {
                // Plugging into switch
                if (isLeftHandAround && !isLeftPlugged && leftHand.GetControl())
                {
                    isLeftPlugged = true;
                    leftHand.OnPlugIn();
                    return;
                }
                if (isRightHandAround && !isRightPlugged && rightHand.GetControl())
                {
                    isRightPlugged = true;
                    rightHand.OnPlugIn();
                    return;
                }
            }
        }
        if (Input.GetKey(KeyCode.Q) && isPlugOutEnabled)
        {
            if (isLeftPlugged && leftHand.GetControl())
            {
                if (counter++ > waitToPlugOut)
                {
                    isLeftPlugged = false;
                    leftHand.OnPlugOut();
                    isPlugOutEnabled = false;
                }
            }
            if (isRightPlugged && rightHand.GetControl())
            {
                if (counter++ > waitToPlugOut)
                {
                    isRightPlugged = false;
                    rightHand.OnPlugOut();
                    isPlugOutEnabled = false;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            counter = 0;
            if((isLeftPlugged && leftHand.GetControl()) || (isRightPlugged && rightHand.GetControl()))
            {
                isPlugOutEnabled = true;
            }
        }
    }

    // Swtich comes back to deactivated state, 0.5 seconds after it is activated
    private void Deactivate() { isActivated = false; }

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
            if (isActivated)
            {
                plugged_red.SetActive(false);
                plugged_green.SetActive(true);
                unplugged_switch.SetActive(false);
            }
            else
            {
                plugged_red.SetActive(true);
                plugged_green.SetActive(false);
                unplugged_switch.SetActive(false);
            }
        }
        else
        {
            plugged_red.SetActive(false);
            plugged_green.SetActive(false);
            unplugged_switch.SetActive(true);
        }
    }

    public bool getActivated() { return isActivated; }

    public bool getLeftPlugged() { return isLeftPlugged; }

    public bool getRightPlugged() { return isRightPlugged; }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(handCheck.transform.position, new Vector3(handCheckRectX, handCheckRectY, 0)); }
}
