using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeController : MonoBehaviour
{
    public GameObject   indicator;
    public GameObject   menu_1;
    public GameObject   menu_2;
    public GameObject   menu_3;
    public GameObject   menu_4;
    private int         selectedMenu = 1;
 
    void Start()
    {
        Vector3 origin = menu_1.transform.position;
        origin.x += 2.5f;
        indicator.transform.position = origin;
    }

    void Update()
    {
        DirectionalKeys();
        EnterKey();
    }

    private void DirectionalKeys()
    {
        if (Input.GetKeyDown("down") || Input.GetKeyDown("s"))
        {
            MoveIndicator(-1);
        }
        if (Input.GetKeyDown("up") || Input.GetKeyDown("w"))
        {
            MoveIndicator(1);
        }
    }

    private void MoveIndicator(int dir)
    {
        Vector3 position;
        if (dir == 1)
        {
            switch (selectedMenu)
            {
                case 1:
                    break;
                case 2:
                    position = menu_1.transform.position;
                    position.x += 2.5f;
                    indicator.transform.position = position;
                    selectedMenu = 1;
                    break;
                case 3:
                    position = menu_2.transform.position;
                    position.x += 2.5f;
                    indicator.transform.position = position;
                    selectedMenu = 2;
                    break;
                case 4:
                    position = menu_3.transform.position;
                    position.x += 2.5f;
                    indicator.transform.position = position;
                    selectedMenu = 3;
                    break;
            }
        }
        if (dir == -1)
        {
            if (selectedMenu != 4)
            {
                switch (selectedMenu)
                {
                    case 1:
                        position = menu_2.transform.position;
                        position.x += 2.5f;
                        indicator.transform.position = position;
                        selectedMenu = 2;
                        break;
                    case 2:
                        position = menu_3.transform.position;
                        position.x += 2.5f;
                        indicator.transform.position = position;
                        selectedMenu = 3;
                        break;
                    case 3:
                        position = menu_4.transform.position;
                        position.x += 2.5f;
                        indicator.transform.position = position;
                        selectedMenu = 4;
                        break;
                    case 4:
                        break;
                }
            }
        }
    }

    private void EnterKey()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("return"))
        {
            switch (selectedMenu)
            {
                case 1:
                    Debug.Log("NEW GAME");
                    break;
                case 2:
                    Debug.Log("LOAD GAME");
                    break;
                case 3:
                    Debug.Log("SETTINGS");
                    break;
                case 4:
                    Debug.Log("QUIT");
                    break;
            }
        }
    }
}
