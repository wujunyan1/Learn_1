using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public GameObject baseMenu;
    public NewMapMenu newMapMenu;

    public Transform buildPanel;

    public GameObject InputPanelPrefable;
    GameObject currInputPanel;

    bool menuOpen = false;

    private void Awake()
    {
        instance = this;
    }

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

    public GameObject OpenInputPanel()
    {
        CloseInputPanel();
        currInputPanel = Instantiate(InputPanelPrefable);
        currInputPanel.transform.SetParent(buildPanel, false);

        return currInputPanel;
    }

    public void CloseInputPanel()
    {
        if (currInputPanel)
        {
            Destroy(currInputPanel);
            currInputPanel = null;
        }
    }
}
