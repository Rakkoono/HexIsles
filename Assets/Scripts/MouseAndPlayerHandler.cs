using UnityEngine;

public class MouseAndPlayerHandler : MonoBehaviour
{
    // Serialized variables
    public Transform rotationCenter;
    [SerializeField, Range(0, 100)]
    private float rotationSpeed = 50f;
    [Space, SerializeField, Range(0, 100)]
    private float zoomSpeed = 50f;
    public Color highlightColor = new Color(40, 40, 40, 0), selectColor = new Color(80, 80, 80, 0), nextMoveColor = new Color(90, 90, 60, 0);

    [Range(.1f, 10)]
    public float playerMovementSpeed = 1;

    // Hidden variables
    private Vector3? mousePosition = new Vector3();
    [HideInInspector]
    MouseSelectable selected;
    [HideInInspector]
    public Player selectedPlayer;
    [HideInInspector]
    public HexField[] colored;

    public MouseSelectable Selected
    {
        get => selected;
        set
        {
            if (selected && selected.GetComponent<Player>()) selectedPlayer = selected.GetComponent<Player>();
            else selectedPlayer = null;
            selected = value;
        }
    }

    // Singleton instance
    static MouseAndPlayerHandler instance;
    public static MouseAndPlayerHandler Instance
    {
        get => instance;
        private set
        {
            if (instance)
                Debug.LogWarning("Singleton class HexagonGrid already exists!");
            else
                instance = value;
        }
    }

    void Start() => Instance = this;

    void Update()
    {
        // Pan on right mouse button
        if (Input.GetMouseButton(1))
        {
            if (mousePosition != null && mousePosition != Input.mousePosition)
            {
                float mouseMovement = Input.mousePosition.x - ((Vector3)mousePosition).x;
                Camera.main.transform.RotateAround(rotationCenter.position, Vector3.up, mouseMovement * Time.deltaTime * rotationSpeed);
            }
            mousePosition = Input.mousePosition;
        }
        else mousePosition = null;

        // Zoom on mouse wheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float size = Camera.main.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * 4;
            if (size > 8) size = 8;
            else if (size < 1) size = 1;
            Camera.main.orthographicSize = size;
        }
    }
}
