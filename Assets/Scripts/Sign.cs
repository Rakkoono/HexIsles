using UnityEngine;

public class Sign : MouseSelectable
{
    [TextArea]
    public string[] dialog;
    public override void OnSelect()
    {
        if (!Manager.GUI.inMenu)
        {
            HexGrid.GetFieldAt(HexGrid.WorldToGridPos(transform.position)).OnMouseDown();
            Manager.Dialogs.ShowFromSign(this);
        }
    }
}
