using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject levelPanel, upgradePanel;
    [SerializeField] private TextMeshProUGUI DiamondsText;


    private void Start()
    {
        //if levelcompleted is true, show level panel
        if (GameManager.Instance.justLevelComplete == true)
        {
            enableLevelPanel();
            GameManager.Instance.justLevelComplete = false;
        }
        //if levelfailed is true, show level panel
        if (GameManager.Instance.justLevelFailed == true)
        {
            enableLevelPanel();
            GameManager.Instance.justLevelFailed = false;
        }
        DiamondsText.text = GameManager.Instance.Diamonds.ToString();
    }

    public void enableLevelPanel()
    {
        levelPanel.SetActive(true);
    }
    public void disableLevelPanel()
    {
        levelPanel.SetActive(false);
    }
    public void enableUpgradePanel()
    {
        upgradePanel.SetActive(true);
    }
    public void enableUpgradePanelFromMain()
    {
        levelPanel.SetActive(true);
        upgradePanel.SetActive(true);
    }
    public void disableUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }

    public void quitGame()
    {
        Application.Quit();
        Debug.Log("Game quit.");
    }
}
