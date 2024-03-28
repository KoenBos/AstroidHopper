using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private bool isTimed, collectMission, surviveMission, gotoMission;
    [SerializeField] private float timeLimit, surviveTime;
    [SerializeField] private Transform gotoLocation;
    [SerializeField] private int levelIndex;
    [SerializeField] private List<GameObject> checkforObjects;
    [SerializeField] private TextMeshProUGUI TimerText, TimerTitleText, MissionTitleText, MissionDescription;
    [SerializeField] private GameObject InfoTextPanel, TimePanel;
    private GameObject player;


    private void Start()
    {
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
    private void FailedMission()
    {
        Debug.Log("Mission Failed");

        
        GameManager.Instance.LevelFailed();
    }
    private IEnumerator CompletedMission()
    {
        Debug.Log("Mission Completed");
        //play star animation
        //wait for animation to finish
        GameManager.Instance.LevelCompleted(levelIndex - 1);
        yield return null;
    }
    private IEnumerator TimedMission()
    {
        TimePanel.SetActive(true);
        TimerTitleText.text = "Time Left";
        float timeLeft = timeLimit;
        while (timeLeft > 0)
        {
            TimerText.text = timeLeft.ToString("F1");
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }
        TimerText.text = "0.0";
        FailedMission();
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
        TimePanel.SetActive(true);
        TimerTitleText.text = "Survive Time";
        float timeLeft = surviveTime;
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
}
