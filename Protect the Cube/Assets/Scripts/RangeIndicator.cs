using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    [SerializeField] public MeshRenderer indicator;
    [SerializeField] public float radius = 12.0f;

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
        if(GetComponent<turretShoot>() != null )
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
        indicator.enabled = false;
    }
}
