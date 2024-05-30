using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ParryBar_Scr : MonoBehaviour
{
    public static ParryBar_Scr instance;

    [SerializeField]
    private Slider mySlider;

    [SerializeField]
    private float sliderSpeed = 2f;

    public bool lastResult = false;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public async Task<bool> TryToCounter()
    {
        BarReset();
        bool result = false;

        while (mySlider.value != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
            {
                lastResult = false;
                return false;
            }


            mySlider.value = Mathf.MoveTowards(mySlider.value, 1, sliderSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (mySlider.value >= 0.37 && mySlider.value <= 0.63)
                    result = true;

                gameObject.SetActive(false);
                lastResult = result;
                return result;
            }

            await Task.Yield();
        }

        gameObject.SetActive(false);
        lastResult = result;
        return result;
    }

    void BarReset()
    {
        mySlider.value = 0;
    }


}
