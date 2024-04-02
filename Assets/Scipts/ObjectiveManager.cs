using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private bool isTimed, collectMission, surviveMission, gotoMission, levelCompleted, LevelStarted;
    [SerializeField] private float timeLimit, surviveTime;
    [SerializeField] private Transform gotoLocation;
    [SerializeField] private int levelIndex;
    [SerializeField] private List<GameObject> checkforObjects;
    [SerializeField] private TextMeshProUGUI TimerText, TimerTitleText, MissionTitleText, MissionDescription, livesText;
    [SerializeField] private GameObject InfoTextPanel, TimePanel, failedPanel, completedPanel,  PauseManager;
    private GameObject player;
    private int maxLives = 3;
    private bool removingLife = false;

    private void Start()
    {
        livesText.text = maxLives.ToString();
        InfoTextPanel.SetActive(true);
        //StartCoroutine(HideMissionText());
        player = GameObject.FindGameObjectWithTag("Player");
        if (isTimed)
        {
            StartCoroutine(TimedMission());
        }
        if (collectMission)
        {
            StartCoroutine(CollectMission());
        }
        if (surviveMission)
        {
            StartCoroutine(SurviveMission());
        }
        if (gotoMission)
        {
            StartCoroutine(GotoMission());
        }
    }
    public void StartLevel()
    {
        LevelStarted = true;
    }
    private void FailedMission()
    {
        StartCoroutine(MissionFailed());
    }
    public void completeLevelHome()
    {
        Time.timeScale = 1;
        GameManager.Instance.LevelCompleted(levelIndex - 1);
    }
    private IEnumerator MissionFailed()
    {
        PauseManager.GetComponent<PauseManager>().canBePaused = false;
        yield return StartCoroutine(slowTimeToZero());
        Debug.Log("Mission Failed");
        failedPanel.SetActive(true);
    }
    private IEnumerator CompletedMission()
    {
        player.GetComponent<Player>().makeInvisible(9999);
        PauseManager.GetComponent<PauseManager>().canBePaused = false;
        yield return StartCoroutine(slowTimeToZero());
        levelCompleted = true;
        completedPanel.SetActive(true);
        Debug.Log("Mission Completed");
        yield return null;
    }

    private IEnumerator slowTimeToZero()
    {
        float t = 0;
        while (t < 1)
        {
            Time.timeScale = Mathf.Lerp(1, 0, t);
            t += Time.unscaledDeltaTime * 0.8f;
            yield return null;
        }
    }
    private IEnumerator TimedMission()
    {
        TimePanel.SetActive(true);
        TimerTitleText.text = "Time Left";
        float timeLeft = timeLimit;
        TimerText.text = timeLimit.ToString("F1");

        //wait level start
        while (!LevelStarted)
        {
            yield return null;
        }

        while (timeLeft > 0 && !levelCompleted)
        {
            TimerText.text = timeLeft.ToString("F1");
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }
        if (!levelCompleted)
        {
        TimerText.text = "0.0";
        FailedMission();
        }
    }
    private IEnumerator HideMissionText()
    {
        yield return new WaitForSeconds(3);
        InfoTextPanel.SetActive(false);
    }
    private IEnumerator CollectMission()
    {
        while (true)
        {
            bool allCollected = true;
            foreach (GameObject obj in checkforObjects)
            {
                if (obj.activeInHierarchy)
                {
                    allCollected = false;
                    break;
                }
            }
            if (allCollected)
            {
                Debug.Log("All Collected");
                yield return StartCoroutine(GotoMission());
            }
            yield return null;
        }
    }

    private IEnumerator SurviveMission()
    {
        maxLives = 1;
        livesText.text = maxLives.ToString();
        TimePanel.SetActive(true);
        TimerTitleText.text = "Survive Time";
        float timeLeft = surviveTime;
        TimerText.text = surviveTime.ToString("F1");

        //wait level start
        while (!LevelStarted)
        {
            yield return null;
        }

        while (timeLeft > 0)
        {
            TimerText.text = timeLeft.ToString("F1");
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
            if (player.GetComponent<Player>().isAlive == false)
            {
                FailedMission();
                yield break;
            }
        }
        TimerText.text = "0.0";
        yield return StartCoroutine(CompletedMission());
    }

    private IEnumerator GotoMission()
    {
        while (Vector3.Distance(player.transform.position, gotoLocation.position) > 1)
        {
            yield return null;
        }
        yield return StartCoroutine(CompletedMission());
    }

    private void Update()
    {
        if (player.GetComponent<Player>().isAlive == false && !removingLife)
        {
            StartCoroutine(RemoveLife());
        }
    }

    private IEnumerator RemoveLife()
    {
        removingLife = true;
        maxLives--;
        livesText.text = maxLives.ToString();
        if (maxLives == 0)
        {
            FailedMission();
            yield break;
        }
        yield return new WaitForSeconds(5.2f);
        removingLife = false;
    }


}
