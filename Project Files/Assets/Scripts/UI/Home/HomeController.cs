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
    public GameObject   noSaveDataDialog;
    public GameObject   saveDataExistsDialog;
    public GameObject   settingsDialog;
    public GameObject   quitDialog;
    private int         selectedMenu = 1;
    private enum        focus { main, noSaveData, saveDataExists, settings, quit };
    private             focus focusStatus = focus.main;
 
    void Start()
    {
        Cursor.visible = false;

        // Dialogs
        noSaveDataDialog    .SetActive(false);
        saveDataExistsDialog.SetActive(false);
        settingsDialog      .SetActive(false);
        quitDialog          .SetActive(false);

        // Indicator
        Vector3 origin = menu_1.transform.position;
        origin.x += 2.5f;
        indicator.transform.position = origin;
    }

    void Update()
    {
        switch (focusStatus)
        {
            case focus.main:
                DirectionalKeys();
                EnterKey();
                break;
            case focus.noSaveData:
                break;
            case focus.saveDataExists:
                break;
            case focus.settings:
                break;
            case focus.quit:
                break;
        }
        
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
                    NewGame();
                    break;
                case 2:
                    LoadGame();
                    break;
                case 3:
                    Settings();
                    break;
                case 4:
                    Quit();
                    break;
            }
        }
    }

    private void NewGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            
        }
        else
        {
            saveDataExistsDialog.SetActive(true);
            focusStatus = focus.saveDataExists;
        }
    }

    private void LoadGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            noSaveDataDialog.SetActive(true);
            focusStatus = focus.noSaveData;
        }
        else
        {

        }
    }

    private void Settings()
    {
        settingsDialog.SetActive(true);
        focusStatus = focus.settings;
    }
    
    private void Quit()
    {
        quitDialog.SetActive(true);
        focusStatus = focus.quit;
    }
}
