using System.Linq;
using UnityEngine;

public class MouseSelectable : MonoBehaviour
{
    // Hidden variables
    [HideInInspector]
    public Color initialColor;
    [HideInInspector]
    public Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        initialColor = rend.material.color;
    }

    private void OnMouseEnter()
    {
        // highlight object
        if (Manager.Players && Manager.Players.SelectedObject != this && !Manager.Players.coloredFields.Contains(this))
            rend.material.SetColor("_Color", initialColor + Manager.Players.highlightTint);
    }

    private void OnMouseExit()
    {
        // de-highlight object
        if (Manager.Players.SelectedObject != this && !Manager.Players.coloredFields.Contains(this))
            ResetColor();
    }

    public void OnMouseDown()
    {
        if (Manager.Players.SelectedObject != this)
        {
            // deselect last selected object, select this object
            if (Manager.Players.SelectedObject)
            {
                Manager.Players.SelectedObject.ResetColor();
                Manager.Players.SelectedObject.OnDeselect();
            }

            Manager.Players.SelectedObject = this;
            rend.material.SetColor("_Color", initialColor + Manager.Players.selectionTint);
            OnSelect();
        }
        else
        {
            Manager.Players.SelectedObject = null;
            ResetColor();
            OnDeselect();
        }
    }

    private void ResetColor()
        => rend.material.SetColor("_Color", initialColor);

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }

}