using UnityEngine;

public class Deactivator : MonoBehaviour
{
    public void Deactivate()
    {
        gameObject.SetActive(false);

        if (Manager.showUseSigns && Manager.Levels.CurrentIndex == 1 && gameObject.name == "MainMenuScreen")
        {
            Manager.showUseSigns = false;
            Manager.Dialogs.Show("\nClick on Signs to read them");
        }
    }
}
