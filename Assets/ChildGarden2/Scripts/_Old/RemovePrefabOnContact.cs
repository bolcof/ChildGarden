using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePrefabOnContact : MonoBehaviour
{
    public GameObject effectPrefab;
    private Dictionary<GameObject, float> prefabContactTimes = new Dictionary<GameObject, float>();

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Onbutu") && !prefabContactTimes.ContainsKey(collision.gameObject))
        {
            prefabContactTimes.Add(collision.gameObject, Time.time);
            StartCoroutine(RemovePrefabAfterDelay(collision.gameObject, 5.0f));
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Onbutu") && prefabContactTimes.ContainsKey(collision.gameObject))
        {
            prefabContactTimes.Remove(collision.gameObject);
        }
    }

    IEnumerator RemovePrefabAfterDelay(GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (prefabContactTimes.ContainsKey(prefab))
        {
            float contactTime = prefabContactTimes[prefab];
            if (Time.time - contactTime >= delay)
            {
                Instantiate(effectPrefab, prefab.transform.position, Quaternion.identity);
                Destroy(prefab);
            }
            prefabContactTimes.Remove(prefab);
        }
    }
}
