using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompareProgress : Progress
{
    public Image baseProgressImage;

    [Range(0, 1)]
    public float baseProgress;

    public Color lowerColor;
    public Color higherColor;

    private float currProgress;

    void Start()
    {
        UpdateDirection();

        SetProgress(this.progress);
    }

    public new void SetProgress(float progress)
    {
        this.currProgress = progress;

        // 当前大于原先值
        if(currProgress > baseProgress)
        {
            // 上面的显示原先值，下面的显示当前值，并且为绿色
            this.progress = baseProgress;
        }
        else
        {
            // 上面的显示当前值，下面的显示原先值，并且为红色
            this.progress = this.currProgress;
        }

        UpdateProgress();
    }

    public void SetBaseProgress(float progress)
    {
        this.baseProgress = progress;

        this.SetProgress(this.currProgress);

        //UpdateBaseProgress();
    }

    protected new void UpdateDirection()
    {
        base.UpdateDirection();

        switch (direction)
        {
            case Direction.BottomToTop:
                baseProgressImage.rectTransform.pivot = new Vector2(0.5f, 0);
                break;
            case Direction.LeftToRight:
                baseProgressImage.rectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case Direction.RightToLeft:
                baseProgressImage.rectTransform.pivot = new Vector2(1f, 0.5f);
                break;
            case Direction.TopToBottom:
                baseProgressImage.rectTransform.pivot = new Vector2(0.5f, 1f);
                break;
        }

        baseProgressImage.rectTransform.offsetMax = new Vector2(0, 0);
        baseProgressImage.rectTransform.offsetMin = new Vector2(0, 0);
    }

    protected new void UpdateProgress()
    {
        base.UpdateProgress();

        this.UpdateBaseProgress();
    }


    void UpdateBaseProgress()
    {
        float showProgress;
        // 当前大于原先值
        if (currProgress > baseProgress)
        {
            // 上面的显示原先值，下面的显示当前值，并且为绿色
                        baseProgressImage.color = higherColor;
            showProgress = this.currProgress;
        }
        else
        {
            // 上面的显示当前值，下面的显示原先值，并且为红色
            baseProgressImage.color = lowerColor;
            showProgress = this.baseProgress;
        }

        switch (direction)
        {
            case Direction.BottomToTop:
                baseProgressImage.rectTransform.localScale = new Vector3(1f, showProgress, 1f);
                break;
            case Direction.LeftToRight:
                baseProgressImage.rectTransform.localScale = new Vector3(showProgress, 1f, 1f);
                break;
            case Direction.RightToLeft:
                baseProgressImage.rectTransform.localScale = new Vector3(showProgress, 1f, 1f);
                break;
            case Direction.TopToBottom:
                baseProgressImage.rectTransform.localScale = new Vector3(1f, showProgress, 1f);
                break;
        }

    }
}
