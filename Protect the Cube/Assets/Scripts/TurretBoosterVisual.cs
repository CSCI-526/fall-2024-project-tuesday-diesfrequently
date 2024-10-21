using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBoosterVisual : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject turretParent = other.gameObject.transform.root.gameObject;
        if (turretParent.CompareTag("Turret"))
        {
            ParticleSystem particle = turretParent.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject turretParent = other.gameObject.transform.root.gameObject;
        if (turretParent.CompareTag("Turret"))
        {
            ParticleSystem particle = turretParent.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Stop();
                particle.Clear();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject turretParent = other.gameObject.transform.root.gameObject;
        if (turretParent.CompareTag("Turret"))
        {
            ParticleSystem particle = turretParent.GetComponent<ParticleSystem>();
            if (particle != null && !particle.isPlaying)
            {
                particle.Play();
            }
        }
    }
}
