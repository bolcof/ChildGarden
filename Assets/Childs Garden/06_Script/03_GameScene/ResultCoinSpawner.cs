using System.Collections.Generic;
using UnityEngine;

public class ResultCoinSpawner : MonoBehaviour {
    [SerializeField]
    private GameObject prefabToInstantiate;

    private bool hasInstantiated = false;

    private void Update() {
        if (hasInstantiated) return;

        //TODO TagRegistry���������H�H
        /*if (TagRegistry.Instance.HasWinLoseTaggedObject())
        {
            Instantiate(prefabToInstantiate, Vector3.zero, Quaternion.identity);
            hasInstantiated = true;
        }*/
    }
}