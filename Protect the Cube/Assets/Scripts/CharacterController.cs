using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterController : MonoBehaviour
{
    //movement variables
    [SerializeField] protected float speed = 5.0f;
    [SerializeField] protected float jumpForce = 5.0f;
    [SerializeField] protected Rigidbody rb;
    private UnityEngine.Vector3 direction;

    //shooting variables
    [SerializeField] float fireRate = 5.0f;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject gunBarrel;
    [SerializeField] GameObject rangeIndicator;
    private float timeSinceLastShot = 0.0f;


    private void Update()
    {
        //movement
        HandleMoveInput();

        //shoot on left(click)
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
        // if (Input.GetButtonDown("Jump"))
        // {
        //     Jump();
        // }
        if(Input.GetButton("Cancel"))
        {
            GameManager.Instance.QuitGame();
        }

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
                    if (rangeIndicator.activeSelf)
                    {
                        rangeIndicator.SetActive(false);
                    }
                }
            }
        }
    }



    void FixedUpdate()
    {
        //move using WASD/arrows
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

        //look at mouse
        LookAtMouse();

        //update shot time
        timeSinceLastShot += Time.fixedDeltaTime;
    }
    // private void Jump()
    // {
    //     if (transform.position.y < 1.01)
    //     {
    //         rb.AddForce(new UnityEngine.Vector3(0.0f, jumpForce, 0.0f));
    //     }
    // }

    private void Shoot()
    {
        if ((timeSinceLastShot > 1 / fireRate) && projectile && gunBarrel)
        {

            var bullet = BulletPool.Instance.GetBullet();

            //var bullet = Instantiate(projectile, gunBarrel.transform.position, gunBarrel.transform.rotation);
            if (bullet == null)
            {
                Debug.Log("All Bullets are Currently Being Used");
                //Time.timeScale = 0; // pauses the game
                return; // return early to indicate "stop shooting"
            }
            bullet.transform.position = gunBarrel.transform.position;
            bullet.transform.rotation = gunBarrel.transform.rotation;

            timeSinceLastShot = 0;
        }
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
}
