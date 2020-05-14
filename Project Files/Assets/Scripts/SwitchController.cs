using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public GameObject       unplugged_switch, plugged_green, plugged_red;
    public GameObject       handCheck;
    public HandController   leftHand, rightHand;
    public float            handCheckRectX, handCheckRectY;
    private bool            leftPlugged, rightPlugged;
    private bool            leftHandAround, rightHandAround;
    private bool            activated;

    void Start()
    {
        leftHandAround  = false;
        rightHandAround = false;
        activated       = false;
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
        leftHandAround  = Physics2D.OverlapBox(handCheck.transform.position, new Vector2(handCheckRectX, handCheckRectY), 0.0f, LayerMask.GetMask("Left Hand"));
        rightHandAround = Physics2D.OverlapBox(handCheck.transform.position, new Vector2(handCheckRectX, handCheckRectY), 0.0f, LayerMask.GetMask("Right Hand"));
    }

    private void ActivateSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // If this switch is plugged in by eiher right or left
            if (leftPlugged || rightPlugged)
            {
                // Activating switch
                if (!activated)
                {
                    // Activate only when the plugged hand is being controlled
                    if ((leftPlugged    &&  leftHand.GetControlling()) ||
                        (rightPlugged   &&  rightHand.GetControlling()))
                    {
                        activated = true;
                        Invoke("Deactivate", 0.5f);
                    }
                }
            }
            else
            {
                // Plugging into switch
                if (leftHandAround && !leftPlugged && leftHand.GetControlling())
                {
                    leftPlugged = true;
                    leftHand.SetStateAfterPlugIn();
                    return;
                }
                if (rightHandAround && !rightPlugged && rightHand.GetControlling())
                {
                    rightPlugged = true;
                    rightHand.SetStateAfterPlugIn();
                    return;
                }
            }
        }
    }

    // Swtich comes back to deactivated state, 0.5 seconds after it is activated
    private void Deactivate() { activated = false; }

    private void SpriteControl()
    {
        if (leftPlugged || rightPlugged)
        {
            if (activated)
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

    public bool getActivated() { return activated; }

    public bool getLeftPlugged() { return leftPlugged; }

    public bool getRightPlugged() { return rightPlugged; }

    public void setPlugged(bool plugged)
    {
        this.leftPlugged = plugged;
        this.rightPlugged = plugged;
    }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(handCheck.transform.position, new Vector3(handCheckRectX, handCheckRectY, 0)); }
}
