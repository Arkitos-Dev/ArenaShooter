using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image circle;
    [SerializeField] private float speed;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject Interface;
    [SerializeField] private GameObject settingsUI;
    
    public PlayerCam playerCam;
    public ViewBobbing viewBobbing;
    public SettingsMenu settings;
    
    
    private float currentValue;
    private bool isPaused = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (currentValue < 100)
        {
            currentValue += speed * Time.deltaTime;
        }
        
        
    }
    
    void Update()
    {
        // Check for the pause key press (default is "Escape" key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(false);
        Interface.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Interface.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
    
}
