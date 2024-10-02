using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtEnemy : MonoBehaviour
{
    public GameObject enemy;
    public GameObject fovStartPoint;
    public float lookSpeed = 200;
    public float maxAngle = 45;
    public float maxAngleReset = 90;
    private Quaternion lookAt;
    private Quaternion targetRotation;
 
    void Update()
    {
    
        if (EnemyInFieldOfView(fovStartPoint))
        {
            Vector3 direction = enemy.transform.position - transform.position;
 
            targetRotation = Quaternion.LookRotation(direction);
            lookAt = Quaternion.RotateTowards(
            transform.rotation, targetRotation, Time.deltaTime * lookSpeed);
            transform.rotation = lookAt;
 
        }
        else if (enemy != null && EnemyInFieldOfViewNoResetPoint(fovStartPoint))
        {
            return;
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation, targetRotation, Time.deltaTime * lookSpeed);
        }
    }
 
    bool EnemyInFieldOfView(GameObject looker)
    {
 
        Vector3 targetDir = enemy.transform.position - looker.transform.position;
 
        float angle = Vector3.Angle(targetDir, looker.transform.forward);
 
        if (angle < maxAngle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
 
    bool EnemyInFieldOfViewNoResetPoint(GameObject looker)
    {
        Vector3 targetDir = enemy.transform.position - looker.transform.position;
        float angle = Vector3.Angle(targetDir, looker.transform.forward);
 
        if (angle < maxAngleReset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
