using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class MousePlayerCtr
{
    public Vector3 mousePos;
    public Vector2 mouseDir;
    public float angle;
    // Update is called once per frame
    public void Update(Player player)
    {
        Vector3 screenPos = Input.mousePosition;
        mousePos = CameraCtr.getMainCamera().ScreenToWorldPoint(screenPos);
        mousePos.z = 0; // Đặt z về 0 để giữ trong mặt phẳng 2D
        mouseDir = (mousePos - player.gameObject.transform.position).normalized;
        angle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg - 90f;
    }
}
