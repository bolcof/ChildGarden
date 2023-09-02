using System.Collections.Generic;
using UnityEngine;


public class RegisterTag : MonoBehaviour
{
    private void OnEnable()
    {
        if (this.CompareTag("WinLose"))
        {
            TagRegistry.Instance.Register(gameObject);
        }
    }

    private void OnDisable()
    {
        if (this.CompareTag("WinLose"))
        {
            TagRegistry.Instance.Unregister(gameObject);
        }
    }
}