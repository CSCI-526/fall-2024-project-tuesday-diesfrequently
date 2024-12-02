using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] protected GameObject target;
    private GameObject currTarget; 
    [SerializeField] protected float lerpSpeed;
    [SerializeField] protected Vector3 offset = new Vector3(-5.0f, 10.0f, -5.0f);
    private bool isCameraTransitioning = false; 

    private void Start() { offset = transform.position; currTarget = target;  }

    void FixedUpdate()
    {
        if (!isCameraTransitioning)
        {
            if (currTarget == null) currTarget = target; // reset to player if other targets disappear
            Vector3 destination = currTarget.transform.position + offset;
            Vector3 smoothDestination = Vector3.Lerp(transform.position, destination, lerpSpeed);
            transform.position = smoothDestination;
        }
    }

    public void SetNewTarget(GameObject new_target, float transition_time, System.Action onComplete)
    {
        if (GameManager.Instance.DEBUG_CAMERA_FOLLOW) Debug.Log("[Follow Camera - SetNewTarget] CAMERA: Starting transition to new target...");
        StartCoroutine(SetNewTargetCoroutine(new_target, transition_time, onComplete));
    }

    public IEnumerator SetNewTargetCoroutine(GameObject new_target, float transition_time, System.Action onComplete)
    {
        if (GameManager.Instance.DEBUG_CAMERA_FOLLOW) Debug.Log("[Camera Follow - CoRoutine] CAMERA: Transition started!");
        isCameraTransitioning = true; // Lock on regular update

        float elapsed = 0.0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new_target.transform.position + offset;

        while (elapsed < transition_time)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / transition_time);
            yield return null; // Wait for the next frame
        }

        // Ensure it's exactly on the target after the transition
        transform.position = new_target.transform.position + offset;
        currTarget = new_target;

        isCameraTransitioning = false; // Resume normal updates
        if (GameManager.Instance.DEBUG_CAMERA_FOLLOW) Debug.Log("[Camera Follow - CoRoutine] CAMERA: Transition complete!");

        onComplete?.Invoke(); // callback
    }
}
