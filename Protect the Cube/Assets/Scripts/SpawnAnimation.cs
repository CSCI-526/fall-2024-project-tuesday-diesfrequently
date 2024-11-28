using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnAnimation : MonoBehaviour
{
    public bool enableAnimation = false;
    protected Vector3 startPosition;
    protected Vector3 endPosition;
    [SerializeField] float duration = 2.0f;
    public Vector3 offset = new Vector3(0.0f, -5.0f, 0.0f);

    private float t = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        endPosition = transform.position;
        if(enableAnimation)
        {
            startPosition = endPosition + offset;
            transform.position = startPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enableAnimation)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, t / duration);
        }
    }

    void TriggerSpawnSequence()
    {
        enableAnimation = true;
    }
}