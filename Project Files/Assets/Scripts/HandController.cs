using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [Header("Movement Attributes")]
    public PlayerController playerController;
    public Camera           mainCamera;
    private Animator        anim;
    public float            retrieveSpeed;
    public float            moveSpeed;
    private short           dir;
    private short           lastDir;
    private bool            controlling;
    private bool            movable;

    [Header("Retrieve Attributes")]
    public GameObject       player;
    private SpriteRenderer  sprite;
    private Rigidbody2D     rigidBody;
    private BoxCollider2D   boxCollider;
    private Vector2         playerPosition;
    public float            retreiveRadius;
    private float           gravityScale;
    private float           mass;
    private bool            isRetrieving;
    private bool            retrieveComplete;

    [Header("Swiches")]
    public SwitchController switch_1;
    

    private void Start()
    {
        // Inactive in default
        gameObject.SetActive(false);

        // Movement Attributes
        anim                = GetComponent<Animator>();
        dir                 = 1;
        lastDir             = 1;
        controlling         = false;
        movable             = true;

        // Retreive Attributes
        sprite              = GetComponent<SpriteRenderer>();
        rigidBody           = GetComponent<Rigidbody2D>();
        boxCollider         = GetComponent<BoxCollider2D>();
        playerPosition      = player.transform.position;
        gravityScale        = rigidBody.gravityScale;
        mass                = rigidBody.mass;
        isRetrieving          = false;        
        retrieveComplete    = true;
    }

    private void Update()
    {
        if (!controlling)
        {
            Retrieve();
        }
        else
        {
            Move();
        }
        AnimationControl();
    }

    // This function is called by PlayerController
    public void Fire(float power)
    {
        // Set every property to default
        rigidBody.gravityScale  = gravityScale;
        rigidBody.mass          = mass;
        retrieveComplete        = false;
        Vector2 fireVector      = Vector2.zero;
        playerPosition          = player.transform.position;
        gameObject              .SetActive(true);

        // Fire vector is calculated.
        // Initial position is set in a little front of the player.
        switch (playerController.GetDir())
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
        rigidBody.AddForce(fireVector, ForceMode2D.Impulse);
    }

    public void StartRetrieve()
    {
        // Trigger 'Retrieve()'. Properties are changed so that the hand can move freely.
        sprite          .enabled = true;
        boxCollider     .isTrigger = true;
        rigidBody       .gravityScale = 0f;
        rigidBody       .mass = 0f;
        movable         = true;
        isRetrieving    = true;

        // Unplug from switch
        switch_1    .setPlugged(false);
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
                rigidBody           .gravityScale = gravityScale;
                rigidBody           .mass = mass;
                boxCollider         .isTrigger = false;
                gameObject          .SetActive(false);
                isRetrieving        = false;
                retrieveComplete    = true;
            }
        }
    }
    

    private void Move()
    {
        Vector3 cameraPosition          = gameObject.transform.position;
        cameraPosition.z                -= 10;
        cameraPosition.y                += 5;
        mainCamera.transform.position   = cameraPosition;

        Vector2 movement = new Vector2(Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime, 0);

        if (movement.x > 0)
        {
            dir     = 1;
            lastDir = 1;
        }
        else if (movement.x < 0)
        {
            dir     = -1;
            lastDir = -1;
        }
        else if (movement.x == 0)
        {
            dir     = 0;
        }

        if (movable) rigidBody.transform.Translate(movement);
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

    public bool GetControlling() { return controlling; }

    public void SetControlling(bool controlling) { this.controlling = controlling; }

    public void SetMovable(bool movable) { this.movable = movable; }

    public bool GetRetreiveComplete() { return retrieveComplete; }
}