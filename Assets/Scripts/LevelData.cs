using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelData : MonoBehaviour
{
    // Serialized variables
    public int turnsLeft = 1;
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
            if (movesLeft <= 0) TurnsLeft--;
            GameManager.instance.movesLeftDisplay.text = "";
            for (int i = 0; i < movesLeft; i++) GameManager.instance.movesLeftDisplay.text += "o ";
        }
    }
    public int TurnsLeft
    {
        get => turnsLeft;
        set
        {
            turnsLeft = value;
            movesLeft = movesPerTurn;
            GameManager.instance.playersMoved = new List<Player>();
            if (SceneManager.GetActiveScene().buildIndex > 2)
                GameManager.instance.PetrifyLonePlayers();
            foreach (TMP_Text txt in GameManager.instance.turnDisplay.GetComponentsInChildren<TMP_Text>())
                txt.text = TurnsLeft + " Turn" + (TurnsLeft == 1 ? "" : "s");
        }
    }

    private void Awake() => movesLeft = movesPerTurn;
}
