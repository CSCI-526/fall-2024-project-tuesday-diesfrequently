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

        void HandleNullTarget()
        {
            Debug.LogWarning("[Camera Follow - CoRoutine] CAMERA: Target became null during transition. Resetting to player.");

            currTarget = target; // Reset to Player
            isCameraTransitioning = false; // Resume regular updates
            onComplete?.Invoke(); // reset
        }

        if (new_target == null) { HandleNullTarget(); yield break; }

        float elapsed = 0.0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new_target.transform.position + offset;

        while (elapsed < transition_time)
        {
            if (new_target == null) { HandleNullTarget(); yield break; }

            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / transition_time);
            yield return null; // Wait for the next frame
        }

        if (new_target != null)
        {
            // Ensure it's exactly on the target after the transition
            transform.position = new_target.transform.position + offset;
            currTarget = new_target;
        }
        else { HandleNullTarget(); yield break; }

        isCameraTransitioning = false; // Resume normal updates
        if (GameManager.Instance.DEBUG_CAMERA_FOLLOW) Debug.Log("[Camera Follow - CoRoutine] CAMERA: Transition complete!");

        onComplete?.Invoke(); // callback
    }
}
