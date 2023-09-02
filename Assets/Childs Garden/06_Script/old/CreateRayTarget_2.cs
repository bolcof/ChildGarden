using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRayTarget_2 : MonoBehaviour
{
    [SerializeField]
    private float distance = 20.0f;
    [SerializeField]
    private GameObject clone;
    private Camera camera;
    private GameObject previewClone;


    private void Start()
    {
        camera = GetComponent<Camera>();
        previewClone = Instantiate(clone);

    }

    // Update is called once per frame
    void Update()
    {
        //   if (Input.GetMouseButton(0))
        //  {
        //     clone.transform.localScale = Vector3.one;
        //  }
        
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            Vector3 movement = Vector3.Scale(previewClone.transform.localScale, hit.normal) / 20;
            previewClone.transform.position = new Vector3(hit.point.x + movement.x, hit.point.y + movement.y, hit.point.z + movement.z);
           
        }
    }
}
