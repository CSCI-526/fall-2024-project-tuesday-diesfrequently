using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperiencePickup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] protected int exp_value = 1;
    [SerializeField] protected float period = 1.0f;
    [SerializeField] protected float amplitude = 1.0f;
    [SerializeField] protected float lifetime = 20.0f;

    [SerializeField] protected float moveSpeed = 8.0f;

    private bool magnetMoveToPlayer = false;
    [SerializeField] public float attractionRange = 4.0f;
    [SerializeField] private GameObject uiExpOrbPrefab;

    private Transform canvasTransform;
    private float counter = 0;
    float dir = 1.0f;

    void Start()
    {
        StartCoroutine(Countdown());
        canvasTransform = GameObject.Find("UI").transform;
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if(counter > period)
        {
            counter = 0;
            dir *= -1.0f;
        }
        transform.position = transform.position + new Vector3(0, dir * amplitude * Time.deltaTime, 0);
        MoveToPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"){
            other.GetComponent<PlayerLevels>().add_exp(exp_value, gameObject);
            Destroy(gameObject);
            // CreateUIOrbAtCenter();
        }
    }

    private void CreateUIOrbAtCenter()
    {
        GameObject uiOrb = Instantiate(uiExpOrbPrefab, canvasTransform);
        RectTransform uiOrbRect = uiOrb.GetComponent<RectTransform>();

        // center it on the Canvas
        uiOrbRect.anchoredPosition = Vector2.zero;

        // Start the coroutine to move the UI orb to the experience bar
        StartCoroutine(MoveUIOrbToExpBar(uiOrbRect));
    }

private IEnumerator MoveUIOrbToExpBar(RectTransform uiOrbRect)
{
    Vector3 worldStartPosition = uiOrbRect.position;
    RectTransform expBarTarget = GameObject.Find("UI/EXP").GetComponent<RectTransform>();
    Vector3 worldTargetPosition = expBarTarget.position;

    if (expBarTarget == null)
    {
        Debug.LogError("EXP Bar target not found in UI");
        yield break;
    }

    // Move towards the experience bar target
    while (Vector3.Distance(worldStartPosition, worldTargetPosition) > 0.1f)
    {
        worldStartPosition = Vector3.Lerp(worldStartPosition, worldTargetPosition, moveSpeed * Time.deltaTime);
        uiOrbRect.position = worldStartPosition;

        Debug.Log("Current Position: " + uiOrbRect.position + " Target Position: " + worldTargetPosition);
        yield return null;
    }
    Destroy(uiOrbRect.gameObject);
}

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void StartMoveToPlayer(){
        magnetMoveToPlayer = true;
    }

    private void MoveToPlayer()
    {
        GameObject playerObject = GameManager.Instance.Player;
        Transform player = playerObject.transform;
        if (player == null)
        {
            return;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if ((magnetMoveToPlayer || distanceToPlayer <= attractionRange)  && Vector3.Distance(transform.position, player.position) >= 0.1f )
        {
            // Move the orb towards the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

}
