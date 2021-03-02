using UnityEngine;

public class Deactivator : MonoBehaviour
{
    public void Deactivate()
    {
        // Deactivate UI after fade out animations
        gameObject.SetActive(false);
        
        // Show sign tutorial on first level load
        if (Manager.Levels.completed == 0 && Manager.Levels.CurrentIndex == 1 && gameObject.name == "MainMenuScreen")
            Manager.Dialogs.Show("\nClick on Signs to read them");
    }
}
