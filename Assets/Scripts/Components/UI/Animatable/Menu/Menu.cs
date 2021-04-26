using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : AnimatableUI
{
    #region Serialized fields
    [SerializeField] private bool stopGameplay;
    [SerializeField] private Menu parent;
    #endregion

    #region Hidden fields
    private Menu[] parents = { };

    private static Menu current;
    public static Menu Current
    {
        get => current;
        set
        {
            if (current == value)
                return;
            // don't close current menu if the next menu is a child of it
            if (value == null || (current != null && !value.parents.Contains(current)))
            {
                // Close current menu and parent menus if they aren't also parents of the next menu
                current.Hide();
                foreach (var parent in current.parents)
                {
                    if (value != null && (value == parent || (bool)value?.parents.Contains(parent)))
                        break;
                    parent.Hide();
                }
            }

            if (value != null)
            {
                // Open next menu and parent menus
                value.Show();
                foreach (var parent in value.parents)
                    parent.Show();
            }

            current = value;
        }
    }

    public bool StopGameplay => stopGameplay;

    public bool IsOpen => Current == this;
    public bool ThisOrChildIsOpen => IsOpen || Current.parents.Contains(this);
    #endregion

    protected override void Awake()
    {
        base.Awake();
        var parentList = new List<Menu>();
        var child = this;
        while (child.parent != null)
        {
            if (child.parents.Length != 0)
            {
                parentList.AddRange(child.parents);
                break;
            }
            parentList.Add(child.parent);
            child = child.parent;
        }

        parents = parentList.ToArray();

        if (!stopGameplay && parents.Any(menu => menu.StopGameplay))
            stopGameplay = true;
    }

    #region Open / Close / Toggle
    public virtual void Open()
    {
        if (IsOpen)
            return;

        Menu.Current = this;

        if (StopGameplay)
            Manager.Instance.SelectedObject = null;
    }

    public virtual void Close()
    {
        if (!IsOpen)
            return;

        Menu.Current = parent ?? Manager.Overlay;
        EventSystem.current.SetSelectedGameObject(null);
    }

    protected virtual void Toggle()
    {
        if (ThisOrChildIsOpen)
            Close();
        else
            Open();
    }
    #endregion
}
