using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillController : MonoBehaviour
{
    public float            moveSpeed;
    public float            inertia;
    private float           tmpInertia;
    public PlayerController player;
    private bool            isGrounded;
    private bool            isLeftGround;
    public short            direction;

    void Start()
    {
        isLeftGround    = true;
        tmpInertia      = inertia;
    }

    void Update()
    {
        Move();
    }

    // add movement vector to the player's position
    private void Move()
    {
        if (isGrounded)
        {
            Vector2 movement = new Vector2(direction, 0) * moveSpeed;
            player.makeMove(movement);
        }
        else
        {
            if (!isLeftGround)
            {
                inertia -= 0.004f;
                if (inertia < 0)
                {
                    isLeftGround    = true;
                    inertia         = tmpInertia;
                    return;
                }
                Vector2 movement = new Vector2(direction, 0) * inertia;
                player.makeMove(movement);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            isGrounded      = true;
            isLeftGround    = false;
            inertia         = tmpInertia;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            isGrounded = false;
        }
    }
}
