﻿using System.Linq;
using UnityEngine;

public class HexField : MouseSelectable
{
    // Hidden fields
    [HideInInspector] private int height = 1;
    public int Height
    {
        get => height;
        set
        {
            height = value;

            transform.localPosition = new Vector3(0, .25f * height - .25f, 0);
            transform.localScale = new Vector3(1, .5f * height, 1);
        }
    }
    [HideInInspector] private Vector2Int position = new Vector2Int();
    public Vector2Int Position 
    {
        get => position;
        set
        {
            position = value;

            transform.parent.position = GridUtility.GridToWorldPos(position);
        }
    }

    public override void OnSelect()
    {
        if (Manager.UI.currentMenu == UIHandler.Menu.None && Manager.Players.lastSelected && Manager.Players.possibleMoves.Contains(this))
            Manager.Players.lastSelected.Move(this);
    }
}