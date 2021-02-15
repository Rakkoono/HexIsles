using TMPro;
using UnityEngine;

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
            if (movesLeft <= 0)
            {
                TurnsLeft--;
                movesLeft = movesPerTurn;
                GameManager.instance.PetrifyLonePlayers();
            }
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
            if (turnsLeft <= 0)
                GameManager.instance.GameOver("Out of turns!");
            foreach (TMP_Text txt in GameManager.instance.turnDisplay.GetComponentsInChildren<TMP_Text>())
                txt.text = TurnsLeft + " Turn" + (TurnsLeft == 1 ? "" : "s");
        }
    }

    private void Awake() => movesLeft = movesPerTurn;
}
