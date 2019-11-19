using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;

public class OptionsMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle fullScreenToggle;


    private int screenInt;
    private bool isFullScreen = false;
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
            isFullScreen = true;
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
        volumeSlider.value = PlayerPrefs.GetFloat("MVolume", 1f);
        audioMixer.SetFloat("volume", logarithmicVolume);
        qualityDropdown.value = PlayerPrefs.GetInt(qualityValue, 2);

        resolutions = Screen.resolutions;
        
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
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
        Debug.Log(volume);
        audioMixer.SetFloat("volume", logarithmicVolume);
        Debug.Log(logarithmicVolume);
        
    }

    public void SetQuality(int qualityIndex)
    {
            QualitySettings.SetQualityLevel(qualityIndex);
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
            isFullScreen = true;
            PlayerPrefs.SetInt("togglestate", 1);
        }

    }

}