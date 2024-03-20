using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelManager : MonoBehaviour
{
    void Start()
    {
        UpdateLevelLockStatus();
    }

    void UpdateLevelLockStatus()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int index = i; // Kopieer de index naar een lokale variabele om te gebruiken in de lambda expressie
            var level = transform.GetChild(i);
            Button levelButton = level.gameObject.GetComponent<Button>();
            levelButton.onClick.AddListener(() => LoadLevel(index));

            bool isUnlocked = GameManager.Instance.LevelsUnlocked[index];
            level.Find("LockSprite").gameObject.SetActive(!isUnlocked);

            // Schakel de interactie van de knop in of uit afhankelijk van de ontgrendelstatus
            levelButton.interactable = isUnlocked;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (GameManager.Instance.LevelsUnlocked[levelIndex])
        {
            Debug.Log($"Level {levelIndex} geladen.");
            // SceneManager.LoadScene($"Level{levelIndex}"); // Voeg de juiste scene naam toe
        }
        else
        {
            Debug.Log("Dit level is nog niet ontgrendeld.");
        }
    }
}
