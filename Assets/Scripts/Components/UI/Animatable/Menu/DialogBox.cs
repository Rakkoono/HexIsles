using System.Collections;
using RedBlueGames.Tools.TextTyper;
using TMPro;
using UnityEngine;

public class DialogBox : Menu
{
    private TextTyper typer;
    private TextTyper Typer => typer ??= GetComponentInChildren<TextTyper>();

    private Dialog currentDialog;
    private int currentPage = 0;

    public override void Close()
    {
        currentPage = 0;
        if (Animator.gameObject.activeSelf)
            base.Close();
    }

    public void Open(Dialog dialog)
    {
        if (currentPage >= currentDialog?.Pages.Length)
        {
            Close();
            return;
        }

        if (dialog != currentDialog)
        {
            currentPage = 0;
            currentDialog = dialog;
        }

        if (!currentDialog)
            Debug.LogWarning("No Dialog");

        Open();
        Typer.Skip();
        Typer.TypeText(currentDialog.Pages[currentPage]);

        currentPage++;
    }

    public void NextPage() => Open(currentDialog);
}