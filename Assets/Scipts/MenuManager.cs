using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject levelPanel;

    public void enableLevelPanel()
    {
        levelPanel.SetActive(true);
    }
    public void disableLevelPanel()
    {
        levelPanel.SetActive(false);
    }

    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Game quit.");
    }
}
