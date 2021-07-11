using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public Image bg;
    public Image progressImage;
    public Text progressLabel;

    [Range(0, 1)]
    public float progress;

    private string customText = null;

    public enum Direction
    {
        LeftToRight = 0,
        RightToLeft = 1,
        BottomToTop = 2,
        TopToBottom = 3
    }

    public Direction direction;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDirection();
        UpdateProgress();
    }
    
    protected void UpdateDirection()
    {
        switch (direction)
        {
            case Direction.BottomToTop:
                progressImage.rectTransform.pivot = new Vector2(0.5f, 0);
                break;
            case Direction.LeftToRight:
                progressImage.rectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case Direction.RightToLeft:
                progressImage.rectTransform.pivot = new Vector2(1f, 0.5f);
                break;
            case Direction.TopToBottom:
                progressImage.rectTransform.pivot = new Vector2(0.5f, 1f);
                break;
        }

        progressImage.rectTransform.offsetMax = new Vector2(0, 0);
        progressImage.rectTransform.offsetMin = new Vector2(0, 0);
    }

    protected void UpdateProgress()
    {
        switch (direction)
        {
            case Direction.BottomToTop:
                progressImage.rectTransform.localScale = new Vector3(1f, progress, 1f);
                break;
            case Direction.LeftToRight:
                progressImage.rectTransform.localScale = new Vector3(progress, 1f, 1f);
                break;
            case Direction.RightToLeft:
                progressImage.rectTransform.localScale = new Vector3(progress, 1f, 1f);
                break;
            case Direction.TopToBottom:
                progressImage.rectTransform.localScale = new Vector3(1f, progress, 1f);
                break;
        }

        UpdateText();
    }

    public void SetDirection(Direction direction)
    {
        this.direction = direction;
        UpdateDirection();
    }

    public void SetProgress(float progress)
    {
        this.progress = progress;
        UpdateProgress();
    }

    public void SetCustomText(string str)
    {
        customText = str;
        UpdateText();
    }

    public void UpdateText()
    {
        if(progressLabel == null)
        {
            return;
        }

        if(customText == null)
        {
            progressLabel.text = string.Format("{0:N1}", progress * 100) + "%";
        }
        else
        {
            progressLabel.text = customText;
        }
    }
}
