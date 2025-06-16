// DraggedIcon.cs
using UnityEngine;
using UnityEngine.UI;

public class DraggedIcon : MonoBehaviour
{
    public Image iconImage;

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
