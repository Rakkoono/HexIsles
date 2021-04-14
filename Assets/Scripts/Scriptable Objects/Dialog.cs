using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "HexIsles/Dialog", order = 0)]
public class Dialog : ScriptableObject
{
    [TextArea(3, 5)]
    [SerializeField] private string[] pages = new string[3];
    public string[] Pages => pages;
}