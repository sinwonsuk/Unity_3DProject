using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionSetting : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle; // 생략 가능

    private Resolution[] availableResolutions;

    void Start()
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution res = availableResolutions[i];
            string option = res.width + " x " + res.height;

            if (!options.Contains(option)) // 중복 제거
                options.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;

        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        fullscreenToggle.onValueChanged.AddListener(ChangeFullscreen);
    }

    void ChangeResolution(int index)
    {
        string[] dims = resolutionDropdown.options[index].text.Split('x');
        int width = int.Parse(dims[0].Trim());
        int height = int.Parse(dims[1].Trim());

        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    void ChangeFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
