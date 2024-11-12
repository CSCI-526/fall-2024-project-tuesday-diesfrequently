// character movement via WASD
// instantiating player bullets (via bullet pool) given timeSinceLastShot

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //movement variables
    [SerializeField] protected float speed = 5.0f;
    [SerializeField] protected float jumpForce = 5.0f;
    [SerializeField] protected Rigidbody rb;
    private UnityEngine.Vector3 direction;
    private bool isMovementLocked = true; // controls "movement" lock
    private bool isShootingLocked = true; // controls "shooting" lock

    //shooting variables
    [SerializeField] float fireRate = 5.0f;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject gunBarrel;
    [SerializeField] GameObject rangeIndicator;
    private float timeSinceLastShot = 0.0f;
    private static bool hasPlayerShot = false;



    private void Update()
    {
        // only check for movement input if movement not locked
        HandleMoveInput();

        if (Input.GetMouseButton(0)) Shoot(); // shoot on left click

        // Quit Game if cancel button pressed
        if(Input.GetButton("Cancel")) { GameManager.Instance.QuitGame(); }

        // detecting turrets
        if (gameObject.GetComponent<PlaceObject>().currentPlaceableObject == null) // only hover when not currently placing a turret
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Turret")
                {
                    GameObject currTurret = hit.collider.gameObject.transform.root.gameObject; // gets root parent me thinks
                    rangeIndicator.SetActive(true);
                    rangeIndicator.transform.position = currTurret.transform.root.position;
                    rangeIndicator.transform.localScale = new UnityEngine.Vector3(currTurret.GetComponent<turretShoot>().maxRange, rangeIndicator.transform.localScale.y, currTurret.GetComponent<turretShoot>().maxRange);
                    rangeIndicator.transform.rotation = UnityEngine.Quaternion.identity;
                }
                else
                {
                    if (rangeIndicator.activeSelf) rangeIndicator.SetActive(false);
                }
            }
        }
    }

    void FixedUpdate()
    {
        // move via WASD keys if movement not locked
        if (!isMovementLocked) rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

        // look at mouse
        LookAtMouse();

        //update shot time
        if (!isShootingLocked) timeSinceLastShot += Time.fixedDeltaTime;
    }

    //private void Shoot()
    //{
    //    if (!isShootingLocked)
    //    {
    //        hasPlayerShot = true; // boolean flag for the first time
    //        Debug.Log("hasPlayerShot is TRUE");
    //        if ((timeSinceLastShot > 1 / fireRate) && projectile && gunBarrel)
    //        {
    //            if (GameManager.Instance.useBulletPool)
    //            {
    //                var bullet = BulletPool.Instance.GetBullet();

    //                if (bullet == null)
    //                {
    //                    Debug.Log("All Bullets are Currently Being Used");
    //                    return; // return early to indicate "stop shooting"
    //                }
    //                bullet.transform.position = gunBarrel.transform.position;
    //                bullet.transform.rotation = gunBarrel.transform.rotation;
    //                timeSinceLastShot = 0;
    //            }
    //            else
    //            {
    //                var bullet = Instantiate(projectile, gunBarrel.transform.position, gunBarrel.transform.rotation);
    //                bullet.transform.position = gunBarrel.transform.position;
    //                bullet.transform.rotation = gunBarrel.transform.rotation;
    //                timeSinceLastShot = 0;
    //            }
    //        }
    //    }
        
    //}

    private void Shoot()
    {
        if (!isShootingLocked)
        {
            hasPlayerShot = true; // boolean flag for the first time
            //Debug.Log("hasPlayerShot is TRUE");

            // Check if projectile and gunBarrel are assigned
            if (projectile == null || gunBarrel == null)
            {
                Debug.LogError("Projectile or Gun Barrel not assigned!");
                return;
            }

            if (timeSinceLastShot > 1 / fireRate)
            {
                if (GameManager.Instance.useBulletPool)
                {
                    if (BulletPool.Instance == null)
                    {
                        Debug.LogError("Bullet Pool is not initialized!");
                        return;
                    }

                    var bullet = BulletPool.Instance.GetBullet();

                    if (bullet == null)
                    {
                        Debug.Log("All Bullets are Currently Being Used");
                        return; // return early to indicate "stop shooting"
                    }

                    bullet.transform.position = gunBarrel.transform.position;
                    bullet.transform.rotation = gunBarrel.transform.rotation;
                    timeSinceLastShot = 0;
                }
                else
                {
                    var bullet = Instantiate(projectile, gunBarrel.transform.position, gunBarrel.transform.rotation);
                    bullet.transform.position = gunBarrel.transform.position;
                    bullet.transform.rotation = gunBarrel.transform.rotation;
                    timeSinceLastShot = 0;
                }
            }
        } else { timeSinceLastShot = 0; }
    }


    void LookAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            UnityEngine.Vector3 target = hit.point - transform.position;
            target.y = 0;
            transform.rotation = UnityEngine.Quaternion.LookRotation(target);
        }
    }

    void HandleMoveInput()
    {
        UnityEngine.Vector3 input = new UnityEngine.Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        UnityEngine.Vector3 cameraForward = Camera.main.transform.forward;
        UnityEngine.Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Rotate the input direction based on camera's orientation
        direction = (cameraForward * input.z + cameraRight * input.x).normalized;
    }

    // lock the player movement
    public void LockMovement()
    {
        Debug.Log("[PlayerController] isMovementLocked = true");
        isMovementLocked = true;
        direction = UnityEngine.Vector3.zero; // Reset direction to prevent continued movement
    }

    // unlock the player movement
    public void UnlockMovement() { Debug.Log("[PlayerController] isMovementLocked = false");  isMovementLocked = false; }

    // lock the player movement
    public void LockShooting() { Debug.Log("[PlayerController] isShootingLocked = true"); isShootingLocked = true; }

    // unlock the player movement
    public void UnlockShooting() { Debug.Log("[PlayerController] isShootingLocked = false"); isShootingLocked = false; }

    // check if player has pressed a movement key
    public static bool HasPressedMovementKeys()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    // check if player has "shot" yet
    public static bool HasShotOnce() { return hasPlayerShot; }
}
