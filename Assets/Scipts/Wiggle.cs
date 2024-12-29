using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour
{
    //settings for the 2 sine waves that will be used this object so the amount of wiggle and the speed of the wiggle can be adjusted, 2 separate sine waves are used to create a more complex wiggle so one can be used for the x axis and the other for the y axis
    [SerializeField] private float wiggleAmountHorizontal = 0.1f;
    [SerializeField] private float wiggleSpeedHorizontal = 1f;
    [SerializeField] private float wiggleAmountVertical = 0.1f;
    [SerializeField] private float wiggleSpeedVertical = 1f;
    [SerializeField] private bool wiggleOnHorizontal = true;
    [SerializeField] private bool wiggleOnVertical = true;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (wiggleOnHorizontal)
        {
            //get the current position of the object
            Vector3 pos = transform.position;
            //calculate the new x position of the object using a sine wave
            pos.x = pos.x + Mathf.Sin(Time.time * wiggleSpeedHorizontal) * wiggleAmountHorizontal;
            //set the new position of the object
            transform.position = pos;
        }
        if (wiggleOnVertical)
        {
            //get the current position of the object
            Vector3 pos = transform.position;
            //calculate the new y position of the object using a sine wave
            pos.y = pos.y + Mathf.Sin(Time.time * wiggleSpeedVertical) * wiggleAmountVertical;
            //set the new position of the object
            transform.position = pos;
        }
    }
}
