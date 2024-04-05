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
    [SerializeField] private TextMeshProUGUI TimerText, TimerTitleText, MissionTitleText, MissionDescription, livesText, LoseReasonText, pauseMenuInfoText, collectedDiamondsText;
    [SerializeField] private GameObject InfoTextPanel, TimePanel, failedPanel, completedPanel,  PauseManager, LevelContinueButton;
    [SerializeField] private string music;
    private GameObject player;
    private int maxLives = 3;
    private bool removingLife = false, alreadyFailed = false;

    private void Start()
    {
        pauseMenuInfoText.text = MissionDescription.text;
        livesText.text = maxLives.ToString();
        InfoTextPanel.SetActive(true);
        //StartCoroutine(HideMissionText());
        player = GameObject.FindGameObjectWithTag("Player");
        if (isTimed)
        {
            MissionTitleText.text = "Timed Mission";
            StartCoroutine(TimedMission());
        }
        if (collectMission)
        {
            MissionTitleText.text = "Collect Mission";
            StartCoroutine(CollectMission());
        }
        if (surviveMission)
        {
            MissionTitleText.text = "Survive Mission";
            StartCoroutine(SurviveMission());
        }
        if (gotoMission)
        {
            MissionTitleText.text = "Goto Mission";
            StartCoroutine(GotoMission());
        }
        AudioManager.Instance.PlayMusic(music, 1.0f);
    }
    public void StartLevel()
    {
        LevelStarted = true;
        //play music
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
        GameManager.Instance.CollectedDiamonds = 0;
        alreadyFailed = true;
        PauseManager.GetComponent<PauseManager>().canBePaused = false;
        yield return StartCoroutine(slowTimeToZero());
        AudioManager.Instance.StopMusic();

        Debug.Log("Mission Failed");
        failedPanel.SetActive(true);
    }
    private IEnumerator CompletedMission()
    {
        if (alreadyFailed)
        {
            yield break;
        }
        levelCompleted = true;
        AudioManager.Instance.StopMusic();
        player.GetComponent<Player>().makeInvisible(9999);
        PauseManager.GetComponent<PauseManager>().canBePaused = false;
        
        completedPanel.SetActive(true);
       
       yield return new WaitForSeconds(0.2f);

        int diamondsCollected = GameManager.Instance.CollectedDiamonds;
        int diamondsAdded = 0;
        while (diamondsCollected > diamondsAdded)
        {
            diamondsAdded++;
            collectedDiamondsText.text = diamondsAdded.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(slowTimeToZero());

        //slowy show level continue button by changing the opacity
        LevelContinueButton.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            LevelContinueButton.GetComponent<CanvasGroup>().alpha = t;
            t += Time.unscaledDeltaTime * 0.8f;
            yield return null;
        }



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
        LoseReasonText.text = "Time's up...";
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
                LoseReasonText.text = "You died...";
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
        if (player.GetComponent<Player>().isAlive == false && !removingLife && !surviveMission)
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
            LoseReasonText.text = "Out of lives...";
            FailedMission();
            yield break;
        }
        yield return new WaitForSeconds(5.2f);
        removingLife = false;
    }


}
