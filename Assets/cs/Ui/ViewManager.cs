using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : EventComponent
{
    private class ViewCount
    {
        public View view;
        public int count;

        public ViewCount(View _view)
        {
            view = _view;
            count = 1;
        }
    }

    LinkedList<View> views;

    public static ViewManager Instance;

    public static Dictionary<string, string> viewsPath;
    private static Dictionary<string, ViewCount> viewPrefabs;

    private void Awake()
    {
        views = new LinkedList<View>();

        Debug.Log("views Awake");

        Instance = this;

        if(viewsPath == null)
        {
            InitViewsPath();
        }
        
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void InitViewsPath()
    {
        viewPrefabs = new Dictionary<string, ViewCount>();

        viewsPath = new Dictionary<string, string>();
        viewsPath.Add("ReadlyEditorMapView", "prefabs/UI/Main/ReadlyEditorMapView");
        viewsPath.Add("HexMapEditorView", "prefabs/UI/EditorPanel");
        viewsPath.Add("ClickCellView", "prefabs/UI/BigBattle/ClickCellView");
        viewsPath.Add("NewMapMenu", "prefabs/UI/BigBattle/NewMapMenu");
        viewsPath.Add("SaveLoadMenu", "prefabs/UI/BigBattle/SaveLoadMenu");
        viewsPath.Add("BigBattleSettingView", "prefabs/UI/BigBattle/BigBattleSettingView");
        viewsPath.Add("LoadBattleRes", "prefabs/UI/BigBattle/loading");
        viewsPath.Add("TroopView", "prefabs/UI/BigBattle/TroopView");
        viewsPath.Add("CityView", "prefabs/UI/BigBattle/CityView");
        viewsPath.Add("CityBuildMessageView", "prefabs/UI/BigBattle/CityBuild/BuildMessage");
    }

    protected override void DestroyEnd()
    {
        Instance = null;
    }

    ObjFunction build = new BuildCityFunction();

    private void Start()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        int num = 10000;

        /// as 比较为 7700/1700   is 983/350  == 3500/2500

        BuildCityFunction b;

        sw.Start();

        for (int i = 0; i < num; i++)
        {
            if(build.Name == "buildCity1")
            {
                b = (BuildCityFunction)build;
            }
        }
        sw.Stop();
        Debug.Log(string.Format("运算{0}次所需时间，{1}Ticks.", num, sw.ElapsedTicks));

        sw.Restart();

        for (int i = 0; i < num; i++)
        {
            if (build is BuildbarrackFunction)
            {
                b = (BuildCityFunction)build;
            }
        }
        sw.Stop();

        Debug.Log(string.Format("运算{0}次所需时间，{1}Ticks.", num, sw.ElapsedTicks));
    }

    public void RemoveView(View v)
    {
        LinkedListNode<View> viewNode = views.Find(v);

        bool showLast = false;

        if (viewNode != null)
        {   
            if(viewNode.Next == null)
            {
                showLast = true;
            }

            Debug.Log("remove view " + v.Name);
            views.Remove(v);
        }

        GameObject.Destroy(v.gameObject);

        if (showLast)
        {
            viewNode = views.Last;
            if(viewNode != null)
            {
                viewNode.Value.Show();
            }
        }
    }

    public void AddView(View v)
    {
        LinkedListNode<View> viewNode = new LinkedListNode<View>(v);
        views.AddLast(viewNode);
    }

    public void ReSetLastView(View v)
    {
        views.Remove(v);
        views.AddLast(v);
    }

    public void CloseView()
    {
        LinkedListNode<View> viewNode = views.Last;
        if(viewNode != null)
        {
            View v = viewNode.Value;
            if (v.CanClose())
            {
                v.Close();

                viewNode = views.Last;
                if (viewNode != null)
                {
                    viewNode.Value.Show();
                }
            }
        }
    }

    private bool GetViewPrefab(string key, out View view)
    {
        ViewCount viewCount;
        if (viewPrefabs.TryGetValue(key, out viewCount))
        {
            view = viewCount.view;
            viewCount.count++;
            return true;
        }

        string path;
        if (viewsPath.TryGetValue(key, out path))
        {
            view = Resources.Load<View>(path);
            viewPrefabs.Add(key, new ViewCount(view));
            return true;
        }

        view = null;
        return false;
    }

    public bool OpenView(string key, out View view, UObject param = null)
    {
        return this.OpenView<View>(key, out view, param);
    }

    public bool OpenView<T>(string key, out T view, UObject param = null) where T : View
    {
        View viewPrefab;
        if (GetViewPrefab(key, out viewPrefab))
        {
            view = (T)Instantiate(viewPrefab, this.transform);
            view.Name = key;
            AddView(view);
            view.Open(param);

            if(param != null)
            {
                UObjectPool.Add(param);
            }
            return true;
        }

        view = null;
        return false;
    }

    public bool OpenOnlyView<T>(string key, out T view, UObject param = null) where T : View
    {
        T v = GetShowView<T>(key);
        if(v == null)
        {
            if(!OpenView<T>(key, out v, param))
            {
                view = null;
                return false;
            }
            else
            {
                view = v;
                return true;
            }
        }
        else
        {
            ReSetLastView(v);
            v.Open(param);

            view = v;
            return true;
        }

    }

    public T GetShowView<T>(string key) where T : View
    {
        Debug.Log("GetShowView");
        foreach (var item in views)
        {
            Debug.Log(item.Name);
            if(item.Name == key)
            {
                return (T)item;
            }
        }

        return null;
    }
}
