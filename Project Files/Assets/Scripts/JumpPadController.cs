using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadController : MonoBehaviour
{
    public Rigidbody2D      playerRigidbody;
    public GameObject       playerCheck;
    public GameObject       jumpPad;
    public SwitchController switchController;
    private Vector2         jumpPadPositionBefore;
    private Vector2         jumpPadPositionAfter;
    public float            playerCheckRadius;
    public float            jumpPower;
    public float            playerCheckRectX, playerCheckRectY;
    private bool            playerOnPad;
    private bool            activated;

    private void Start()
    {
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
        playerOnPad = Physics2D.OverlapBox(playerCheck.transform.position, new Vector2(playerCheckRectX, playerCheckRectY), 0.0f, LayerMask.GetMask("Player"));
    }

    private void ActivateJumpPad()
    {
        if (switchController.getActivated() && playerOnPad && !activated)
        {
            playerRigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            activated = true;
            jumpPad.gameObject.transform.position = jumpPadPositionAfter;
            Invoke("Deactivate", 0.5f);
        }
    }

    private void Deactivate() { 
        activated = false;
        jumpPad.gameObject.transform.position = jumpPadPositionBefore;
    }

    private void OnDrawGizmos() { Gizmos.DrawWireCube(playerCheck.transform.position, new Vector3(playerCheckRectX, playerCheckRectY, 0)); }
}
