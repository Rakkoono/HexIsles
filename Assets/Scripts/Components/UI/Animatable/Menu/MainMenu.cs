using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    #region Serialized fields
    [SerializeField] private AnimatableUI thxForPlayingMessage;
    [SerializeField] private TMP_Text continueButtonLabel;
    [SerializeField] private Button resetButton;
    #endregion

    #region Hidden fields
    private bool onStartup = true;
    private bool byEscape;
    #endregion

    public override void Open() => Open(false);

    public void Open(bool byEsc)
    {
        byEscape = byEsc;
        base.Open();
    }

    public override void Close()
    {
        if (Manager.Instance.CompletedLevels == 0 && onStartup)
        {
            Manager.DialogBox.Open(Config.StartUpDialog);
            onStartup = false;
        }
        else
            base.Close();
    }

    public void Toggle(bool byEsc)
    {
        if (ThisOrChildIsOpen)
        {
            if (!byEsc || byEscape)
                Close();
        }
        else
            Open(byEsc);
    }

    public override void Show()
    {
        base.Show();

        continueButtonLabel.text = (Manager.Instance.CompletedLevels == 0 || Manager.Instance.CompletedLevels >= Config.Levels.Length) && !byEscape ? "New Game" : "Continue";
        resetButton.interactable = byEscape;
        if (Manager.Instance.CompletedLevels >= Config.Levels.Length)
            thxForPlayingMessage.Show();
    }

    public override void Hide()
    {
        base.Hide();

        thxForPlayingMessage.Hide();
    }

    public void Continue()
    {
        if (byEscape)
            Manager.Overlay.Open();
        else if (Manager.Instance.CompletedLevels >= Config.Levels.Length)
            Manager.Instance.RestartGame();
        else
            Manager.Instance.LoadLatestLevel();
    }
}
