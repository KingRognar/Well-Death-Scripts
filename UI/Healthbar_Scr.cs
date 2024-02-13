using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar_Scr : MonoBehaviour
{
    public static Healthbar_Scr instance;

    public RectTransform healthbarForeground;
    public RectTransform healthbarBackground;

    private int initialWidth = 30;
    public int minHeight = 20;
    public int heightStep = 2;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
    private void Start()
    {
        updateHealthbar();
    }

    public void updateHealthbar()
    {
        healthbarForeground.sizeDelta = new Vector2 (initialWidth, minHeight + heightStep * Player_Scr.instance.healthMax);
        healthbarBackground.sizeDelta = new Vector2(27, heightStep * Player_Scr.instance.healthCur);
    }
}
