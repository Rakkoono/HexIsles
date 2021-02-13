using System.Collections.Generic;
using UnityEngine;

public class HexagonField : MonoBehaviour
{

    // Serialized Variables
    [SerializeField, Range(1, 10)]
    public int height = 1;

    // Hidden Variables
    [HideInInspector]
    public List<GameObject> objects = new List<GameObject>();
    [HideInInspector]
    public Vector2Int position = new Vector2Int();
    private int lastHeight = 1;

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

    // Debugging Method
    [ContextMenu("Delete")]
    private void DeleteField()
    {
        // Remove the field
        HexagonGrid.Instance.fields.Remove(position);
        HexagonGrid.Instance.deleted.Add(position);

#if UNITY_EDITOR
        DestroyImmediate(gameObject);
#else
        Destroy(gameObject);
#endif
    }
}