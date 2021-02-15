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
        if (GameManager.instance && GameManager.instance.Selected != this && !GameManager.instance.coloredFields.Contains(this))
            rend.material.SetColor("_Color", initialColor + GameManager.instance.highlightTint);
    }

    private void OnMouseExit()
    {
        // de-highlight object
        if (GameManager.instance.Selected != this && !GameManager.instance.coloredFields.Contains(this))
            ResetColor();
    }

    public void OnMouseDown()
    {
        if (GameManager.instance.Selected != this)
        {
            // deselect last selected object, select this object
            if (GameManager.instance.Selected)
            {
                GameManager.instance.Selected.ResetColor();
                GameManager.instance.Selected.OnDeselect();
            }

            GameManager.instance.Selected = this;
            rend.material.SetColor("_Color", initialColor + GameManager.instance.selectionTint);
            OnSelect();
        }
    }

    private void ResetColor()
        => rend.material.SetColor("_Color", initialColor);

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }

}