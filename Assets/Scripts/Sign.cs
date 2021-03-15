public class Sign : MouseSelectable
{
    public Dialog dialog;

    public override void OnSelect()
    {
        if (Manager.UI.currentMenu == UIHandler.Menu.None)
        {
            GridUtility.GetFieldAt(GridUtility.WorldToGridPos(transform.position)).ToggleSelect();
            Manager.Dialogs.Show(dialog);
        }

        Manager.Players.SelectedObject = null;
    }
}
