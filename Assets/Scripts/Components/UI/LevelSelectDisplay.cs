using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectDisplay : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;

    public Sprite Sprite
    {
        get => image.sprite;
        set => image.sprite = value;
    }

    public string Text
    {
        get => text.text;
        set => text.text = value;
    }

    public bool Interactable
    {
        get => button.interactable;
        set => button.interactable = value;
    }
}
