using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleCenter : MonoBehaviour
{
    public enum BattleSpeed
    {
        STOP,
        SLOW,
        NORMAL,
        FAST,
        FASTEST,
    }

    private BattleSpeed currSpeedType = BattleSpeed.STOP;
    private float currSpeed;

    Coroutine c;

    GameControl control;

    public GameControl gameControl
    {
        get
        {
            return control;
        }
    }
    

    // 操作
    protected int targetMask;
    protected bool mouseDown = false;
    protected Vector3 downPosition;
    protected Vector3 upPosition;


    public BattleCenter()
    {
        control = new GameControl();
        BattleWorld.gameControl = control;
    }

    // Start is called before the first frame update
    void Start()
    {
        targetMask = LayerMask.GetMask("BattleMap");

        //StartBattle();
    }

    public void Update()
    {

    }


    // 选择区域内的人物
    // 区域为梯形
    private void SelectSoldiers()
    {
        // 左/右上角 downPosition
        // 右/左下角 upPosition
    }

    public void SetCampNum(int num)
    {
        control.SetCampNum(num);

    }

    

    public void StartBattle()
    {
        c = StartCoroutine(MainLoop());
    }

    public void StopBattle()
    {
        if (c != null)
        {
            StopCoroutine(c);
            c = null;

            control.StopBattle();
        }
    }

    private float GetBattleSpeed()
    {
        float speed = BattleConstant.deltaTime;

        switch (currSpeedType)
        {
            case BattleSpeed.STOP:
                speed /= 1f;
                break;
            case BattleSpeed.SLOW:
                speed /= 0.5f;
                break;
            case BattleSpeed.NORMAL:
                speed /= 1f;
                break;
            case BattleSpeed.FAST:
                speed /= 2f;
                break;
            case BattleSpeed.FASTEST:
                speed /= 3f;
                break;
        }

        return speed;
    }

    public void UpdateSpeed(BattleSpeed speed)
    {
        Debug.Log("-----------------");

        BattleSpeed old = currSpeedType;
        currSpeedType = speed;
        currSpeed = GetBattleSpeed();

        if(currSpeedType == BattleSpeed.STOP)
        {
            
            StopBattle();
        }
        else if(old == BattleSpeed.STOP)
        {
            StartBattle();
        }
    }

    public BattleSpeed GetCurrBattleSpeed()
    {
        return currSpeedType;
    }

    // 游戏主循环
    private IEnumerator MainLoop()
    {
        while (true)
        {
            UpdateFrame();
            yield return new WaitForSecondsRealtime(currSpeed);
        }
    }

    private void UpdateFrame()
    {
        // Debug.Log("UpdateFrame");
        control.LogicUpdate();
    }

    public GameControl GetGameControl()
    {
        return control;
    }


}
