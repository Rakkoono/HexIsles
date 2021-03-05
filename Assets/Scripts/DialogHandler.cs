using System.Collections;
using TMPro;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogBox;
    public Dialog startUpDialog;

    private Dialog currentDialog;
    private int currentPage = 0;
    private Coroutine showTextCoroutine = null;

    public void Hide()
    {
        currentPage = 0;
        if (dialogBox.transform.parent.gameObject.activeSelf)
            Manager.UI.dialogBoxAnimator.Play("DialogBoxOut");
    }

    public void Show(Dialog dialog)
    {
        if (currentDialog && currentPage >= currentDialog.Pages.Length)
        {
            Hide();
            return;
        }

        if (dialog != currentDialog)
        {
            currentPage = 0;
            currentDialog = dialog;
        }
        
        if (showTextCoroutine != null) StopCoroutine(showTextCoroutine);
        showTextCoroutine =  StartCoroutine(ShowText(currentDialog.Pages[currentPage]));
        
        currentPage++;
    }

    private IEnumerator ShowText(string text)
    {
        dialogBox.transform.parent.gameObject.SetActive(true);
        dialogBox.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogBox.text += c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void NextPage() => Show(currentDialog);
}