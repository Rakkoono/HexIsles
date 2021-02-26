using UnityEngine;

public class Sign : MouseSelectable
{
    [TextArea]
    public string[] dialog;
    public override void OnSelect()
    {
        if (!Manager.UI.inMenu)
        {
            HexGrid.GetFieldAt(HexGrid.WorldToGridPos(transform.position)).ToggleSelect();
            Manager.Dialogs.ShowFromSign(this);
        }
    }
}
