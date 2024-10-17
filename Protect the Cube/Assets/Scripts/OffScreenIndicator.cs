using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreenIndicator : MonoBehaviour
{
    public static OffScreenIndicator Instance;

    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private int poolSize = 20; // number of indicators in ObjectPool

    private Queue<GameObject> indicatorPool = new Queue<GameObject>();


    private void Awake()
    {
        Instance = this; // static class

        for (int i = 0; i < poolSize; i++)
        {
            GameObject indicator = Instantiate(indicatorPrefab);
            indicator.transform.SetParent(transform);
            indicator.SetActive(false);
            indicatorPool.Enqueue(indicator);
        }
    }

    public GameObject GetIndicator(GameObject target)
    {
        if (indicatorPool.Count > 0)
        {
            GameObject indicator = indicatorPool.Dequeue();
            indicator.SetActive(true);
            Indicator ind = indicator.GetComponent<Indicator>();
            ind.target = target;
            ind.timeLeft = ind.revealTime;
            return indicator;
        }
        else
        {
            return null; // return nothing if all indicators are being used
        }
    }

    public void ReturnIndicator(GameObject indicator)
    {
        indicator.SetActive(false);
        indicatorPool.Enqueue(indicator);
    }
}
