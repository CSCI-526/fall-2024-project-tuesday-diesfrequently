using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upgradeBtn : MonoBehaviour
{
    public int id;

    public void updateturret(){
        GameObject clicked = getObjectById(id);
        clicked.GetComponent<ClickUpgrade>().upgrade();
    }
    public static GameObject getObjectById(int id)
    {
    Dictionary<int, GameObject> m_instanceMap = new Dictionary<int, GameObject>();
    //record instance map

    m_instanceMap.Clear();
    List<GameObject> gos = new List<GameObject>();
    foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
    {
        if (gos.Contains(go))
        {
            continue;
        }
        gos.Add(go);
        m_instanceMap[go.GetInstanceID()] = go;
    }

    if (m_instanceMap.ContainsKey(id))
    {
        return m_instanceMap[id];
    }
    else
    {
        return null;
    }
}
}
