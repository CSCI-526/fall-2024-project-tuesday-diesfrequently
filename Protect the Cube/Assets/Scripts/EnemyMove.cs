using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMove : MonoBehaviour
{
    // Start is called before the first frame update
    protected GameObject _target;
    protected Rigidbody _rb;

    protected List<GameObject> targetList = new List<GameObject>();

    public float moveSpeed = 0.1f;

    protected float slowDebufTimer = 0;
    protected float slowAmount = 0;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected bool targetPlayerOnly = false;
    [SerializeField] protected bool targetBuildingsOnly = false;

    private bool isEnemyMovementLocked = false; // controls enemy "movement" lock

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        // SetTarget(GameManager.Instance.Nexus);
        UpdateTargetList();
    }

    protected void UpdateTargetList()
    {
        if(!targetPlayerOnly)
        {
            targetList = GameObject.FindGameObjectsWithTag("Nexus").ToList();
        }
        if(!targetBuildingsOnly)
        {
            targetList.Add(GameObject.FindWithTag("Player"));
        }
    }

    // Update is called once per frame
    void Update()
    {   
        SetTarget(targetList);
        // Debug.Log("target list: "+_target.name);
        slowDebufTimer -= Time.deltaTime;
        if (slowDebufTimer <= 0){
            slowAmount = 0;
        }

        if (transform.position.y < -5)
        {
            Destroy(gameObject);
        }

    }

    void FixedUpdate()
    {
        if (!_target)
        {
            SetTarget(targetList);
            // Debug.Log("target: "+_target.name);
        }
        if (_target.IsDestroyed() || !_target.activeSelf)
        {
            targetList.Remove(_target);
            SetTarget(targetList);
        }

        // does not move if enemy movement is locked
        if (!isEnemyMovementLocked)
        {
            Vector3 dirToTarget = _target.transform.position - _rb.transform.position;
            dirToTarget.y = 0.0f;
            dirToTarget.Normalize();

            _rb.MovePosition(transform.position + dirToTarget * (moveSpeed * (1 - slowAmount)) * Time.fixedDeltaTime);
            transform.rotation = UnityEngine.Quaternion.LookRotation(dirToTarget, Vector3.up);
        }
        
    }

    protected void SetTarget(List<GameObject> targetList)
    {   
        float closest = Int32.MaxValue; //add your max range here
        GameObject closestObject = null;
        for (int i = 0; i < targetList.Count(); i++)  //list of gameObjects to search through
        {
            if (targetList[i] && !targetList[i].IsDestroyed())
            {
                float dist = Vector3.Distance(targetList[i].transform.position, transform.position);
                if (dist < closest)
                {
                    closest = dist;
                    closestObject = targetList[i];
                }
            }
        }
        // Debug.Log("closest: "+closestObject.name);
        _target = closestObject; 
    }

    private void OnCollisionStay(Collision collision)
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

    // lock the enemy movement
    public void LockMovement() { Debug.Log("[EnemyMove] isEnemyMovementLocked = true"); isEnemyMovementLocked = true; }

    // unlock the enemy movement
    public void UnlockMovement() { Debug.Log("[EnemyMove] isEnemyMovementLocked = false"); isEnemyMovementLocked = false; }
}
