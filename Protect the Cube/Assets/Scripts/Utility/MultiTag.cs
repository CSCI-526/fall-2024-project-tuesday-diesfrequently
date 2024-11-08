using UnityEngine;

// used to implement multi-tag tagging system for prefabs

[System.Serializable]
public class MultiTag : MonoBehaviour
{
    [SerializeField] private string[] tags;

    public bool HasTag(string tag)
    {
        foreach (var t in tags)
        {
            if (t == tag) return true;
        }
        return false;
    }

    public string[] GetTags()
    {
        return tags;
    }
}