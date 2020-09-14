using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmEnabler : MonoBehaviour
{
    public short arms;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.EnableArms(arms);
            gameObject.SetActive(false);
        }
    }
}
