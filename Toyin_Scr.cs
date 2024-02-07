using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toyin_Scr : MonoBehaviour
{
    public float magnitude1 = 10;
    public float magnitude2 = 10;
    public float speed1 = 5;
    public float speed2 = 5;
    Vector3 originalPos;
    private void Start()
    {
        originalPos = transform.position;
    }
    void Update()
    {
        //float newY = magnitude2 * Mathf.Sin(magnitude1 * Mathf.Sin(Time.time * speed));
        //float newY = magnitude2 * Mathf.Sin(Time.time * speed1) + magnitude1 * Mathf.Sin(Time.time * speed2);
        float newY = magnitude2 * Mathf.Sin(Time.time * speed1) * magnitude1 * Mathf.Sin(Time.time * speed2);
        transform.position = originalPos + new Vector3(0, newY, 0);
    }
}
