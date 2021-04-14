using System.Collections;
using TMPro;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    private TMP_Text text;
    private Animator animator;

    private Dialog currentDialog;
    private int currentPage = 0;
    private Coroutine showTextCoroutine = null;

    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void HideDialog()
    {
        currentPage = 0;
        if (animator.gameObject.activeSelf)
            Manager.UIAnimateOut(animator);
    }

    public void ShowDialog(Dialog dialog)
    {
        if (currentDialog && currentPage >= currentDialog.Pages.Length)
        {
            HideDialog();
            return;
        }

        if (dialog != currentDialog)
        {
            currentPage = 0;
            currentDialog = dialog;
        }
        
        if (!currentDialog)
        {
            Debug.LogWarning("No Dialog");
        }
        animator.gameObject.SetActive(true);
        if (showTextCoroutine != null) StopCoroutine(showTextCoroutine);
        showTextCoroutine = StartCoroutine(ShowText(currentDialog.Pages[currentPage]));
        
        currentPage++;
    }

    private IEnumerator ShowText(string text)
    {
        this.text.text = "";
        foreach (var c in text.ToCharArray())
        {
            this.text.text += c;
            yield return new WaitForSeconds(Time.deltaTime * Config.Current.LetterAnimationTime);
        }
    }

    public void NextPage() => ShowDialog(currentDialog);
}