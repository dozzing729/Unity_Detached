using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{

    private int     stage;
    private int     enabledArms;
    private Vector3 position;

    public SaveData(int stage, PlayerController player)
    {
        this.stage  = stage;
        position    = player.transform.position;
        enabledArms = player.GetEnabledArms();
    }
}
