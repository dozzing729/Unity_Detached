using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public GameObject       target;
    public GameObject       unpluggedSprite, pluggedSpriteRed, pluggedSpriteGreen;
    public HandController   leftHand, rightHand;
    public PlayerController player;
    public int              waitToPlugOut;
    protected int           counter;
    protected bool          isLeftHandAround, isRightHandAround;
    protected bool          isLeftPlugged, isRightPlugged;
    protected bool          isPlugOutEnabled;

    virtual protected void Start()
    {
        unpluggedSprite     .SetActive(true);
        pluggedSpriteGreen  .SetActive(false);
        pluggedSpriteRed    .SetActive(false);
        isPlugOutEnabled    = false;
        counter             = waitToPlugOut;
    }

    virtual protected void Update()
    {
        ActivateSwitch();
        SpriteControl();
    }

    virtual protected void ActivateSwitch()
    {
        // Plug in
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Left hand ready to plug in
            if (isLeftHandAround && leftHand.GetControl())
            {
                OnActivation();
                return;
            }
            // Right hand ready to plug out
            if (isRightHandAround && rightHand.GetControl())
            {
                OnActivation();
                return;
            }
        }
        // Start plugging out
        if (Input.GetKey(KeyCode.Q) && isPlugOutEnabled)
        {
            // If left hand must come out
            if (isLeftPlugged && leftHand.GetControl())
            {
                if (counter++ > waitToPlugOut)
                {
                    OnDeactivation();
                }
            }
            // If right hand must come out
            if (isRightPlugged && rightHand.GetControl())
            {
                if (counter++ > waitToPlugOut)
                {
                    OnDeactivation();
                }
            }           
        }
        if (Input.GetKeyDown(KeyCode.R) && player.getControlling())
        {
            OnDeactivation();
            isLeftHandAround    = false;
            isRightHandAround   = false;
            isPlugOutEnabled    = true;
        }
        // Plug out
        if (Input.GetKeyUp(KeyCode.Q))
        {
            counter = 0;
            if ((isLeftPlugged && leftHand.GetControl()) || 
                (isRightPlugged && rightHand.GetControl()))
            {
                isPlugOutEnabled = true;
            }
        }
    }

    virtual protected void OnActivation() {
        if (isLeftHandAround)
        {
            isLeftPlugged = true;
            leftHand.OnPlugIn();
            return;
        }
        if (isRightHandAround)
        {
            isRightPlugged = true;
            rightHand.OnPlugIn();
            return;
        }
    }
    virtual protected void OnDeactivation() 
    {
        counter = 0;

        if (isLeftPlugged)
        {
            isLeftPlugged       = false;
            isPlugOutEnabled    = false;
            leftHand.OnPlugOut();
            return;
        }
        if (isRightPlugged)
        {
            isRightPlugged      = false;
            isPlugOutEnabled    = false;
            rightHand.OnPlugOut();
            return;
        }
    }

    virtual protected void SpriteControl()
    {
        if (isLeftPlugged || isRightPlugged)
        {
            pluggedSpriteGreen  .SetActive(true);
            unpluggedSprite     .SetActive(false);
        }
        else
        {
            pluggedSpriteGreen  .SetActive(false);
            unpluggedSprite     .SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Hand"))
        {
            // 11: Left Hand, 12: Right Hand
            if (collision.gameObject.layer == 11)
            {
                isLeftHandAround = true;
            }
            if (collision.gameObject.layer == 12)
            {
                isRightHandAround = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Hand"))
        {
            // 11: Left Hand, 12: Right Hand
            if (collision.gameObject.layer == 11)
            {
                OnDeactivation();
                isLeftHandAround = false;
            }
            if (collision.gameObject.layer == 12)
            {
                OnDeactivation();
                isRightHandAround = false;
            }
        }
    }
}
