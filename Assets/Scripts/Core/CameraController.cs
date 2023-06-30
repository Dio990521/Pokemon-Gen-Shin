
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public void SetCameraLimit(float minX, float minY, float maxX, float maxY)
    {
        this.minX = minX;
        this.minY = minY;
        this.maxX = maxX;
        this.maxY = maxY;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector3 cameraPosition = playerTransform.position;
        cameraPosition.x = Mathf.Clamp(playerTransform.position.x, minX, maxX);
        cameraPosition.y = Mathf.Clamp(playerTransform.position.y, minY, maxY);
        transform.position = cameraPosition;
    }

}
