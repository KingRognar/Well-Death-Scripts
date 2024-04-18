using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Camera_Scr : MonoBehaviour
{
    public static Camera_Scr instance;

    public Transform currentMapTrans;

    [SerializeField]
    private AnimationCurve cameraRotationCurve;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
    void Update()
    {
        // TODO: временное решение
        transform.position = new Vector3(transform.position.x, Player_Scr.instance.transform.position.y + 5f, transform.position.z);
    }

    public async Task CameraRotationAnim(float direction)
    {
        Transform camera = Camera.main.transform;
        Quaternion startQuat = camera.rotation;
        Vector3 offset = new Vector3(0, camera.position.y, 0);
        Vector3 startPos = camera.position;
        float rotationSpeed = 3f;
        float targetAngle = 0;
        if (direction < 0)
            targetAngle = 90;
        else
            targetAngle = -90;

        Camera.main.transform.RotateAround(currentMapTrans.position, Vector3.up, targetAngle); // TODO: придумать получше вариант получения позиции объекта карты
        Quaternion targetQuat = camera.rotation;
        Vector3 targetPos = camera.position;

        float temp = 0;
        while (temp != 1)
        {
            if (destroyCancellationToken.IsCancellationRequested)
                return;

            temp = Mathf.MoveTowards(temp, 1, rotationSpeed * Time.deltaTime);
            camera.rotation = Quaternion.Lerp(startQuat, targetQuat, cameraRotationCurve.Evaluate(temp));
            camera.position = DataBase_Scr.SlerpWO(startPos, targetPos, currentMapTrans.position + offset, cameraRotationCurve.Evaluate(temp)); // TODO: придумать получше вариант получения позиции объекта карты
            await Task.Yield();
        }
    }
}
