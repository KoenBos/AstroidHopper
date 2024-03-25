using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private bool isTimed, collectMission, surviveMission, gotoMission;
    [SerializeField] private float timeLimit, surviveTime;

    [SerializeField] private List<GameObject> checkforObjects;


    private void Start()
    {
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

    private IEnumerator TimedMission()
    {
        yield return new WaitForSeconds(timeLimit);
        Debug.Log("Mission Failed");
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
                Debug.Log("Mission Completed");
                break;
            }
            yield return null;
        }
    }

    private IEnumerator SurviveMission()
    {
        yield return new WaitForSeconds(surviveTime);
        Debug.Log("Mission Completed");
    }

    private IEnumerator GotoMission()
    {
        while (true)
        {
            bool allReached = true;
            foreach (GameObject obj in checkforObjects)
            {
                if (Vector3.Distance(obj.transform.position, transform.position) > 1)
                {
                    allReached = false;
                    break;
                }
            }
            if (allReached)
            {
                Debug.Log("Mission Completed");
                break;
            }
            yield return null;
        }
    }
}
