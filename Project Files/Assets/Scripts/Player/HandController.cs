using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class HandController : PhysicalObject
{
    [Header("Movement Attributes")]
    public GameObject           player;
    private PlayerController    playerController;
    public Camera               mainCamera;
    private Animator            anim;
    public float                retrieveSpeed;
    public float                moveSpeed;
    public float                checkRectX;
    public float                checkRectY;
    public float                treadmillVelocity;
    private short               dir;
    private short               lastDir;
    private bool                isFireComplete;
    private bool                isControlling;
    private bool                isMovable;
    private bool                isOnTreadmill;

    [Header("Retrieve Attributes")]
    private SpriteRenderer  sprite;
    private BoxCollider2D   boxCollider;
    private Vector2         playerPosition;
    public float            retreiveRadius;
    private float           gravityScale;
    private float           mass;
    private bool            isRetrieving;
    private bool            isRetrieveComplete;

    private new void Start()
    {
        // Inactive in default
        gameObject.SetActive(false);

        // Movement Attributes
        playerController    = player.GetComponent<PlayerController>();
        anim                = GetComponent<Animator>();
        dir                 = 1;
        lastDir             = 1;
        treadmillVelocity   = 0f;
        isFireComplete      = false;
        isControlling       = false;
        isMovable           = true;
        isOnTreadmill       = false;

        // Retreive Attributes
        sprite              = GetComponent<SpriteRenderer>();
        boxCollider         = GetComponent<BoxCollider2D>();
        playerPosition      = player.transform.position;
        gravityScale        = rigidbody.gravityScale;
        mass                = rigidbody.mass;
        isRetrieving        = false;        
        isRetrieveComplete  = true;
    }

    private new void Update()
    {
        GroundCheck();
        if (!isControlling)
        {
            Retrieve();
        }
        Move();
        AnimationControl();
    }

    // This function is called by PlayerController
    public void Fire(float power)
    {
        // Set every property to default
        rigidbody.gravityScale  = gravityScale;
        rigidbody.mass          = mass;
        isRetrieveComplete      = false;
        Vector2 fireVector      = Vector2.zero;
        playerPosition          = player.transform.position;
        gameObject              .SetActive(true);

        // Fire vector is calculated.
        // Initial position is set to a little front of the player.
        switch (playerController.getDir())
        {
            case 1:
                playerPosition.x                += 2;
                gameObject.transform.position   = playerPosition;
                fireVector                      = new Vector2(5 + power, 15 + power);
                break;
            case -1:
                playerPosition.x                -= 2;
                gameObject.transform.position   = playerPosition;
                fireVector                      = new Vector2(-5 - power, 15 + power);
                break;
        }

        // Fire
        rigidbody.AddForce(fireVector, ForceMode2D.Impulse);
    }

    public void StartRetrieve()
    {
        // Trigger 'Retrieve()'. Properties are changed so that the hand can move freely.
        sprite.enabled          = true;
        boxCollider.isTrigger   = true;
        rigidbody.gravityScale  = 0f;
        rigidbody.mass          = 0f;
        isMovable               = false;
        isRetrieving            = true;
    }

    private void Retrieve()
    {
        // Player's position is the target position.
        playerPosition = player.transform.position;

        if (isRetrieving)
        {
            Vector2 temp        = new Vector2(transform.position.x, transform.position.y);
            Vector2 diff        = playerPosition - temp;
            Vector2 direction   = diff.normalized;
            Vector2 movement    = direction * retrieveSpeed * Time.deltaTime;

            // Move towards the player
            transform.Translate(movement, Space.World);

            // Retrieve complete
            if (diff.magnitude < retreiveRadius)
            {
                rigidbody           .gravityScale = gravityScale;
                rigidbody           .mass = mass;
                boxCollider         .isTrigger = false;
                gameObject          .SetActive(false);
                isFireComplete      = false;
                isRetrieving        = false;
                isRetrieveComplete  = true;
            }
        }
    }

    private void Move()
    {
        // User input calculated
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float vertical = rigidbody.velocity.y;

        if (isControlling)
        {
            // Camera position setting
            Vector3 cameraPosition = transform.position;
            cameraPosition.z = -100;
            cameraPosition.y += 7;
            mainCamera.transform.position = cameraPosition;

            // Camera size setting
            //mainCamera.orthographicSize = 85 + 70 * cameraPosition.y / 73;
            //if (mainCamera.orthographicSize > 25) mainCamera.orthographicSize = 25; 
            //if (mainCamera.orthographicSize < 13) mainCamera.orthographicSize = 14;

            // Direction setting
            if (horizontal > 0)
            {
                dir = 1;
                lastDir = 1;
            }
            else if (horizontal < 0)
            {
                dir = -1;
                lastDir = -1;
            }
            else if (horizontal == 0)
            {
                dir = 0;
            }

            // Move
            if (isMovable)
            {
                if (isOnTreadmill)
                {
                    horizontal += treadmillVelocity * Time.deltaTime;
                    rigidbody.velocity = new Vector3(horizontal, vertical, 0.0f);
                }
                else
                {
                    if (horizontal != 0)
                    {
                        rigidbody.velocity = new Vector3(horizontal, vertical, 0.0f);
                    }
                }
            }
        }
        else
        {
            if (isOnTreadmill)
            {
                horizontal = treadmillVelocity * Time.deltaTime;
                rigidbody.velocity = new Vector3(horizontal, vertical, 0.0f);
            }
        }
    }

    private void GroundCheck()
     {
        // If the hand didn't touch ground after it was initially fired, it can't move
        if (!isFireComplete) {
            isMovable = false;
            isFireComplete = Physics2D.OverlapBox(transform.position, new Vector2(checkRectX, checkRectY), 0.0f, LayerMask.GetMask("Ground"));
            if (isFireComplete) 
            {
                isMovable = true;
            }
        }
    }

    private void AnimationControl()
    {
        switch(dir) {
            case 1:
                anim.Play("move_right");
                break;
            case -1:
                anim.Play("move_left");
                break;
            case 0:
                if (lastDir == 1)
                    anim.Play("idle_right");
                if (lastDir == -1)
                    anim.Play("idle_left");
                break;
        }
    }

    public void OnPlugIn()
    {
        sprite.enabled          = false;
        boxCollider.isTrigger   = true;
        rigidbody.gravityScale  = 0f;
        rigidbody.mass          = 0f;
        isMovable               = false;
        rigidbody.velocity      = Vector2.zero;
    }

    public void OnPlugOut()
    {
        sprite.enabled          = true;
        boxCollider.isTrigger   = false;
        rigidbody.gravityScale  = gravityScale;
        rigidbody.mass          = mass;
        isMovable               = true;
    }


    public void SetTreadmillVelocity(float treadmillVelocity)
    { this.treadmillVelocity = treadmillVelocity; }

    public bool GetOnTreadMill()
    { return isOnTreadmill; }

    public void SetOnTreadmill(bool isOnTreadmill)
    { this.isOnTreadmill = isOnTreadmill; }

    public bool GetControl() 
    { return isControlling; }

    public void SetControl(bool isControlling) 
    { this.isControlling = isControlling; }

    public void SetMovable(bool isMovable) 
    { this.isMovable = isMovable; }

    public bool GetRetrieveComplete() 
    { return isRetrieveComplete; }

    private void OnDrawGizmos() 
    { Gizmos.DrawWireCube(transform.position, new Vector3(checkRectX, checkRectY, 0)); }
}