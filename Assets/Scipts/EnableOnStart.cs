using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStart : MonoBehaviour
{
    [SerializeField] private GameObject otherObject;
    [SerializeField] private bool enableOtherObject = false;
    [SerializeField] private bool enableSprite = false;

    void Start()
    {
        if (enableOtherObject)
        {
            otherObject.SetActive(true);
        }
        if (enableSprite)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
