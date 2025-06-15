using UnityEngine;

public class CrosshairOnGUI : MonoBehaviour
{
    public Texture2D crosshairTex;
    public float size = 32f;  // 그릴 크기

    void OnGUI()
    {
        if (crosshairTex == null) return;

        // 화면 중앙 좌표 계산
        float x = (Screen.width - size) / 2f;
        float y = (Screen.height - size) / 2f;

        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTex);
    }
}
