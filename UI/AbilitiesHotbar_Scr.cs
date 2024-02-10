using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesHotbar_Scr : MonoBehaviour
{
    private Image[] abilImages = new Image[5];

    private void Start()
    {
        getAbilImages();
        getPlayerAbilIcons();
    }

    private void getAbilImages()
    {
        Transform trans = transform.GetChild(0).GetChild(0);
        for (int i = 0; i < trans.childCount; i++)
        {
            abilImages[i] = trans.GetChild(i).GetComponent<Image>();
        }
    }
    public void getPlayerAbilIcons()
    {
        AbilitySO[] abilitySOs = Player_Scr.instance.playerAbilities;
        for (int i = 0; i < abilitySOs.Length; i++)
        {
            if (abilitySOs[i] == null)
                return;

            abilImages[i].sprite = abilitySOs[i].abilityIcon;
        }
    }
}
