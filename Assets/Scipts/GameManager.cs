using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<bool> LevelsUnlocked { get; private set; }
    public int Diamonds { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLevels();
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLevels()
    {
        int totalLevels = 10; // Pas dit aan op basis van je daadwerkelijke aantal levels
        LevelsUnlocked = new List<bool>(new bool[totalLevels]);
        LevelsUnlocked[0] = true; // Zorgt ervoor dat level 1 altijd ontgrendeld is
    }

    public void LevelCompleted(int levelIndex)
    {
        if (levelIndex < LevelsUnlocked.Count - 1)
        {
            LevelsUnlocked[levelIndex + 1] = true; // Ontgrendelt het volgende level
            SaveProgress();
        }
    }

    public void SaveProgress()
    {
        for (int i = 0; i < LevelsUnlocked.Count; i++)
        {
            PlayerPrefs.SetInt($"Level_{i}_Unlocked", LevelsUnlocked[i] ? 1 : 0);
        }
        PlayerPrefs.SetInt("Diamonds", Diamonds);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        for (int i = 0; i < LevelsUnlocked.Count; i++)
        {
            LevelsUnlocked[i] = PlayerPrefs.GetInt($"Level_{i}_Unlocked", 0) == 1;
        }
        LevelsUnlocked[0] = true; // Verzekert dat level 1 altijd ontgrendeld blijft
        Diamonds = PlayerPrefs.GetInt("Diamonds", 0);
    }
}
