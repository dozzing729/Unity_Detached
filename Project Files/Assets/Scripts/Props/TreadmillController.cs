using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillController : MonoBehaviour
{
    private Transform   objectOnTreadmill;
    public short        dir;
    public float        speed;

    private void Move()
    {
        //Vector2 moveVector = new Vector2(dir, 0) * speed * Time.fixedDeltaTime;
        Vector2 newPosition = objectOnTreadmill.position;
        newPosition.x += dir * speed * Time.deltaTime;
        objectOnTreadmill.localPosition = newPosition;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // determine if objects on the treadmill are supposed to move
        if (collision.collider.CompareTag("Player") ||
            collision.collider.CompareTag("Hand") ||
            collision.collider.CompareTag("Physical Object"))
        {
            // move the objects
            objectOnTreadmill = collision.collider.transform;
            Move();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") ||
            collision.collider.CompareTag("Hand") ||
            collision.collider.CompareTag("Physical Object"))
        {
            collision.gameObject.GetComponent<PhysicalObject>().ApplyInertia(dir, speed);
        }
    }
}
