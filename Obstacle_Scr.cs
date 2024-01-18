using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Scr : MonoBehaviour
{
    public static List<GameObject> obstaclesList = new List<GameObject>();

    // TODO: сделать префабы

    private void OnEnable() => obstaclesList.Add(this.gameObject);
    private void OnDisable() => obstaclesList.Remove(this.gameObject);

}
