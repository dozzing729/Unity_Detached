using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : PhysicalObject
{
    [Header("Movement Attributes")]
    public Camera       mainCamera;
    private Rigidbody2D rigidBody;
    public GameObject   normal;
    public float        moveSpeed;
    public float        jumpHeight;
    private float       treadmillVelocity;
    private bool        isOnTreadmill;
    private bool        isGrounded;
    private bool        isMovable;
    private bool        isControlling;

    [Header("Shoot Attributes")]
    public ArmController    firstArm;
    public ArmController    secondArm;
    public float            powerLimit;
    public float            powerIncrement;
    private float           power;
    private short           arms;
    private short           enabledArms;
    private bool            isLeftRetrieving;
    private bool            isRightRetrieving;

    [Header("Ground Check Attributes")]
    public GameObject   groundCheck;
    public float        groundCheckWidth;

    [Header("Animation Attributes")]
    private Animator    animator;
    private short       dir;
    private short       lastDir;
    private enum        State { idle, walk, jump, charge, fire };
    private State       state;
    private bool        isStateFixed;

    [Header("Destruction Attributes")]
    public GameObject   head;
    public GameObject   body;
    public GameObject   left_arm;
    public GameObject   right_arm;
    private bool isDead;

    private new void Start()
    {
        base.Start();
        // Movement attributes
        rigidBody           = GetComponent<Rigidbody2D>();
        treadmillVelocity   = 0;
        isOnTreadmill       = false;
        isMovable           = true;
        isControlling       = true;

        // Shoot attributes
        power               = 0.0f;
        arms                = enabledArms;
        isLeftRetrieving    = false;
        isRightRetrieving   = false;

        // Animation attributes
        animator        = normal.GetComponent<Animator>();
        dir             = 0;
        lastDir         = 1;
        state           = State.idle;
        isStateFixed    = false;

        // Destruction attributes
        isDead = false;
    }

    private new void Update()
    {
        base.Update();
        if (GetDestroyed())
        {
            if (!isDead)
            {
                OnDestruction();
            }
        }
        else
        {
            GroundCheck();
            ChangeControl();
            AnimationControl();
            if (isControlling)
            {
                Jump();
                Move();
                Shoot();
                Retrieve();
            }
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.transform.position, new Vector2(2.2f * groundCheckWidth, 0.5f), 0.0f, LayerMask.GetMask("Ground")) ||
                     Physics2D.OverlapBox(groundCheck.transform.position, new Vector2(2.2f * groundCheckWidth, 0.5f), 0.0f, LayerMask.GetMask("Wall")) ||
                     Physics2D.OverlapBox(groundCheck.transform.position, new Vector2(2.2f * groundCheckWidth, 0.5f), 0.0f, LayerMask.GetMask("Physical Object"));
        if (!isGrounded)
        {
            state = State.jump;
        }
    }

    private void Move()
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

        // User input movement
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        if (horizontal < 0)
        {
            dir     = -1;
            lastDir = -1;
            if (isGrounded && !isStateFixed)
            {
                state = State.walk;
            }
        }
        if (horizontal > 0)
        {
            dir     = 1;
            lastDir = 1;
            if (isGrounded && !isStateFixed)
            {
                state = State.walk;
            }
        }
        if (horizontal == 0)
        {
            dir = 0;
            if (isGrounded && !isStateFixed)
            {
                state = State.idle;
            }
        }

        // Move
        if (isMovable)
        {
            float vertical = rigidBody.velocity.y;

            if (isOnTreadmill)
            {
                horizontal += treadmillVelocity * Time.deltaTime;
                rigidBody.velocity = new Vector3(horizontal, vertical, 0.0f);
            }
            else
            {
                if (horizontal != 0)
                {
                    rigidBody.velocity = new Vector3(horizontal, vertical, 0.0f);
                }
            }
        }

    }

    private void Jump()
    {
        if (isGrounded && isMovable)
        {
            if (Input.GetButtonDown("Jump"))
            {
                float horizontal = rigidBody.velocity.x * Time.deltaTime;
                float vertical = rigidBody.velocity.y + jumpHeight;
                rigidBody.velocity = new Vector3(horizontal, vertical, 0.0f);
            }
        }
    }

    private void Shoot()
    {
        if (isGrounded && !isStateFixed)
        {
            // Charge
            if (Input.GetKey(KeyCode.L) && arms != 0)
            {
                // Charging start.
                state = State.charge;
                // Player can't move while charging.
                isMovable = false;
                // Increase power until limit;
                if (power < powerLimit) power += powerIncrement;
            }

            // Fire
            if (Input.GetKeyUp(KeyCode.L) && arms != 0)
            {
                // Firing start.
                state = State.fire;
                // Wait for the fire animation to finish.
                Invoke("MakeShoot", 0.3f);
                // Player's state is fixed while the animation is playing
                isStateFixed = true;
                // Call HandController class's function to actually fire
                if (arms == 2)
                {
                    firstArm.Fire(power);
                }
                if (arms == 1)
                {
                    if (enabledArms == 1)
                    {
                        firstArm.Fire(power);
                    }
                    else
                    {
                        secondArm.Fire(power);
                    }
                }
                power = 0.0f;
            }
        }
    }

    private void MakeShoot()
    {
        // Once firing is done, player is able to move, change state and an arm is reduced.
        isMovable       = true;
        isStateFixed    = false;
        state           = State.idle;
        arms--;
    }

    private void Retrieve()
    {
        // Retrieve
        if (Input.GetKeyDown(KeyCode.R) && isMovable)
        {
            if (arms == enabledArms - 1 && enabledArms != 0)
            {
                isLeftRetrieving = true;
                firstArm.StartRetrieve();
            }
            else if (arms == enabledArms - 2 && enabledArms == 2)
            {
                isLeftRetrieving = true;
                isRightRetrieving = true;
                firstArm.StartRetrieve();
                secondArm.StartRetrieve();
            }
        }

        // Check if retreiving is all done
        if (isLeftRetrieving)
        {
            isLeftRetrieving = !firstArm.GetRetrieveComplete();
            if (!isLeftRetrieving) arms++;
        }
        if (isRightRetrieving)
        {
            isRightRetrieving = !secondArm.GetRetrieveComplete();
            if (!isRightRetrieving) arms++;
        }

    }

    private void ChangeControl()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && state != State.charge)
        {
            if ((arms == 1 && enabledArms == 2) ||
                (arms == 0 && enabledArms == 1))
            {
                if (isControlling)
                {
                    if (!isLeftRetrieving)
                    {
                        isControlling = false;
                        firstArm.SetControl(true);
                    }
                }
                else if (firstArm.GetControl())
                {
                    isControlling = true;
                    firstArm.SetControl(false);
                }
            }
            else if (arms == 0)
            {
                if (isControlling)
                {
                    if (!isLeftRetrieving)
                    {
                        isControlling = false;
                        firstArm.SetControl(true);
                    }
                }
                else if (firstArm.GetControl())
                {
                    firstArm.SetControl(false);
                    secondArm.SetControl(true);
                }
                else
                {
                    isControlling = true;
                    secondArm.SetControl(false);
                }

            }
        }
    }

    private void AnimationControl()
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
                if (!isControlling) state = State.idle;
                break;
            case State.jump:
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
                if (!isControlling && groundCheck) state = State.idle;
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

    private void OnDestruction()
    {
        head        .transform.parent = null;
        body        .transform.parent = null;
        left_arm    .transform.parent = null;
        right_arm   .transform.parent = null;

        Rigidbody2D headRB      = head.GetComponent<Rigidbody2D>();
        Rigidbody2D bodyRB      = body.GetComponent<Rigidbody2D>();
        Rigidbody2D leftArmRB   = left_arm.GetComponent<Rigidbody2D>();
        Rigidbody2D rightArmRB  = right_arm.GetComponent<Rigidbody2D>();

        headRB      .AddForce(new Vector2(0.0f, 2.0f), ForceMode2D.Impulse);
        bodyRB      .AddForce(new Vector2(0.0f, 1.0f), ForceMode2D.Impulse);
        leftArmRB   .AddForce(new Vector2(1.0f, 1.0f), ForceMode2D.Impulse);
        rightArmRB  .AddForce(new Vector2(-1.0f, 1.0f), ForceMode2D.Impulse);

        isDead = true;
    }

    private void OnDrawGizmos()
    { Gizmos.DrawWireCube(groundCheck.transform.position, new Vector2(2.2f * groundCheckWidth, 0.5f)); }

    public void SetTreadmillVelocity(float treadmillVelocity)
    { this.treadmillVelocity = treadmillVelocity; }

    public bool GetOnTreadMill()
    { return isOnTreadmill; }

    public void SetOnTreadmill(bool isOnTreadmill)
    { this.isOnTreadmill = isOnTreadmill; }

    public void EnableArms(short enabledArms)
    { this.enabledArms = arms = enabledArms; }

    public int GetEnabledArms()
    { return enabledArms; }

    public short GetDir()
    { return lastDir; }

    public bool GetControlling()
    { return isControlling; }

    public void SetControlling(bool input)
    { isControlling = input; }

    public bool GetLeftRetrieving()
    { return isLeftRetrieving; }

    public bool GetRightRetrieving()
    { return isRightRetrieving; }

    public void SetLeftRetrieving(bool input)
    { isLeftRetrieving = input; }

    public void SetRightRetrieving(bool input)
    { isRightRetrieving = input; }

    public short GetArms()
    { return arms; }
}
