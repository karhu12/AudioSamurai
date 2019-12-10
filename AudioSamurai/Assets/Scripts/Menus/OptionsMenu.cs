using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class OptionsMenu : MonoBehaviour
{
    private const string LOGIN_PREF = "login";
    private const string USERNAME_PREF = "username";

    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle fullScreenToggle;

    private int screenInt;
    private float logarithmicVolume;

    const string qualityValue = "qualityvalue";

    int dropdownValue;
    string dropdownText;
    int currentResolutionIndex;

    Resolution[] resolutions;

    void Awake()
    {
        screenInt = PlayerPrefs.GetInt("togglestate");

        if (screenInt == 1)
        {
            fullScreenToggle.isOn = true;
        } 
        
        else
        {
            fullScreenToggle.isOn = false;
        }

        qualityDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
            {
                PlayerPrefs.SetInt(qualityValue, qualityDropdown.value);
                PlayerPrefs.Save();
            }
        ));

    }

    void Start()
    {
        float volume = PlayerPrefs.GetFloat("MVolume", 1f);
        volumeSlider.value = volume;
        SetVolume(volume);

        qualityDropdown.value = PlayerPrefs.GetInt(qualityValue, 2);

        resolutions = Screen.resolutions.Distinct().ToArray();
        
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].ToString();
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

            currentResolutionIndex = PlayerPrefs.GetInt("Resolution index");

        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        dropdownValue = resolutionDropdown.value;
        dropdownText = resolutionDropdown.options[dropdownValue].text;

        PlayerPrefs.SetInt("Resolution index", dropdownValue);

    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("MVolume", volume);
        logarithmicVolume = Mathf.Log10(PlayerPrefs.GetFloat("MVolume")) * 20;
        audioMixer.SetFloat("volume", logarithmicVolume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void PlayClickSound()
    {
        FindObjectOfType<AudioManager>().Play("Click");
    }

    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        if (isFullScreen == false)
        {
            PlayerPrefs.SetInt("togglestate", 0);
        }
        
        else
        {
            PlayerPrefs.SetInt("togglestate", 1);
        }

    }

    public void OnBackButtonPress()
    {
        FindObjectOfType<AudioManager>().Play("ClickDeny");
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
    }

    public void LogOut()
    {
        LoginManager.Instance.LogOut();
    }

}