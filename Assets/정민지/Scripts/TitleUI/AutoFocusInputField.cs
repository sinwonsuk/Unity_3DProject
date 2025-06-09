using TMPro;
using UnityEngine;

public class AutoFocusInputField : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        // 프레임 끝나고 포커스 주기
        StartCoroutine(FocusInputFieldNextFrame());
    }

    private System.Collections.IEnumerator FocusInputFieldNextFrame()
    {
        yield return null; // 한 프레임 기다림
        inputField.Select();              // 텍스트 선택
        inputField.ActivateInputField();  // 포커스 활성화
    }
}
