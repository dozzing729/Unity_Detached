using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public GameObject unplugged_switch, plugged_green, plugged_red;
    public GameObject handCheck;
    public HandController leftHand, rightHand;
    public float handCheckRadius;
    private bool leftPlugged, rightPlugged;
    private bool leftHandAround, rightHandAround;
    private bool activated;

    void Start()
    {
        leftHandAround  = rightHandAround = false;
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
        leftHandAround  = Physics2D.OverlapCircle(handCheck.transform.position, handCheckRadius, LayerMask.GetMask("Left Hand"));
        rightHandAround = Physics2D.OverlapCircle(handCheck.transform.position, handCheckRadius, LayerMask.GetMask("Right Hand"));
    }

    private void ActivateSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (leftPlugged || rightPlugged)
            {
                // Activating switch
                if (!activated)
                {
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
                if (leftHandAround && !leftPlugged)
                {
                    leftPlugged = true;
                    leftHand.GetComponent<SpriteRenderer>().enabled = false;
                    leftHand.SetMovable(false);
                    return;
                }
                if (rightHandAround && !rightPlugged)
                {
                    rightPlugged = true;
                    rightHand.GetComponent<SpriteRenderer>().enabled = false;
                    rightHand.SetMovable(false);
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

    private void OnDrawGizmos() { Gizmos.DrawWireSphere(handCheck.transform.position, handCheckRadius); }
}
