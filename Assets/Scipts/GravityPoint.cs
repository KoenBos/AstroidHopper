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
            //rotate the planet / asteroid
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerStay2D(Collider2D obj)
    {
        float dist = Vector2.Distance(obj.transform.position, transform.position);
        float gravitationalPower = CalculateGravitationalPower(dist);

        Vector2 direction = (transform.position - obj.transform.position).normalized * gravitationalPower;
        obj.GetComponent<Rigidbody2D>().AddForce(direction);

        // Rotate the player towards the planet
        if (obj.CompareTag("Player") || obj.CompareTag("Alien"))
        {
            RotateAround(obj, direction, dist);

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

    void RotateAround(Collider2D rotateObject, Vector2 gravityDirection, float dist)
    {
        // Rotate player towards the planet if close to the surface
        if (dist < planetRadius + 0.8f)
        {
            rotateObject.transform.up = Vector3.MoveTowards(rotateObject.transform.up, -gravityDirection.normalized, 5 * Time.deltaTime);

            //if rotating rotate the player around the planet / asteroid
            if (isRotating)
            {
                rotateObject.transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
