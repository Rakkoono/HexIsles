﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexField : MouseSelectable
{

    // Serialized fields
    [Range(1, 10)]
    public int height = 1;

    // Hidden fields
    [HideInInspector]
    public List<GameObject> objects = new List<GameObject>();
    [HideInInspector]
    public Vector2Int position = new Vector2Int();
    private int lastHeight = 1;

    public override void OnSelect()
    {
        if (Manager.UI.currentMenu == UIHandler.Menu.None && Manager.Players.lastSelected && Manager.Players.possibleMoves.Contains(this))
                Manager.Players.lastSelected.Move(this);
    }

    // Debugging Method
    private void OnValidate()
    {
        // Update height if changed in editor
        if (height != lastHeight)
        {
            lastHeight = height;

            transform.localPosition = new Vector3(
                transform.position.x,
                .25f * height - .25f,
                transform.position.z
            );

            transform.localScale = new Vector3(
                transform.localScale.x,
                .5f * height,
                transform.localScale.z
            );
        }
    }
}