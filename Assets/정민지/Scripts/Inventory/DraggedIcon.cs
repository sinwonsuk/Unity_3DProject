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

    public void StartDrag(Sprite icon)
    {
        iconImage.sprite = icon;
        gameObject.SetActive(true);
        transform.position = Input.mousePosition; // �巡�� ���� �� ��� ��ġ ����
    }

    public void EndDrag()
    {
        gameObject.SetActive(false);
        iconImage.sprite = null;
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

}
