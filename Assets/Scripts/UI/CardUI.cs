using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : EventComponent
{
    public Image headImg;
    public Text heroName;
    public Text msg;

    public Transform starPanel;

    int id;

    /**
    public void UpdateView(int id)
    {
        this.id = id;
        HeroConfigData data = HeroConfig.GetInstance().GetSoldierConfig(id.ToString());

        // 名字
        heroName.text = data.name;
        //msg.text = data.msg;

        // 头像
        ResManager res = ResManager.GetInstance();
        Texture2D texture2D = res.GetHeadImg(id);

        if(texture2D != null)
        {
            Sprite sprite = Sprite.Create(texture2D,
                new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            headImg.sprite = sprite;
        }

        // 星星
        Image[] children = starPanel.GetComponentsInChildren<Image>();
        foreach (Image child in children)
        {
            child.gameObject.SetActive(false);
        }

        Image baseImg = children[0];

        for (int i = 0; i < data.star; i++)
        {
            Image img;
            if (i < children.Length)
            {
                img = children[i];
            }
            else
            {
                img = Instantiate<Image>(baseImg, starPanel);
            }

            img.gameObject.SetActive(true);
        }
    }
    */

    public void OnClickHero()
    {
        Camp camp = BattleData.GetInstance().GetSelfCamp();
        
    }
}
