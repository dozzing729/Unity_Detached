using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillController : MonoBehaviour
{
    public short        dir;
    public float        speed;
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // determine if objects on the treadmill are supposed to move
        if (collision.collider.CompareTag("Physical Object"))
        {
            GameObject objectOnTreadmill = collision.gameObject;
            Rigidbody2D rb = objectOnTreadmill.GetComponent<Rigidbody2D>();

            if (rb.velocity.magnitude < speed)
            {
                float horizontal = dir * speed * Time.deltaTime;
                float vertical = rb.velocity.y * Time.deltaTime;
                rb.velocity = new Vector3(horizontal, vertical, 0.0f);
            }
        }
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (!player.GetOnTreadMill())
            {
                player.SetOnTreadmill(true);
                player.SetTreadmillVelocity(dir * speed);
            }
        }
        if (collision.collider.CompareTag("Hand"))
        {
            ArmController hand = collision.gameObject.GetComponent<ArmController>();
            if (!hand.GetOnTreadMill())
            {
                hand.SetOnTreadmill(true);
                hand.SetTreadmillVelocity(dir * speed);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Physical Object"))
        {
            GameObject objectOffTreadmill = collision.gameObject;
            PhysicalObject physicalObject = objectOffTreadmill.GetComponent<PhysicalObject>();
            physicalObject.ApplyInertia(dir, speed);
        }
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.SetOnTreadmill(false);
        }
        if (collision.collider.CompareTag("Hand"))
        {
            ArmController hand = collision.gameObject.GetComponent<ArmController>();
            hand.SetOnTreadmill(false);
        }
    }
}
