using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputCityName : MonoBehaviour
{
    string name = "城市名称";
    Text nameLabel;

    InputField nameInput;
    Text nameInputText;
    Text Placeholder;

    Button button;

    public BuildCityFunction Func { get; set; }

    private void Awake()
    {
        nameLabel = transform.GetChild(1).GetComponent<Text>();

        nameInput = transform.GetChild(2).GetComponent<InputField>();
        Placeholder = nameInput.transform.GetChild(0).GetComponent<Text>();
        nameInputText = nameInput.transform.GetChild(1).GetComponent<Text>();

        button = transform.GetChild(3).GetComponent<Button>();
    }

    private void Start()
    {
        nameLabel.text = name;

        Placeholder.text = "请输入名字";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate {
            OnButton();
        });

        CameraMove.Locked = true;
    }

    public void OnButton()
    {
        string cityName = nameInputText.text;

        Func.BuildCity(cityName);

        CameraMove.Locked = false;
    }
}
