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

    private bool isMoveToPlayer = false; 

    private float counter = 0;
    float dir = 1.0f;

    void Start()
    {
        StartCoroutine(Countdown());
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
            
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public void StartMoveToPlayer(){
        isMoveToPlayer= true;
    }

    private void MoveToPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Transform player = playerObject.transform;
        if (isMoveToPlayer && player != null)
        {
            // Move the orb towards the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Optionally, stop moving when very close to the player
            if (Vector3.Distance(transform.position, player.position) < 0.1f)
            {
                isMoveToPlayer = false;  // Stop moving when very close
            }
        }
    }
}
