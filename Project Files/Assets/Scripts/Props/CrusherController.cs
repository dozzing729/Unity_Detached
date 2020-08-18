using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherController : MonoBehaviour
{
    public float speed;
    public float accel;
    public float bound;

    public GameObject columnCollider;
    public GameObject player;
    public GameObject leftArm;
    public GameObject rightArm;

    private Vector2 crusherPosition;
    private Vector2 origin;

    private bool isActivated;
    private bool isGoingUp;


    private void Start()
    {
        crusherPosition = transform.position;
        origin          = crusherPosition;
        isGoingUp       = false;
        isActivated     = true;
    }

    private void Update()
    {
        OperateCrusher();
    }

    private void OperateCrusher()
    {
        crusherPosition = transform.position;

        if (isActivated)
        {
            if (isGoingUp)
            {
                if (crusherPosition.y < origin.y + bound)
                {
                    transform.Translate(new Vector2(0, speed) * Time.deltaTime);
                    columnCollider.transform.Translate(new Vector2(0, speed) * Time.deltaTime);
                }
                else
                {
                    isGoingUp = false;
                }
            }
            else
            {
                if (crusherPosition.y > origin.y)
                {
                    transform.Translate(new Vector2(0, -1 * Mathf.Pow(accel ,speed * Time.deltaTime)));
                    columnCollider.transform.Translate(new Vector2(0, -1 * Mathf.Pow(accel, speed * Time.deltaTime)));
                }
                else
                {
                    isGoingUp = true;
                }
            }
        }
    }
}