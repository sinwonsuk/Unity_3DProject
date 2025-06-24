using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DpiSetting : MonoBehaviour
{

    void Awake()
    {
        // ����� �ΰ��� �ҷ�����
        Sensitivity = PlayerPrefs.GetFloat(PREF_KEY, 5f);

        // �����̴� �ʱ�ȭ
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.1f;
            sensitivitySlider.maxValue = 20f;
            sensitivitySlider.value = Sensitivity;

            sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
        }

        UpdateSensitivityText();
    }

    void OnSliderChanged(float value)
    {
        Sensitivity = value;
        PlayerPrefs.SetFloat(PREF_KEY, value);
        UpdateSensitivityText();
    }

    void UpdateSensitivityText()
    {
        if (sensitivityText != null)
            sensitivityText.text = $"�ΰ���: {Sensitivity:F1}";
    }

    public Slider sensitivitySlider;
    public TMP_Text sensitivityText;

    public static float Sensitivity { get; private set; } = 5f;

    private const string PREF_KEY = "MouseSensitivity";
}
