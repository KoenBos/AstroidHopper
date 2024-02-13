using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    public float gravityScale = 1.0f;
    public float planetRadius = 5.0f; 
    public float gravityMaxRange = 10.0f;
    public float gravityFallOffStrength = 1.0f;
    public bool isRotating = false;
    public float rotationSpeed = 1.0f;

    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerStay2D(Collider2D obj)
    {
        float dist = Vector2.Distance(obj.transform.position, transform.position);
        float gravitationalPower = CalculateGravitationalPower(dist);

        Vector2 direction = (transform.position - obj.transform.position).normalized * gravitationalPower;
        obj.GetComponent<Rigidbody2D>().AddForce(direction);

        //if rotate, rotate everything in the trigger with the planet / asteroid
        if (isRotating)
        {
            obj.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        // Rotate the player towards the planet
        if (obj.CompareTag("Player"))
        {
            RotatePlayer(obj, direction, dist);
        }
    }

    float CalculateGravitationalPower(float dist)
    {
        if (dist > planetRadius)
        {
            //gravity with inverse square law with a FallOffStrength
            float effectiveDistance = Mathf.Max(dist - planetRadius, 1f); 
            return gravityScale / Mathf.Pow(effectiveDistance, gravityFallOffStrength);
        }
        else
        {
            return gravityScale;
        }
    }

    void RotatePlayer(Collider2D player, Vector2 gravityDirection, float dist)
    {
        // Rotate player towards the planet if close to the surface
        if (dist < planetRadius + 0.8f)
        {
            player.transform.up = Vector3.MoveTowards(player.transform.up, -gravityDirection.normalized, 5 * Time.deltaTime);
        }
    }
}
