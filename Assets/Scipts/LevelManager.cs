using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour
{
    void Start()
    {
        Invoke("UpdateLevelLockStatus", 0.1f); // Geeft GameManager tijd om te initialiseren
    }

    public void UpdateLevelLockStatus()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int index = i;
            var level = transform.GetChild(i);
            var button = level.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError($"Button component ontbreekt op level object {level.name}.");
                continue;
            }

            var lockSprite = level.Find("LockSprite");
            if (lockSprite == null)
            {
                Debug.LogError($"LockSprite niet gevonden op level object {level.name}.");
                continue;
            }

            bool isUnlocked = GameManager.Instance.LevelsUnlocked[index];
            lockSprite.gameObject.SetActive(!isUnlocked);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => LoadLevel(index));
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (GameManager.Instance.LevelsUnlocked[levelIndex])
        {
            Debug.Log($"Level {levelIndex} geladen.");
            // Voeg hier code toe om het
            // SceneManager.LoadScene("Level" + levelIndex); // Uncomment en pas aan
        }
        else
        {
            Debug.Log("Dit level is nog niet ontgrendeld.");
        }
    }
}
