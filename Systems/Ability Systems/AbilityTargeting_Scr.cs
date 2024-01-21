using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTargeting_Scr : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform marker;
    [Space(30)]
    public string objectName;
    public Vector3 hitPoint;
    public Vector3 hitNormal;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            objectName =  hit.transform.gameObject.name;
            hitPoint = hit.point;
            hitNormal = hit.normal;

            marker.position = hitPoint + hitNormal * 0.5f;
            marker.position = new Vector3(Mathf.Round(marker.position.x), Mathf.Round(marker.position.y), Mathf.Round(marker.position.z));
        }
            
    }
}
