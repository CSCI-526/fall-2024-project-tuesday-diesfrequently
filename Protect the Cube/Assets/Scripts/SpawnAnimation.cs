using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnAnimation : MonoBehaviour
{
    public bool enableAnimation = false;
    public Vector3 startPosition;
    public Vector3 endPosition;
    [SerializeField] float duration = 2.0f;
    public Vector3 offset = new Vector3(0.0f, -5.0f, 0.0f);

    private float t = 0.0f;
    private bool isNexusAtFinalPosition;

    // Start is called before the first frame update
    private void Awake()
    {
        isNexusAtFinalPosition = false;
        endPosition = transform.position;
        startPosition = endPosition + offset;
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableAnimation)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, t / duration);
        }

        if (endPosition == transform.position) { isNexusAtFinalPosition = true; enableAnimation = false;  }
    }

    public void TriggerSpawnSequence() { enableAnimation = true; }
    public bool isNexusInSpawnPos() { return isNexusAtFinalPosition;  }
    public void ResetIsNexusInSpawnPos() { isNexusAtFinalPosition = false;  }
}
