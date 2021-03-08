using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] private Transform rotationCenter;

    private void Update() => transform.RotateAround(rotationCenter.position, Vector3.up, 8 * Time.deltaTime);
}