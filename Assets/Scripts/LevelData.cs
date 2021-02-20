using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    // Serialized variables
    public int turns = 1;
    [SerializeField]
    private int turnsLeft = 1;
    public int movesPerTurn = 1;
    public Vector2Int targetPosition;
    public Player.MoveSet playerType;

    // Hidden variables
    private int movesLeft;
    public int MovesLeft
    {
        get => movesLeft;
        set
        {
            movesLeft = value;
            if (movesLeft > movesPerTurn)
            {
                movesLeft -= movesPerTurn;
                TurnsLeft++;
            }
            else if (movesLeft <= 0)
            {
                movesLeft += movesPerTurn;
                TurnsLeft--;
            }
            Manager.GUI.movesLeftDisplay.text = "";
            for (int i = 0; i < movesLeft; i++)
                Manager.GUI.movesLeftDisplay.text += "o ";
        }
    }
    public int TurnsLeft
    {
        get => turnsLeft;
        set
        {
            turnsLeft = value;
            Manager.Players.moved = new List<Player>();
            if (turns != turnsLeft && Manager.Levels.CurrentIndex > 2)
                Manager.Players.PetrifyLonePlayers();

            foreach (TMP_Text txt in Manager.GUI.turnDisplay.GetComponentsInChildren<TMP_Text>())
                txt.text = turnsLeft.ToString();
        }
    }
}
