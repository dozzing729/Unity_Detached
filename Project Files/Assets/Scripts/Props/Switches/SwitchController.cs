using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public GameObject       target;
    public GameObject       unpluggedSprite, pluggedSpriteRed, pluggedSpriteGreen;
    public ArmController    leftArm, rightArm;
    public PlayerController player;
    public int              waitToPlugOut;
    protected int           counter;
    protected bool          isLeftArmAround, isRightArmAround;
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
        HandCheck();
        PlugCheck();
        ActivateSwitch();
        SpriteControl();
    }

    virtual protected void HandCheck()
    {
        isLeftArmAround  = Physics2D.OverlapBox(transform.position, new Vector3(2.3f, 3.2f, 0), 0.0f, LayerMask.GetMask("Left Arm"));
        isRightArmAround = Physics2D.OverlapBox(transform.position, new Vector3(2.3f, 3.2f, 0), 0.0f, LayerMask.GetMask("Right Arm"));
    }

    virtual protected void PlugCheck()
    {
        if (isLeftPlugged && !isLeftArmAround)
        {
            isLeftPlugged = false;
            OnDeactivation();
        }
        if (isRightPlugged && !isRightArmAround)
        {
            isRightPlugged = false;
            OnDeactivation();
        }
    }

    virtual protected void ActivateSwitch()
    {
        if (!isLeftPlugged && !isRightPlugged)
        {
            // Plug in
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Left hand ready to plug in
                // Right hand ready to plug out
                if (isLeftArmAround && leftArm.GetControl() ||
                    isRightArmAround && rightArm.GetControl())
                {
                    OnActivation();
                    return;
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && player.GetControlling())
            {
                OnDeactivation();
                isLeftArmAround     = false;
                isRightArmAround    = false;
                isPlugOutEnabled    = true;
            }
        }
        // Start plugging out
        if (Input.GetKey(KeyCode.Q) && isPlugOutEnabled)
        {
            // If left hand must come out
            if (isLeftPlugged && leftArm.GetControl())
            {
                if (counter++ > waitToPlugOut)
                {
                    OnDeactivation();
                }
            }
            // If right hand must come out
            if (isRightPlugged && rightArm.GetControl())
            {
                if (counter++ > waitToPlugOut)
                {
                    OnDeactivation();
                }
            }
        }
        // Plug out
        if (Input.GetKeyUp(KeyCode.Q))
        {
            counter = 0;
            if ((isLeftPlugged && leftArm.GetControl()) ||
                (isRightPlugged && rightArm.GetControl()))
            {
                isPlugOutEnabled = true;
            }
        }
    }

    virtual protected void OnActivation() {
        if (isLeftArmAround)
        {
            isLeftPlugged = true;
            leftArm.OnPlugIn();
            return;
        }
        if (isRightArmAround)
        {
            isRightPlugged = true;
            rightArm.OnPlugIn();
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
            leftArm.OnPlugOut();
            return;
        }
        if (isRightPlugged)
        {
            isRightPlugged      = false;
            isPlugOutEnabled    = false;
            rightArm.OnPlugOut();
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

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Hand"))
    //    {
    //        // 11: Left Hand
    //        if (collision.gameObject.layer == 11)
    //        {
    //            isLeftArmAround = true;
    //        }
    //        // 12: Right Hand
    //        if (collision.gameObject.layer == 12)
    //        {
    //            isRightArmAround = true;
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Hand"))
    //    {
    //        // 11: Left Hand
    //        if (collision.gameObject.layer == 11)
    //        {
    //            isLeftArmAround = false;
    //            if (isLeftPlugged)
    //            {
    //                OnDeactivation();
    //            }
    //        }
    //        // 12: Right Hand
    //        if (collision.gameObject.layer == 12)
    //        {
    //            isRightArmAround = false;
    //            if (isRightPlugged)
    //            {
    //                OnDeactivation();
    //            }
    //        }
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(2.3f, 3.2f, 0));
    }
}
