using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Scr : MonoBehaviour
{
    void Update()
    {
        // TODO: временное решение
        transform.position = new Vector3(transform.position.x, Player_Scr.instance.transform.position.y + 5f, transform.position.z);
    }
}
