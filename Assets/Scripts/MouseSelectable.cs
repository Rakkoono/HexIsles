using System.Linq;
using UnityEngine;

public class MouseSelectable : MonoBehaviour
{
    // Hidden variables
    [HideInInspector]
    public Color color;
    [HideInInspector]
    public Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        color = rend.material.color;
    }

    private void OnMouseEnter()
    {
        // highlight object
        if (Manager.Players && Manager.Players.SelectedObject != this && !Manager.Players.coloredObjects.Contains(this))
            rend.material.color = color + Manager.Players.highlightTint;
    }

    private void OnMouseExit()
    {
        // de-highlight object
        if (Manager.Players.SelectedObject != this && !Manager.Players.coloredObjects.Contains(this))
            ResetColor();
    }

    private void OnMouseDown() => ToggleSelect();

    public void ToggleSelect() => Manager.Players.SelectedObject = Manager.Players.SelectedObject == this ? null : this;

    public void ResetColor()
        => rend.material.color = color;

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }

}