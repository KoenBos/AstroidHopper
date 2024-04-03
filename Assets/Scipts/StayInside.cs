using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StayInside : MonoBehaviour
{
    public Transform minimapCenter; // Het dynamische middelpunt van de minimap, meestal de speler.
    public float minimapRadius = 50f; // De radius van de cirkelvormige minimap in wereldafmetingen.
    private Vector3 initialTargetPosition; // De oorspronkelijke wereldpositie van het icoon.

    void Start()
    {
        // Sla de oorspronkelijke wereldpositie van het icoon op.
        initialTargetPosition = transform.position;
    }

    void Update()
    {
        // Bereken de offset van het icoon tot het centrum van de minimap.
        Vector3 offsetFromCenter = initialTargetPosition - minimapCenter.position;
        
        // Als de afstand van het icoon tot het minimap centrum groter is dan de minimap radius,
        // plaats dan het icoon aan de rand van de minimap.
        if (offsetFromCenter.magnitude > minimapRadius)
        {
            // Bereken een nieuwe positie op de rand van de minimap.
            Vector3 onEdgePosition = minimapCenter.position + (offsetFromCenter.normalized * minimapRadius);
            transform.position = new Vector3(onEdgePosition.x, onEdgePosition.y, transform.position.z);
        }
        else
        {
            // Wanneer binnen de minimap radius, reset het icoon naar zijn oorspronkelijke positie.
            transform.position = initialTargetPosition;
        }
    }
}
