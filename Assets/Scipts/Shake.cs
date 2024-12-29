using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private Vector3 originalPos;
    public bool isShaking = false;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeSpeed = 1.0f;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void StartShake()
    {
        isShaking = true;
    }

    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            // Shake using Perlin noise for smooth transitions
            float shakeAmountX = Mathf.PerlinNoise(Time.time * shakeSpeed, 0) * 2 - 1;
            float shakeAmountY = Mathf.PerlinNoise(0, Time.time * shakeSpeed) * 2 - 1;
            transform.localPosition = originalPos + new Vector3(shakeAmountX, shakeAmountY, 0) * shakeIntensity;
        }
    }
}
