using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    private PlayerGrapplingHook playerGrapplingHook;
    private LineRenderer lineRenderer;
    private GameObject ropeSegmentPrefab;
    private List<GameObject> ropeSegments;
    private Transform playerTransform;

    public void Initialize(PlayerGrapplingHook grapplingHook, LineRenderer lr, GameObject segmentPrefab, List<GameObject> segments, Transform player)
    {
        playerGrapplingHook = grapplingHook;
        lineRenderer = lr;
        ropeSegmentPrefab = segmentPrefab;
        ropeSegments = segments;
        playerTransform = player;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.tag != "Player")
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            // Schakel de LineRenderer uit en creëer touwsegmenten
            lineRenderer.positionCount = 0;
            CreateRopeSegmentsBetweenPoints(playerTransform.position, transform.position);
        }
    }

    void CreateRopeSegmentsBetweenPoints(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        int segmentCount = Mathf.CeilToInt(distance / 0.5f); // Aantal segmenten gebaseerd op afstand
        Vector2 direction = (end - start).normalized;

        GameObject previousSegment = null;
        for (int i = 0; i < segmentCount; i++)
        {
            Vector2 segmentPosition = start + direction * (i * 0.5f);
            GameObject segment = Instantiate(ropeSegmentPrefab, segmentPosition, Quaternion.identity);
            ropeSegments.Add(segment);

            if (i == 0)
            {
                // Verbind het eerste segment met de speler
                HingeJoint2D joint = segment.AddComponent<HingeJoint2D>();
                joint.connectedBody = playerTransform.GetComponent<Rigidbody2D>();
            }
            else
            {
                // Verbind opeenvolgende segmenten
                HingeJoint2D joint = segment.AddComponent<HingeJoint2D>();
                joint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
            }

            previousSegment = segment;
        }

        // Verbind het laatste segment met de harpoon
        if (previousSegment != null)
        {
            HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
            joint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
        }
    }
}
