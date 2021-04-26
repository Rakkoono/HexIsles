using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : Menu
{
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text messageText;

    public void Open(GameOverData data)
    {
        if (Current.StopGameplay)
            return;

        if (data.UnlockNextLevel && Manager.Instance.CompletedLevels == Manager.Instance.LevelIndex - 1)
            Manager.Instance.UnlockNextLevel();

        nextButton.interactable = Manager.Instance.CompletedLevels >= Manager.Instance.LevelIndex;

        base.Open();

        // Show random game over message
        messageText.text = data.Messages.RandomElement();

        // Play sound effect
        Manager.SfxSource.PlayOneShot(data.Sound, .7f);
    }
}
