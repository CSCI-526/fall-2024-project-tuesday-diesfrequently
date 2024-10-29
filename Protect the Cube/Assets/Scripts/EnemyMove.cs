using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// identifies closest target from a list of targets, maintains reference via _target
// SetTarget finds closest target based on distacne
// FixedUpdate moves enemy towards target while accounting for any slowing effects (from slow tower)
// OnCollisionEnter defined how enemy collistions are handled
// - [Nexus] inflict dmg to nexus + EnemyHealth.die()
// - [Player] inflict dmg to player
// Slow Effect Management via GetSlowed() and slowDebuffTimer
// Update() will update the target + check for enemy positioning, destroying if it falls below a certain Y threshold

public class EnemyMove : MonoBehaviour
{
    private GameObject _target;
    private Rigidbody _rb;

    private List<GameObject> targetList = new List<GameObject>();

    public float moveSpeed = 0.1f;
    private float slowDebufTimer = 0;
    private float slowAmount = 0;
    [SerializeField] int damage = 1;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        SetTarget(GameManager.Instance.WaveManager.GetTargetList());
        //UpdateTargetList();
    }

    private void UpdateTargetList()
    {
        targetList = GameObject.FindGameObjectsWithTag("Nexus").ToList();
        targetList.Add(GameObject.FindWithTag("Player"));

    }

    // Update is called once per frame
    void Update()
    {   
        SetTarget(GameManager.Instance.WaveManager.GetTargetList());
        slowDebufTimer -= Time.deltaTime;
        if (slowDebufTimer <= 0) slowAmount = 0;
        if (transform.position.y < -5) Destroy(gameObject);

    }

    void FixedUpdate()
    {
        //if (_target.IsDestroyed() || !_target.activeSelf)
        //{
        //    targetList.Remove(_target);
        //    SetTarget(targetList);
        //}

        if (_target == null) SetTarget(GameManager.Instance.WaveManager.GetTargetList());
        Vector3 dirToTarget = (_target.transform.position - _rb.transform.position).normalized;
        dirToTarget.y = 0.0f;
        _rb.MovePosition(transform.position + dirToTarget * (moveSpeed * (1 - slowAmount)) * Time.fixedDeltaTime);
        transform.rotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
    }

    private void SetTarget(List<GameObject> targetList)
    {   
        float closest = Mathf.Infinity; // max range
        GameObject closestObject = null;

        foreach (var target in targetList) // list of gameObjects to search through
        {
            if (target == null) continue;
            float dist = Vector3.Distance(target.transform.position, transform.position);

            if (dist < closest)
            {
                closest = dist;
                closestObject = target;
            }
        }
        _target = closestObject; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Nexus"))
        {
            collision.gameObject.GetComponent<Nexus>().TakeDamage(damage);
            GetComponent<EnemyHealth>().Die();
        }
        else if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
        }
    }

    public void GetSlowed(float slowRate){

        slowAmount = Mathf.Max(slowAmount, slowRate);
        slowDebufTimer = 0.1f;
    }
}
