using UnityEngine;

public class SunRotation : MonoBehaviour
{
    public float rotationSpeed = 10f;

    private void Update()
    {
        transform.Rotate(Vector3.right * (rotationSpeed * Time.deltaTime));
    }
}