using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InputController_Scr : MonoBehaviour
{
    public static InputController_Scr instance;

    private bool _isAwaitingInput;
    public bool isAwiatingInput
    { 
        get { return _isAwaitingInput; } 
        set { _isAwaitingInput = value; AbilityTargeting_Scr.instance.isAwaitingInput = value; }
    }


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public async Task AwaitTurn()
    {
        isAwiatingInput = true;
        while (isAwiatingInput && !Input.GetKeyDown(KeyCode.P))
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            await Task.Yield();
        }
    }
}
