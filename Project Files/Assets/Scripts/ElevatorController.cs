using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public float speed;
    public float upperBound;
    public float lowerBound;

    public GameObject elevator;
    public ElevatorSwitchController elevatorController;
    private Vector2 elevatorPosition;

    private bool up;
    private bool activated;

    private void Start()
    {
        elevatorPosition = elevator.transform.position;
        up = false;
        activated = false;
    }

    private void Update()
    {
        SwitchCheck();
        ActivateElevator();
    }

    private void SwitchCheck()
    {
        if (elevatorController.getToggled())
        {
            if (up)
                up = false;
            else up = true;

            if (!activated)
                activated = true;

            elevatorController.setToggled(false);
        }
    }

    private void ActivateElevator()
    {
        if (activated)
        {
            elevatorPosition = elevator.transform.position;
            if (up)
            {
                if (elevatorPosition.y < upperBound)
                {
                    elevator.transform.Translate(new Vector2(0, speed * Time.deltaTime));
                }
                else
                {
                    activated = false;
                }
            }
            else // down
            {
                if (elevatorPosition.y > lowerBound)
                {
                    elevator.transform.Translate(new Vector2(0, -1 * speed * Time.deltaTime));
                }
                else
                {
                    activated = false;
                }
            }
        }
    }

}
