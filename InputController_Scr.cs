using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static DataBase_Scr;

public class InputController_Scr : MonoBehaviour
{
    public static InputController_Scr instance;

    private bool _isAwaitingInput = false;
    public bool isAwaitingInput
    {
        get { return _isAwaitingInput; }
        set { 
            _isAwaitingInput = value;
            if (AbilityTargeting_Scr.instance != null)
                AbilityTargeting_Scr.instance.isAwaitingInput = value; 
        }
    }
    public bool isAwaitingTurn = false; // TODO: ����������� � TBS_Scr

    private Task camRotationTask = null;

    private Matrix2x2 controlsRotationMatrix = new Matrix2x2(new float[,] { { 1, 0 }, { 0, 1 } });

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
    /// <summary>
    /// Task �������� ����� ������
    /// </summary>
    public async Task AwaitTurn()
    {
        isAwaitingInput = true;
        isAwaitingTurn = true;
        while (isAwaitingTurn && !Input.GetKeyDown(KeyCode.P))
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            await Task.Yield();
        }
    }

    /// <summary>
    /// ����� ���������� ��� ������� ������ ������������
    /// </summary>
    public async void MovementCtrl(InputAction.CallbackContext inpContext) // TODO: ? + ��������� + ������������
    {
        if (!inpContext.started) //�������� ������, ���� ������� ������
            return;
        if (!isAwaitingInput) //�������� ������, ���� ����� �� ��������
            return;
        if (Player_Scr.instance.turnTask != null && !Player_Scr.instance.turnTask.IsCompleted) //�������� ������, ���� Task'� �������� ��������� ��� �����������
            return;


        Vector2 moveDirection = inpContext.ReadValue<Vector2>();
        moveDirection = controlsRotationMatrix.Multiply(moveDirection);

        await Player_Scr.instance.MakeMove(moveDirection);
    }
    /// <summary>
    /// ����� ���������� ��� ������� ������ �������� ������
    /// </summary>
    public async void CameraCtrl(InputAction.CallbackContext inpContext) // TODO: ���������, ��������� � ��������� ������
    {
        if (!inpContext.started)
            return;
        if (camRotationTask != null && !camRotationTask.IsCompleted)
            return;

        float direction = inpContext.ReadValue<float>();

        if (direction > 0)
            controlsRotationMatrix = controlsRotationMatrix.RotateLeft();
        else
            controlsRotationMatrix = controlsRotationMatrix.RotateRight();

        await (camRotationTask = Camera_Scr.instance.CameraRotationAnim(direction));
        camRotationTask = null;
    }
}
