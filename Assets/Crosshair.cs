using UnityEngine;

public class CrosshairOnGUI : MonoBehaviour
{
    public Texture2D crosshairTex;
    public float size = 32f;  // �׸� ũ��

    void OnGUI()
    {
        if (crosshairTex == null) return;

        // ȭ�� �߾� ��ǥ ���
        float x = (Screen.width - size) / 2f;
        float y = (Screen.height - size) / 2f;

        GUI.DrawTexture(new Rect(x, y, size, size), crosshairTex);
    }
}
