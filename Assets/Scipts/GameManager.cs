using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

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
        // Bijvoorbeeld: je hebt 10 levels, waarbij alleen de eerste bij start unlocked is
        LevelsUnlocked = new List<bool>() { true };
        for (int i = 1; i < 10; i++) // Pas het aantal levels aan op basis van je spel
        {
            LevelsUnlocked.Add(false);
        }
    }

    public void LevelCompleted(int levelIndex)
    {
        if (levelIndex < LevelsUnlocked.Count - 1)
        {
            LevelsUnlocked[levelIndex + 1] = true; // Ontgrendel het volgende level
        }
        SaveProgress();
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
        Diamonds = PlayerPrefs.GetInt("Diamonds", 0);
    }
}
