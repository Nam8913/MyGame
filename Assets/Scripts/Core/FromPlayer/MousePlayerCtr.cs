using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class MousePlayerCtr
{
    public Vector3 mousePos;
    // Update is called once per frame
    public void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
