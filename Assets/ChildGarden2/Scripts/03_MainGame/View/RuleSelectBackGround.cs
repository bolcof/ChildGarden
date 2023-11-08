using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSelectBackGround : MonoBehaviour {
    [SerializeField] private float rotateSpeed;
    private void Update() {
        transform.Rotate(new Vector3(0f, 0f, rotateSpeed * Time.deltaTime));
    }
}
