using UnityEngine;

public class Deactivator : MonoBehaviour
{
    public void Deactivate()
    {
        // Deactivate UI after fade out animations
        gameObject.SetActive(false);
        
        // Show sign tutorial on first level load
        if (Manager.Current.completedLevels == 0 && Manager.Current.levelIndex == 1 && gameObject.name == "MainMenuScreen")
            Manager.Dialogs.Show(Config.Current.StartUpDialog);
    }
}
