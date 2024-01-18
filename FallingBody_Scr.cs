using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBody_Scr : MonoBehaviour
{
    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 forceVector = -transform.position;
        forceVector.y = -forceVector.y;
        rb.AddForce(forceVector * Random.Range(25f,60f));
    }

    private void Update()
    {
        if (transform.position.y <= -40)
            Destroy(gameObject);
    }
}
