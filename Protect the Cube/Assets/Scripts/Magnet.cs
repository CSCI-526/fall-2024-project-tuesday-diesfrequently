using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] protected float period = 1.0f;
    [SerializeField] protected float amplitude = 1.0f;
    [SerializeField] protected float lifetime = 20.0f;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"){
            suckExp();
            Destroy(gameObject);
            
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
    private void suckExp(){
                // Find all game objects with the "ExperienceOrb" and "GoldOrb" tags
        GameObject[] experienceOrbs = GameObject.FindGameObjectsWithTag("ExperienceOrb");
        GameObject[] goldOrbs = GameObject.FindGameObjectsWithTag("GoldOrb");

        // Combine the two arrays into a list
        List<GameObject> allOrbs = new List<GameObject>();
        allOrbs.AddRange(experienceOrbs);
        allOrbs.AddRange(goldOrbs);

        // Loop over each object and call StartMoveToPlayer on its ExperiencePickup component
        foreach (GameObject experienceObject in allOrbs)
        {
            ExperiencePickup pickup = experienceObject.GetComponent<ExperiencePickup>();

            if (pickup != null)
            {
                Debug.Log("moving EXP");
                pickup.StartMoveToPlayer();
                pickup.StartMoveToEXPBar();
            }
            else
            {
                Debug.LogWarning("ExperiencePickup component not found on " + experienceObject.name);
            }
        }
    }
}
