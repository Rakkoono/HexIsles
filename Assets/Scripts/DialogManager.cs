using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogBox;
    [SerializeField]
    private TMP_Text[] lines = new TMP_Text[3];

    private Sign currentSign;
    private int page = 0;

    public void Hide()
    {
        page = 0;
        if (dialogBox.activeSelf)
            Manager.GUI.dialogBoxAnim.Play("DialogBoxOut");
    }

    public void ShowFromSign(Sign sign)
    {
        if (sign == null && currentSign == null)
        {
            Hide();
            return;
        }
        if (sign != null && sign != currentSign)
            page = 0;
        page++;
        if (sign) currentSign = sign;
        if (page - 1 >= currentSign.dialog.Length) Hide();
        else Show(currentSign.dialog[page - 1]);
    }

    public void Show(string dialog)
    {
        string[] lines = dialog.Split('\n');
        for (int i = 0; i < this.lines.Length; i++)
            this.lines[i].text = lines.Length > i ? lines[i] : "";
        dialogBox.SetActive(true);
    }

    public void NextPage() => ShowFromSign(null);
}