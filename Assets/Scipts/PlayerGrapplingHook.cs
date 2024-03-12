using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapplingHook : MonoBehaviour
{
    public GameObject harpoonPrefab;
    public GameObject ropeSegmentPrefab;
    public LineRenderer lineRenderer;
    private GameObject currentHarpoon;
    private List<GameObject> ropeSegments = new List<GameObject>();
    private bool harpoonFired = false;

    void Start()
    {
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (harpoonFired)
            {
                ReleaseGrapplingHook();
            }
            else
            {
                FireHarpoon(-transform.up); // Pas de richting aan afhankelijk van de speleroriëntatie
                harpoonFired = true;
            }
        }

        if (currentHarpoon != null)
        {
            lineRenderer.SetPositions(new Vector3[] { transform.position, currentHarpoon.transform.position });
        }
    }

    void FireHarpoon(Vector2 direction)
    {
        lineRenderer.positionCount = 2;
        Vector2 spawnPosition = transform.position;
        currentHarpoon = Instantiate(harpoonPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D harpoonRb = currentHarpoon.GetComponent<Rigidbody2D>();
        harpoonRb.velocity = direction * 10;

        // Zet het Harpoon script om te detecteren wanneer deze iets raakt en touwsegmenten te creëren
        Harpoon harpoonScript = currentHarpoon.AddComponent<Harpoon>();
        harpoonScript.Initialize(this, lineRenderer, ropeSegmentPrefab, ropeSegments, transform);
    }

    public void ReleaseGrapplingHook()
    {
        // Verwijder alle touwsegmenten
        foreach (GameObject segment in ropeSegments)
        {
            Destroy(segment);
        }
        ropeSegments.Clear();

        // Verwijder de harpoon en reset de LineRenderer
        Destroy(currentHarpoon);
        currentHarpoon = null;
        lineRenderer.positionCount = 0;
        harpoonFired = false;
    }
}
