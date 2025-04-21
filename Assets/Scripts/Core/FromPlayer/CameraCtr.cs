using Unity.VisualScripting;
using UnityEngine;

public class CameraCtr : MonoBehaviour
{
    public Camera cam; 
    private Player player;

    public float maxOffset = 10f;             // Độ lệch tối đa từ player
    public float deadZoneRadius = 4f;        // Rìa mềm: khoảng cách không phản ứng
    public float smoothSpeed = 1.8f;           // Tốc độ di chuyển camera

    [Header("Zoom Settings")]
    public float minZoom = 3f;
    public float maxZoom = 6f;
    public float zoomSpeed = 5f;
    private float targetZoom = 5f; // Giá trị zoom mục tiêu

    private Vector3 velocity = Vector3.zero;


    void FixedUpdate()
    {
       if (player == null) return;

        // --- Zoom bằng con lăn chuột ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Làm mượt zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 10f);

        // --- Camera follow theo chuột (với rìa mềm) ---
        Vector3 clampedOffset = Vector3.zero;
        if(Input.GetMouseButton(1))
        {
            Vector3 offset = cam.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
            offset.z = 0;
            float distance = offset.magnitude;

            
            if (distance > deadZoneRadius)
            {
                float t = Mathf.InverseLerp(deadZoneRadius, maxOffset, distance);
                clampedOffset = offset.normalized * Mathf.Lerp(0, maxOffset, t);
            }

        }
        Vector3 targetPos = player.transform.position + clampedOffset;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / smoothSpeed);
    }



    public static Camera getMainCamera()
    {
        if(Camera.main != null)
        {
            return Camera.main;
        }
        Camera camera = GameObject.FindFirstObjectByType<Camera>().GetComponent<Camera>();
        if(camera == null)
        {
            Debug.LogError("No camera found in the scene.");
            return null;
        }
        return camera;
    }
    
    public static Camera CreateCameraCtr(Player player)
    {
        Camera camera = getMainCamera();
        if (camera == null)
        {
            return null;
        }
        CameraCtr cameraCtr = camera.AddComponent<CameraCtr>();
        cameraCtr.cam = camera;
        cameraCtr.player = player;
        return camera;
    }
}
