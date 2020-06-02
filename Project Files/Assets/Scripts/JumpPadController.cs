using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadController : MonoBehaviour
{
    private Rigidbody2D             playerRigidbody;
    private Rigidbody2D             leftHandRigidbody;
    private Rigidbody2D             rightHandRigidbody;
    public GameObject               playerCheck;
    public GameObject               player;
    public GameObject               leftHand;
    public GameObject               rightHand;
    public GameObject               jumpPad;
    public JumpPadSwitchController  switchController;
    private Vector2                 jumpPadPositionBefore;
    private Vector2                 jumpPadPositionAfter;
    public float                    playerCheckRadius;
    public float                    jumpPower;
    public float                    checkRectX, checkRectY;
    private bool                    isPlayerOnPad;
    private bool                    isLeftHandOnPad;
    private bool                    isRightHandOnPad;
    private bool                    activated;

    private void Start()
    {
        playerRigidbody         = player.gameObject.GetComponent<Rigidbody2D>();
        leftHandRigidbody       = leftHand.gameObject.GetComponent<Rigidbody2D>();
        rightHandRigidbody      = rightHand.gameObject.GetComponent<Rigidbody2D>();
        jumpPadPositionBefore   = jumpPad.transform.position;
        jumpPadPositionAfter    = jumpPadPositionBefore;
        jumpPadPositionAfter.y  += 1;
        activated               = false;
    }

    private void Update()
    {
        PlayerCheck();
        ActivateJumpPad();
    }

    private void PlayerCheck()
    {
        isPlayerOnPad       = Physics2D.OverlapBox(playerCheck.transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Player"));
        isLeftHandOnPad     = Physics2D.OverlapBox(playerCheck.transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Left Hand"));
        isRightHandOnPad    = Physics2D.OverlapBox(playerCheck.transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Right Hand"));
    }

    private void ActivateJumpPad()
    {
        if (switchController.getActivated() && !activated)
        {
            if (isPlayerOnPad) 
            {
                playerRigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }
            if (isLeftHandOnPad)
            {
                leftHandRigidbody.AddForce(Vector2.up * jumpPower * 1.3f, ForceMode2D.Impulse);
            }
            if (isRightHandOnPad)
            {
                rightHandRigidbody.AddForce(Vector2.up * jumpPower * 1.3f, ForceMode2D.Impulse);
            }
            
            activated = true;
            jumpPad.gameObject.transform.position = jumpPadPositionAfter;
            Invoke("Deactivate", 0.5f);
        }
    }

    private void Deactivate() { 
        activated = false;
        jumpPad.gameObject.transform.position = jumpPadPositionBefore;
    }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(playerCheck.transform.position, new Vector3(checkRectX, checkRectY, 0)); }
}
