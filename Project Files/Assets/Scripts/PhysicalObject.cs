using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalObject : MonoBehaviour
{
    private Boolean         isDestroyed;
    public GameObject       normalSprite;
    public GameObject       destroyedSprite;
    public new Rigidbody2D  rigidbody;
    
    protected void Start()
    {
        isDestroyed = false;
        rigidbody   = GetComponent<Rigidbody2D>();
        normalSprite    .SetActive(true);
        destroyedSprite .SetActive(false);
    }

    protected void Update()
    {
        SpriteControl();
    }

    private void SpriteControl()
    {
        if (isDestroyed)
        {
            normalSprite.SetActive(false);
            destroyedSprite.SetActive(true);
        }
        else
        {
            normalSprite.SetActive(true);
            destroyedSprite.SetActive(false);
        }
    }

    public void MoveObject(int dir, float speed)
    {
        Vector2 newPosition = transform.position;
        newPosition.x += dir * speed * Time.deltaTime;
        transform.localPosition = newPosition;
    }

    public void ApplyInertia(short dir, float speed)
    {
        float horizontal = dir * speed * Time.deltaTime;
        float vertical = rigidbody.velocity.y * Time.deltaTime;
        rigidbody.velocity = new Vector3(horizontal, vertical, 0.0f);
    }

    public Boolean GetDestroyed() { return isDestroyed; }
    public void SetDestroyed(Boolean isDestroyed) { this.isDestroyed = isDestroyed; }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            transform.parent = collision.transform;
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            transform.parent = null;
        }
    }
}
