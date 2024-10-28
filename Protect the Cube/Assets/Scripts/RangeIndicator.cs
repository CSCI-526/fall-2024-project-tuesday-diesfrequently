using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    [SerializeField] public MeshRenderer indicator;
    [SerializeField] public float radius = 12.0f;
    [SerializeField] public bool useTurretRange = true;
    [SerializeField] public bool alwaysShow = false;

    private void Start()
    {
        ShowIndicator();
    }

    public void SetRadius(float radius)
    {
        indicator.gameObject.transform.localScale = new Vector3(radius, indicator.gameObject.transform.localScale.y, radius);
    }    

    public void ShowIndicator()
    {
        indicator.enabled = true;
        if(GetComponent<turretShoot>() != null && useTurretRange)
        {
            SetRadius(GetComponent<turretShoot>().maxRange );
        }
        else
        {
            SetRadius(radius);
        }
    }

    public void HideIndicator()
    {
        if (!alwaysShow){
            indicator.enabled = false;

        }
    }

    private void Update()
    {
        indicator.gameObject.transform.rotation = Quaternion.identity;
    }
}
