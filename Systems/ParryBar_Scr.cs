using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ParryBar_Scr : MonoBehaviour
{
    [SerializeField]
    private Slider mySlider;

    [SerializeField]
    private float sliderSpeed = 2f;



    public async Task<bool> TryToCounter()
    {
        BarReset();
        bool result = false;

        while (mySlider.value != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return false;

            mySlider.value = Mathf.MoveTowards(mySlider.value, 1, sliderSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (mySlider.value >= 0.37 && mySlider.value <= 0.63)
                    result = true;

                gameObject.SetActive(false);
                return result;
            }

            await Task.Yield();
        }

        gameObject.SetActive(false);
        return result;
    }

    void BarReset()
    {
        mySlider.value = 0;
    }


}
