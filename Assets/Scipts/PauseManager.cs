using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public bool canBePaused = true;
    public GameObject pausePanel;

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canBePaused)
        {
            if (GameIsPaused)
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
        AudioManager.Instance.ResumeMusic();
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        AudioManager.Instance.PauseMusic();
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameManager.Instance.LevelFailed();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
