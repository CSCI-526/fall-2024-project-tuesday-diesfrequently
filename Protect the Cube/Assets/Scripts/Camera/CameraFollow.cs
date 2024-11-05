using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] protected GameObject target;
    [SerializeField] protected float lerpSpeed;
    [SerializeField] protected Vector3 offset = new Vector3(-5.0f,10.0f,-5.0f);

    private void Start()
    {
        offset = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 destination = target.transform.position + offset;
        Vector3 smoothDestination = Vector3.Lerp(transform.position, destination, lerpSpeed);
        transform.position = smoothDestination;

        //transform.LookAt(target.transform.position);
    }
}
