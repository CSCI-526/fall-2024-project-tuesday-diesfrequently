using UnityEngine;

public class Indicator : MonoBehaviour
{
    public float revealTime = 2f;
    public float timeLeft;
    public GameObject target;
    public float offset = 10f;

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            if(target && target.GetComponent<Nexus>() != null)
            {
                target.GetComponent<Nexus>().indicator = null;
            }
            OffScreenIndicator.Instance.ReturnIndicator(gameObject);
            return;
        }
        HandlePosition();
        //Debug.Log("Indicator Position: " + transform.position);
    }

    private void HandlePosition()
    {
        if(target != null)
        {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(target.transform.position);
            transform.localScale = Vector3.one;

            if (screenpos.z > 0 && screenpos.x > 0 && screenpos.y > 0 && screenpos.x < Screen.width && screenpos.y < Screen.height) // on screen
            {
                target.GetComponent<Nexus>().indicator = null;
                OffScreenIndicator.Instance.ReturnIndicator(gameObject);
                return;
            }

            Vector3 clampedPos = screenpos;
            clampedPos.x = Mathf.Clamp(clampedPos.x, offset, Screen.width - offset);
            clampedPos.y = Mathf.Clamp(clampedPos.y, offset, Screen.height - offset);
            clampedPos.z = 0;

            transform.position = clampedPos;
        }


        /*if (screenpos.z < 0)
        {
            screenpos *= -1;
        }*/


        /*Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;
        screenpos -= screenCenter;

        Vector3 direction = screenpos - screenCenter;

        float angle = Mathf.Atan2(screenpos.y, screenpos.x);
        angle -= 90 * Mathf.Deg2Rad;

        float cos = Mathf.Cos(angle);
        float sin = -Mathf.Sin(angle);

        screenpos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

        float m = cos / sin;
        Vector3 screenBounds = screenCenter * 0.9f;

        if (cos>0)
        {
            screenpos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
        }
        else
        {
            screenpos = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);
        }

        if (screenpos.x > screenBounds.x)
        {
            screenpos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
        }
        else if (screenpos.x < -screenBounds.x)
        {
            screenpos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
        }

        screenpos += screenCenter;

        transform.localPosition = screenpos;
        transform.localRotation = Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg);*/

    }
}
