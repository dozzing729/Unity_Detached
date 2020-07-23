using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    public Boolean showValues;
    private new Rigidbody2D rigidbody;
    private new Transform transform;
    private Vector3 position;
    private Vector3 velocity;
    private float speed;
    private short direction;
    
    void Start()
    {
        rigidbody   = GetComponent<Rigidbody2D>();
        transform   = GetComponent<Transform>();
    }

    void Update()
    {
        position    = transform.position;
        velocity    = rigidbody.velocity;
        speed       = velocity.magnitude;

        if (velocity.x > 0)     direction = 1;
        if (velocity.x == 0)    direction = 0;
        if (velocity.x < 0)     direction = -1;

        ShowValues();
    }

    public void ApplyInertia(short dir, float inertia)
    {
        Vector2 inertiaVector = new Vector2(dir, 0) * inertia * Time.deltaTime * 30;
        rigidbody.AddForce(inertiaVector, ForceMode2D.Impulse);
    }

    public void ShowValues()
    {
        if (showValues)
        {
            //Debug.Log("Position: " + position);
            //Debug.Log("Velocity: " + velocity);
            //Debug.Log("Speed: " + speed);
            //Debug.Log("Direction: " + direction);
        }
    }

    public Rigidbody2D GetRigidbody() { return rigidbody; }
    public Transform GetTransform() { return transform; }
    public Vector3 GetPosition() { return position; }
    public Vector3 GetVelocity() { return velocity; }
    public float GetSpeed() { return speed; }
    public short GetDirection() { return direction; }
}
