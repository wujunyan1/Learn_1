using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : EventComponent
{
    public static UiManager instance;

    public GameObject baseMenu;
    public NewMapMenu newMapMenu;

    public Transform buildPanel;

    public GameObject InputPanelPrefable;
    GameObject currInputPanel;

    bool menuOpen = false;

    List<View> views;
    Dictionary<string, string> uiPaths;

    private void Awake()
    {
        instance = this;
        views = new List<View>();

        RegisterUI();
    }

    private void Start()
    {
        this.RegisterEvent("OPEN_UI", OpenUI);
        
    }

    void RegisterUI()
    {
        uiPaths = new Dictionary<string, string>();

        uiPaths.Add("SaveOrLoadUI", "prefabs/UI/SaveLoadMenu");
        uiPaths.Add("CreateNewGameUI", "prefabs/UI/NewMapMenu");
    }

    void OpenUI(UEvent evt)
    {
        UObject objs = (UObject)evt.eventParams;
        string uiName = (string)objs.Get("UI_NAME");

        string uiPath;
        if(uiPaths.TryGetValue(uiName, out uiPath))
        {
            LoadPrefabs(uiPath, objs);
        }
        
    }

    void LoadPrefabs(string ui_path, UObject o)
    {
        GameObject prefab = Resources.Load(ui_path) as GameObject;
        GameObject v = Instantiate(prefab, this.transform);

        View vi = v.GetComponent<View>();
        vi.Open(o);
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
