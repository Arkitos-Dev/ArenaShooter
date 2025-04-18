using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public PlayerMovement playerMov;
    public Weapon weapon;
    
    public void Restart()
    {
        SceneManager.LoadScene("ArenaLevel01");
        Time.timeScale = 1;
        playerMov.enabled = true;
        weapon.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
