using TMPro;
using UnityEngine;

public class AutoFocusInputField : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        // ������ ������ ��Ŀ�� �ֱ�
        StartCoroutine(FocusInputFieldNextFrame());
    }

    private System.Collections.IEnumerator FocusInputFieldNextFrame()
    {
        yield return null; // �� ������ ��ٸ�
        inputField.Select();              // �ؽ�Ʈ ����
        inputField.ActivateInputField();  // ��Ŀ�� Ȱ��ȭ
    }
}
