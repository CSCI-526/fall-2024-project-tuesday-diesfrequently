using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RangedEnemyMove : EnemyMove
{
    [SerializeField] protected float _range;
    [SerializeField] protected float _shotInterval;
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected Transform _muzzle;
    private float _timeSinceLastShot = 0.0f;

    void FixedUpdate()
    {
        _timeSinceLastShot += Time.fixedDeltaTime;

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

        Vector3 dirToTarget = _target.transform.position - _rb.transform.position;
        dirToTarget.y = 0.0f;
        dirToTarget.Normalize();

        _rb.MovePosition(transform.position + dirToTarget * (moveSpeed * (1 - slowAmount)) * Time.fixedDeltaTime);

        Vector3 toPlayer = GameManager.Instance.Player.transform.position - _rb.transform.position;
        toPlayer.y = 0.0f;

        if(toPlayer.magnitude <= _range) //look and shoot at player if in range
        {
            transform.rotation = UnityEngine.Quaternion.LookRotation(toPlayer, Vector3.up);

            if(_timeSinceLastShot > _shotInterval)  //shoot at player
            {
                Instantiate(_projectilePrefab, _muzzle.position, _muzzle.rotation);
                _timeSinceLastShot = 0.0f;
            }
        }
        else
        {
            transform.rotation = UnityEngine.Quaternion.LookRotation(dirToTarget, Vector3.up);
        }
    }
}
