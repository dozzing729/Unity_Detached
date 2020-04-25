using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Attributes")]
    public Camera camera;
    public float moveSpeed;
    public float jumpHeight;
    private bool isGrounded;
    private bool jumped;
    private bool blocked;
    private Rigidbody2D rigidBody;
    private bool movable;
    private bool controlling;
    private bool controlEnabled;

    [Header("Shoot Attributes")]
    public HandController firstHand;
    public HandController secondHand;
    public float powerLimit;
    public float powerIncrement;
    private float power;
    private short arms;
    private bool leftRetreiving;
    private bool rightRetreiving;

    [Header("Ground Check Attributes")]
    public GameObject groundCheck;
    public float groundCheckRadius;

    private Animator animator;
    private short dir;
    private short lastDir;
    private enum State { idle, walk, jump_ready, jump_air, charge, fire };
    private State state;
    private bool stateFixed;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movable = true;
        jumped = false;
        blocked = false;
        controlling = true;

        power = 0.0f;
        arms = 2;
        leftRetreiving = false;
        rightRetreiving = false;

        dir = 0;
        lastDir = 1;
        state = State.idle;
        stateFixed = false;
    }
    
    void Update()
    {
        GroundCheck();

        if (controlling)
        {
            Jump();
            Move();
            Shoot();
        }

        changeControl();
        AnimationControl();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            blocked = true;
        }
        else
        {
            blocked = false;
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, LayerMask.GetMask("Ground"));
        if (!isGrounded)
            isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, LayerMask.GetMask("Left Hand"));
        if (!isGrounded)
            isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, LayerMask.GetMask("Right Hand"));

        if (!isGrounded)
        {
            state = State.jump_air;
            jumped = false;
        }
    }

    void Move()
    {
        Vector3 cameraPosition = gameObject.transform.position;
        cameraPosition.z -= 1;
        cameraPosition.y += 5;
        camera.transform.position = cameraPosition;

        Vector2 movement = new Vector2(Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime, 0);

        if (movement.x < 0)
        {
            dir = -1;
            lastDir = -1;
            if (isGrounded)
            {
                if (!stateFixed) state = State.walk;
            }
        }
        if (movement.x > 0)
        {
            dir = 1;
            lastDir = 1;
            if (isGrounded)
            {
                if (!stateFixed) state = State.walk;
            }
        }
        if (movement.x == 0)
        {
            dir = 0;
            if (isGrounded)
            {
                if (!stateFixed) state = State.idle;
            }
        }

        if (movable)
        {
            rigidBody.transform.Translate(movement);
        }

    }

    void Jump()
    {
        if (isGrounded && movable)
        {
            if (Input.GetButtonDown("Jump") && !jumped)
            {
                // Jump start.
                state = State.jump_ready;
                // Wait until the jump_animation is finished.
                Invoke("MakeJump", 0.3f);
                // Player's state is fixed while jump_ready animation is playing.
                stateFixed = true;
                // Player has jumped.
                jumped = true;
            }
        }
    }

    void MakeJump()
    {
        rigidBody.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
        stateFixed = false;
    }

    void Shoot()
    {
        if (isGrounded && !stateFixed)
        {
            // Charge
            if (Input.GetKey(KeyCode.L) && arms != 0)
            {
                if (!leftRetreiving || !rightRetreiving)
                {
                    // Charging start.
                    state = State.charge;
                    // Player can't move while charging.
                    movable = false;
                    // Increase power until limit;
                    if (power < powerLimit) power += powerIncrement;
                }
            }

            // Fire
            if (Input.GetKeyUp(KeyCode.L) && arms != 0)
            {
                if (!leftRetreiving || !rightRetreiving)
                {
                    // Firing start.
                    state = State.fire;
                    // Wait for the fire animation to finish.
                    Invoke("MakeShoot", 0.3f);
                    // Player's state is fixed while the animation is playing
                    stateFixed = true;
                    // Call HandController class's function to actually fire
                    if (arms == 2) firstHand.Fire(power);
                    if (arms == 1) secondHand.Fire(power);
                    power = 0.0f;
                }
            }
        }

        // Retreive
        if (Input.GetKeyDown(KeyCode.R) && movable)
        {
            if (arms == 1)
            {
                leftRetreiving = true;
                firstHand.StartRetrieve();
            }
            else if (arms == 0)
            {
                leftRetreiving = true;
                rightRetreiving = true;
                firstHand.StartRetrieve();
                secondHand.StartRetrieve();
            }
        }

        // Check if retreiving is all done
        if (leftRetreiving)
        {
            leftRetreiving = !firstHand.getRetreiveComplete();
            if (!leftRetreiving) arms++;
        }
        if (rightRetreiving)
        {
            rightRetreiving = !secondHand.getRetreiveComplete();
            if (!rightRetreiving) arms++;
        }
        
    }

    void MakeShoot()
    {
        // Once firing is done, player is able to move, change state and an arm is reduced.
        movable = true;
        stateFixed = false;
        state = State.idle;
        arms--;
    }

    void changeControl()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && arms != 2)
        {
            if (arms == 1)
            {
                if (controlling)
                {
                    controlling = false;
                    firstHand.setControlling(true);
                }
                else if (firstHand.getControlling())
                {
                    controlling = true;
                    firstHand.setControlling(false);
                }
            }
            else if (arms == 0)
            {
                if (controlling)
                {
                    controlling = false;
                    firstHand.setControlling(true);
                }
                else if (firstHand.getControlling())
                {
                    firstHand.setControlling(false);
                    secondHand.setControlling(true);
                }
                else
                {
                    controlling = true;
                    secondHand.setControlling(false);
                }

            }
        }
    }

    void AnimationControl()
    {
        switch (state)
        {
            case State.idle:
                if (lastDir == 1)
                {
                    if (arms == 2) animator.Play("Idle_Right_1");
                    if (arms == 1) animator.Play("Idle_Right_2");
                    if (arms == 0) animator.Play("Idle_Right_3");

                }
                if (lastDir == -1)
                {
                    if (arms == 2) animator.Play("Idle_Left_1");
                    if (arms == 1) animator.Play("Idle_Left_2");
                    if (arms == 0) animator.Play("Idle_Left_3");
                }
                break;
            case State.walk:
                if (dir == 1)
                {
                    if (arms == 2) animator.Play("Walk_Right_1");
                    if (arms == 1) animator.Play("Walk_Right_2");
                    if (arms == 0) animator.Play("Walk_Right_3");
                }
                if (dir == -1)
                {
                    if (arms == 2) animator.Play("Walk_Left_1");
                    if (arms == 1) animator.Play("Walk_Left_2");
                    if (arms == 0) animator.Play("Walk_Left_3");
                }
                if (!controlling) state = State.idle;
                break;
            case State.jump_ready:
                if (lastDir == 1)
                {
                    if (arms == 2) animator.Play("Jump_Right_Ready_1");
                    if (arms == 1) animator.Play("Jump_Right_Ready_2");
                    if (arms == 0) animator.Play("Jump_Right_Ready_3");
                }
                if (lastDir == -1)
                {
                    if (arms == 2) animator.Play("Jump_Left_Ready_1");
                    if (arms == 1) animator.Play("Jump_Left_Ready_2");
                    if (arms == 0) animator.Play("Jump_Left_Ready_3");
                }
                break;
            case State.jump_air:
                if (lastDir == 1)
                {
                    if (arms == 2) animator.Play("Jump_Right_Air_1");
                    if (arms == 1) animator.Play("Jump_Right_Air_2");
                    if (arms == 0) animator.Play("Jump_Right_Air_3");
                }
                if (lastDir == -1)
                {
                    if (arms == 2) animator.Play("Jump_Left_Air_1");
                    if (arms == 1) animator.Play("Jump_Left_Air_2");
                    if (arms == 0) animator.Play("Jump_Left_Air_3");
                }
                if (!controlling && groundCheck) state = State.idle;
                break;
            case State.charge:
                if (lastDir == 1)
                {
                    if (arms == 2) animator.Play("Shoot_Right_Charge_1");
                    if (arms == 1) animator.Play("Shoot_Right_Charge_2");
                    if (arms == 0) animator.Play("Shoot_Right_Charge_3");
                }
                if (lastDir == -1)
                {
                    if (arms == 2) animator.Play("Shoot_Left_Charge_1");
                    if (arms == 1) animator.Play("Shoot_Left_Charge_2");
                    if (arms == 0) animator.Play("Shoot_Left_Charge_3");
                }
                break;
            case State.fire:
                if (lastDir == 1)
                {
                    if (arms == 2) animator.Play("Shoot_Right_Fire_1");
                    if (arms == 1) animator.Play("Shoot_Right_Fire_2");
                }
                if (lastDir == -1)
                {
                    if (arms == 2) animator.Play("Shoot_Left_Fire_1");
                    if (arms == 1) animator.Play("Shoot_Left_Fire_2");
                }
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
    }

    public short getDir()
    {
        return lastDir;
    }

    public bool getControlling()
    {
        return controlling;
    }

    public void setControlling(bool input)
    {
        controlling = input;
    }
}
