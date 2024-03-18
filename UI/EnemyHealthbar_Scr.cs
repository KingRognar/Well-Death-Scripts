using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar_Scr : MonoBehaviour
{
    [SerializeField] private Image healthbarImage;

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void UpdateHealthbar(int maxValue, int curValue)
    {
        if (maxValue <= 0)
            return;
        healthbarImage.fillAmount = (float)curValue / maxValue;
    }
}
