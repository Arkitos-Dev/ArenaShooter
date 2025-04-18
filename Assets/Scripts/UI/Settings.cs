using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // Public references to your UI components
    public Slider fovSlider;
    public Slider sensitivitySlider;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullscreenToggle;
    public TMP_InputField fovInputField;
    public TMP_InputField sensitivityInputField;

    public PlayerCam playerCam;
    public Camera mainCam;
    public GameObject pauseMenu;
    public GameObject mainMenu;

    void Start()
    {
        // Initialize listeners for UI elements
        fovSlider.onValueChanged.AddListener(SetFOV);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        graphicsDropdown.onValueChanged.AddListener(SetGraphics);
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);

        // Initialize dropdown options for resolutions and load settings
        InitializeResolutionDropdown();
    }


    private void Update()
    {
        UpdatePlaceHolderText();
    }

    // Function to set the Field of View
    public void SetFOV(float fov)
    {
        mainCam.fieldOfView = fov;
    }

    // Function to set mouse sensitivity
    public void SetSensitivity(float sensitivity)
    {
        //playerCam.sens = sensitivity;
    }

    // Function to change resolution and fullscreen mode
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
    }

    // Function to adjust graphics quality level
    public void SetGraphics(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Function to toggle fullscreen mode
    public void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // Initialize the resolution dropdown with available options
    private void InitializeResolutionDropdown()
    {
        resolutionDropdown.ClearOptions(); // Clear existing options
        List<string> options = new List<string>();
        
        foreach (Resolution resolution in Screen.resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = GetCurrentResolutionIndex();
        resolutionDropdown.RefreshShownValue();
    }

    // Helper method to get the current resolution index
    private int GetCurrentResolutionIndex()
    {
        Resolution currentResolution = Screen.currentResolution;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == currentResolution.width &&
                Screen.resolutions[i].height == currentResolution.height)
            {
                return i;
            }
        }
        return 0; // Fallback to the first resolution if not found
    }

    public void UpdatePlaceHolderText()
    {
        fovInputField.placeholder.GetComponent<TextMeshProUGUI>().text = mainCam.fieldOfView.ToString("N1", CultureInfo.InvariantCulture);
        sensitivityInputField.placeholder.GetComponent<TextMeshProUGUI>().text = playerCam.sens.ToString("N1", CultureInfo.InvariantCulture);

    }

    public void GoBack()
    {
        gameObject.SetActive(false);
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
    }
    
    public void SaveSettings()
    {
        // Save FOV and sensitivity
        PlayerPrefs.SetFloat("FOV", mainCam.fieldOfView);
        //PlayerPrefs.SetFloat("Sensitivity", playerCam.sens);

        // Save resolution
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);

        // Save graphics quality and fullscreen mode
        PlayerPrefs.SetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);

        // Always call PlayerPrefs.Save() when you're done setting all your preferences
        PlayerPrefs.Save();
    }
    
    public void LoadSettings()
    {
        // Load FOV and sensitivity
        if (PlayerPrefs.HasKey("FOV"))
        {
            float fov = PlayerPrefs.GetFloat("FOV");
            mainCam.fieldOfView = fov;
            fovSlider.value = fov;
        }

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            float sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            playerCam.sens = sensitivity;
            sensitivitySlider.value = sensitivity;
        }

        // Load resolution
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = resolutionIndex;
            SetResolution(resolutionIndex);
        }

        // Load graphics quality
        if (PlayerPrefs.HasKey("GraphicsQuality"))
        {
            int graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality");
            graphicsDropdown.value = graphicsQuality;
            SetGraphics(graphicsQuality);
        }

        // Load fullscreen mode
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            bool isFullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            fullscreenToggle.isOn = isFullscreen;
            ToggleFullscreen(isFullscreen);
        }
    }

}
