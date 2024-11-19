using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Building : MonoBehaviour
{
    [SerializeField] public bool placed = false;
    [SerializeField] public bool rotatable = true;
    [SerializeField] public string buildingName = "missing name";
    [SerializeField] public string buildingDesc = "missing description";

    [SerializeField] public float colorDiffAfterPlace = 50f;
    private bool coroutineRunning = false;
    public virtual void OnPlace()
    {

        // foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        // {
        //     Color originalColor = renderer.material.color;
        //     Color color = new Color(originalColor.r + colorDiffAfterPlace / 255f, originalColor.g + colorDiffAfterPlace / 255f, originalColor.b + colorDiffAfterPlace / 255f);
        //     renderer.material.color = color;
        // }
        placed = true;
        
        foreach(Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = true;
        }


        ShowIndicators();
    }


    public virtual void Boost()
    {
        
    }

    public virtual bool CanPlace()
    {
        return true;
    }

    public void ShowIndicators(float duration = -1) //-1 will toggle permanently
    {
        RangeIndicator[] indicators = GetComponents<RangeIndicator>();
        foreach (RangeIndicator i in indicators)
        {
            i.ShowIndicator();
        }
        if(duration >= 0 && !coroutineRunning)
        {
            coroutineRunning = true;
            StartCoroutine(hideAfterDelay(duration));
        }
    }

    public void HideIndicators()
    {
        RangeIndicator[] indicators = GetComponents<RangeIndicator>();
        foreach (RangeIndicator i in indicators)
        {
            i.HideIndicator();
        }
    }

    IEnumerator hideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideIndicators();
        coroutineRunning = false;
    }
}
