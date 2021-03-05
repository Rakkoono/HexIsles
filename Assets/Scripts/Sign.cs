public class Sign : MouseSelectable
{
    public Dialog dialog;

    public override void OnSelect()
    {
        if (Manager.UI.currentMenu == UIHandler.Menu.None)
        {
            HexGrid.GetFieldAt(HexGrid.WorldToGridPos(transform.position)).ToggleSelect();
            Manager.Dialogs.Show(dialog);
        }
    }
}
