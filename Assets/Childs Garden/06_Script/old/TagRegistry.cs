using System.Collections.Generic;
using UnityEngine;

public class TagRegistry : MonoBehaviour
{
    public static TagRegistry Instance;

    private List<GameObject> winLoseTaggedObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Register(GameObject obj)
    {
        if (!winLoseTaggedObjects.Contains(obj))
        {
            winLoseTaggedObjects.Add(obj);
        }
    }

    public void Unregister(GameObject obj)
    {
        if (winLoseTaggedObjects.Contains(obj))
        {
            winLoseTaggedObjects.Remove(obj);
        }
    }

    public bool HasWinLoseTaggedObject()
    {
        return winLoseTaggedObjects.Count > 0;
    }
}
