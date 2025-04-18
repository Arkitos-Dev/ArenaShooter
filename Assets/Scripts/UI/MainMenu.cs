using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    
    public void Play()
    {
        SceneManager.LoadScene("ArenaLevel01");
        
    }

    public void Settings()
    {
        gameObject.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
