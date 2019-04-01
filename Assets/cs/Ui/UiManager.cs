using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject baseMenu;
    public NewMapMenu newMapMenu;

    bool menuOpen = false;

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (menuOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    void Open()
    {
        menuOpen = true;
        CameraMove.Locked = true;
        baseMenu.SetActive(true);
    }

    void Close()
    {
        menuOpen = false;
        CameraMove.Locked = false;
        baseMenu.SetActive(false);

        if (newMapMenu.gameObject.activeSelf)
        {
            newMapMenu.gameObject.SetActive(false);
        }
    }
}
