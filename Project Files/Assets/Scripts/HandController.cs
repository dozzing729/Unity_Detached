using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [Header("Movement Properties")]
    public PlayerController playerController;
    public Camera camera;
    private Animator anim;
    public float retrieveSpeed;
    public float moveSpeed;
    private short dir;
    private short lastDir;

    [Header("Retrieve Properties")]
    public GameObject player;
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider;
    private Vector2 playerPosition;
    public float retreiveRadius;
    private float gravityScale;
    private float mass;
    private bool retrieving;
    private bool retrieveComplete;

    public SwitchController switch_1;
    private bool controlling;
    private bool movable;


    private void Start()
    {
        gameObject.SetActive(false);

        anim = GetComponent<Animator>();
        dir = 1;
        lastDir = 1;

        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerPosition = player.transform.position;
        gravityScale = rigidbody.gravityScale;
        mass = rigidbody.mass;
        retrieving = false;

        controlling = false;
        movable = true;
        retrieveComplete = true;
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

    public void Fire(float power)
    {
        // Set every property to default
        rigidbody.gravityScale = gravityScale;
        rigidbody.mass = mass;
        retrieveComplete = false;
        Vector2 fireVector = Vector2.zero;

        playerPosition = player.transform.position;
        gameObject.SetActive(true);

        // Fire. Initial position set right in front of the player's face.
        switch (playerController.getDir())
        {
            case 1:
                playerPosition.x += 1;
                gameObject.transform.position = playerPosition;
                fireVector = new Vector2(5 + power, 15 + power);
                break;
            case -1:
                playerPosition.x -= 1;
                gameObject.transform.position = playerPosition;
                fireVector = new Vector2(-5 - power, 15 + power);
                break;
        }

        rigidbody.AddForce(fireVector, ForceMode2D.Impulse);
    }

    public void StartRetrieve()
    {
        // Trigger 'Retrieve()'. Properties are changed so that the hand can move freely.
        GetComponent<SpriteRenderer>().enabled = true;
        movable = true;
        retrieving = true;
        boxCollider.isTrigger = true;
        rigidbody.gravityScale = 0f;
        rigidbody.mass = 0f;

        // Unplug from switch
        switch_1.setPlugged(false);
    }

    private void Retrieve()
    {
        playerPosition = player.transform.position;

        if (retrieving)
        {
            Vector2 temp = new Vector2(transform.position.x, transform.position.y);
            Vector2 diff = playerPosition - temp;
            Vector2 direction = diff.normalized;
            Vector2 movement = direction * retrieveSpeed * Time.deltaTime;

            // Move towards the player
            transform.Translate(movement, Space.World);

            // Retrieve complete
            if (diff.magnitude < retreiveRadius)
            {
                rigidbody.gravityScale = gravityScale;
                rigidbody.mass = mass;
                boxCollider.isTrigger = false;
                gameObject.SetActive(false);
                retrieving = false;
                retrieveComplete = true;
            }
        }
    }
    

    private void Move()
    {
        Vector3 cameraPosition = gameObject.transform.position;
        cameraPosition.z -= 10;
        camera.transform.position = cameraPosition;

        Vector2 movement = new Vector2(Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime, 0);

        if (movement.x > 0)
        {
            dir = 1;
            lastDir = 1;
        }
        else if (movement.x < 0)
        {
            dir = -1;
            lastDir = -1;
        }
        else if (movement.x == 0)
        {
            dir = 0;
        }

        if (movable) rigidbody.transform.Translate(movement);
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
                if (lastDir == 1) anim.Play("idle_right");
                if (lastDir == -1) anim.Play("idle_left");
                break;
        }
    }

    public bool getControlling() { return controlling; }

    public void setControlling(bool controlling) { this.controlling = controlling; }

    public void setMovable(bool movable) { this.movable = movable; }

    public bool getRetreiveComplete() { return retrieveComplete; }
}
