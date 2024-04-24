using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootController : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 1400f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        rb.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);
    }
}
