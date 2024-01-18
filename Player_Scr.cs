using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using TBS;
using static DataBase_Scr;

public class Player_Scr : MonoBehaviour
{
    public static Player_Scr instance;

    public Transform playerArmature;
    public Transform currentMapTrans;

    private Animator playerAnimator;

    private Task turnTask = null, camRotationTask = null;
    [SerializeField]
    private AnimationCurve cameraRotationCurve;
    private Matrix2x2 controlsRotationMatrix = new Matrix2x2(new float[,] { { 1, 0 }, { 0, 1 } });

    public TBSToken tbsToken;

    private bool isAwaitingTurn = false;

    [SerializeField]
    private int damage = 4;
    private int healthCur = 20, healthMax = 20;
    bool isKicking = false;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isKicking = !isKicking;
            Debug.Log("сменил стойку");
        }

    }

    private void UpdatePlayerMapPos(Vector3Int newPos) // TODO: сделать общий метод и запихнуть его в Field_Scr
    {
        Vector3Int oldPos = Field_Scr.playerMapPos;
        Field_Scr.GetMapCell(oldPos).objID = 0;
        Field_Scr.GetMapCell(newPos).objID = IDDict[IDkeys.player];
        Field_Scr.playerMapPos = newPos;
        Field_Scr.UpdatePathMap();
    }

    ////---//// »гровые механики ////---////
    public void TakeDamage(int damage, Vector3Int enemyPosition) // TODO: получать пиздов
    {
        _ = AnimationsDB_Scr.instance.DBGetHitAnim(transform, playerAnimator, Field_Scr.MapToWorldPosition(enemyPosition));
        healthCur -= damage;
        if (healthCur <= 0)
            Die();
    }
    public void TakeDamage(int damage)
    {
        healthCur -= damage;
        if (healthCur <= 0)
            Die();
    }
    private void Die()
    {

    }

    ////---//// ќбработка инпута игрока ////---////
    /// <summary>
    /// Task ожидани€ ввода игрока
    /// </summary>
    public async Task AwaitTurn()
    {
        isAwaitingTurn = true;
        while (isAwaitingTurn && !Input.GetKeyDown(KeyCode.P))
        {
            await Task.Yield();
        }
    }
    /// <summary>
    /// ћетод вызываемый при нажатии клавиш передвижени€
    /// </summary>
    public async void MovementCtrl(InputAction.CallbackContext inpContext) // TODO: ? + подписать + перелопатить
    {
        if (!inpContext.started) //проходим дальше, если клавиша нажата
            return;
        if (!isAwaitingTurn) //проходим дальше, если ожидаетс€ ввод игрока
            return;
        if (turnTask != null && !turnTask.IsCompleted) //проходим дальше, если Task'и анимации завершены или остутствуют
            return;


        Vector2 movementDirection = inpContext.ReadValue<Vector2>();
        movementDirection = controlsRotationMatrix.Multiply(movementDirection);
        Vector3Int nextPos = Field_Scr.playerMapPos + new Vector3Int((int)movementDirection.x, -(int)movementDirection.y, 0);
        nextPos = PositionAvailability(nextPos);
        Vector3 animPos = Field_Scr.MapToWorldPosition(nextPos);

        if (Field_Scr.GetMapCell(nextPos).objID == IDDict[IDkeys.enemy]) // TODO: переписать
        {
            turnTask = MakeAttack(nextPos);
            await turnTask;
            turnTask = null;
            isAwaitingTurn = false;
            return;
        }
        if (nextPos != Field_Scr.playerMapPos)
        {
            UpdatePlayerMapPos(nextPos);
            turnTask = AnimationsDB_Scr.instance.DBMovementAnim(transform, playerAnimator, animPos); // TODO: переписать
            await turnTask;
            turnTask = null;
            isAwaitingTurn = false;
        } else {
            turnTask = BumpAnim(movementDirection);
            await turnTask;
            turnTask = null;
        }
    }
    /// <summary>
    /// ћетод вызываемый при нажатии клавиш поворота камеры
    /// </summary>
    public async void CameraCtrl(InputAction.CallbackContext inpContext) // TODO: подписать, перенести в отдельный скрипт
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

        camRotationTask = CameraRotationAnim(direction);
        await camRotationTask;
        camRotationTask = null;
    } 
    Vector3Int PositionAvailability(Vector3Int position) // TODO: переделать дл€ атак
    {
        Vector3Int pPos = Field_Scr.playerMapPos;

        //провер€ем не выходит ли позици€ за границы X и Y 
        if (!Field_Scr.CheckMapBounds(position))
            return pPos;

        //провер€ем свободна ли клетка на провер€емой позиции, если да - провер€ем что под ней
        if (Field_Scr.GetMapCell(position).objID >= 0)
        {
            //считаем сколько свободных клеток под провер€емой позицией
            int dropHeight = DropHeight(position);
            if (dropHeight > 2)
                TakeDamage(dropHeight - 2);
            return (position - new Vector3Int(0, 0, dropHeight));
        }

        //провер€ем свободны ли клетки над провер€емой позицией и над игроком, если да - возвращаем перую
        if (Field_Scr.GetMapCell(position + new Vector3Int(0, 0, 1)).objID >= 0 &&
            Field_Scr.GetMapCell(pPos + new Vector3Int(0, 0, 1)).objID == 0) 
            return position + new Vector3Int(0, 0, 1);

        return pPos;
    }
    /// <summary>
    /// ћетод возвращающий количество свободных клеток под провер€емой позицией
    /// </summary>
    /// <param name="position">ѕровер€ема€ позици€</param>
    /// <returns> оличество свободных клеток</returns>
    int DropHeight(Vector3Int position) // TODO: мб улучшить
    {
        int dropHeight = 0;
        while ((position.z - (dropHeight + 1)) >= 0 && Field_Scr.GetMapCell(position - new Vector3Int(0, 0, dropHeight + 1)).objID == 0) 
            dropHeight++;

        Debug.Log("Drop height - " + dropHeight);
        return dropHeight;
    }


    private async Task MakeAttack(Vector3Int enemyPosition)
    {
        Enemy_Scr targetEnemy = Field_Scr.GetMapCell(enemyPosition).enemyRef;

        if (!isKicking)
        {
            _ = AnimationsDB_Scr.instance.DBAttcakAnim(transform, playerAnimator, Field_Scr.MapToWorldPosition(enemyPosition));
            await targetEnemy.TakeDamage(damage, Field_Scr.playerMapPos);
        } else {
            _ = targetEnemy.GetPushed(targetEnemy.enemyMapPos - Field_Scr.playerMapPos);
            await AnimationsDB_Scr.instance.DBAttcakAnim(transform, playerAnimator, Field_Scr.MapToWorldPosition(enemyPosition));

        }


    }


    ////---//// јнимационные “аски ////---////
    async Task BumpAnim(Vector2 direction) // TODO: сделать второй вариант? сделать красиво. подписать
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = Field_Scr.playerMapPos + new Vector3(direction.x, -direction.y, 0);
        targetPos = new Vector3(targetPos.x, targetPos.z, Field_Scr.mapHeight - targetPos.y - 1f); // TODO: помен€ть
        float movementSpeed = 4f;
        float arcHeight = .5f;

        Vector2 startXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 nextXZ = startXZ;
        Vector2 targetXZ = new Vector2(targetPos.x, targetPos.z);
        float dist = Vector2.Distance(startXZ, targetXZ);

        playerAnimator.Play("Move State");
        float temp = 0;
        while (temp != 1)
        {
            if (temp < 0.5f)
                nextXZ = Vector2.MoveTowards(nextXZ, targetXZ, movementSpeed * Time.deltaTime);
            else
                nextXZ = Vector2.MoveTowards(nextXZ, startXZ, movementSpeed * Time.deltaTime);
            temp = Mathf.MoveTowards(temp, 1, movementSpeed * Time.deltaTime);
            float baseY = Mathf.Lerp(startPos.y, targetPos.y, temp);
            float arc = arcHeight * Vector2.Distance(nextXZ, startXZ) * Vector2.Distance(nextXZ, targetXZ) / (0.25f * dist * dist);
            transform.position = new Vector3(nextXZ.x, baseY + arc, nextXZ.y);
            await Task.Yield();
        }
        playerAnimator.Play("Idle State");
        transform.position = startPos;
    }
    /// <summary>
    /// Task анимации - поворачивает камеру по дуге вокруг мировой оси Y
    /// </summary>
    /// <param name="direction">Ќаправление поворота</param>
    async Task CameraRotationAnim(float direction)
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

        Camera.main.transform.RotateAround(currentMapTrans.position, Vector3.up, targetAngle); // TODO: придумать получше вариант получени€ позиции объекта карты
        Quaternion targetQuat = camera.rotation;
        Vector3 targetPos = camera.position;

        float temp = 0;
        while(temp != 1)
        {
            temp = Mathf.MoveTowards(temp, 1, rotationSpeed * Time.deltaTime);
            camera.rotation = Quaternion.Lerp(startQuat, targetQuat, cameraRotationCurve.Evaluate(temp));
            camera.position = DataBase_Scr.SlerpWO(startPos, targetPos, currentMapTrans.position + offset, cameraRotationCurve.Evaluate(temp)); // TODO: придумать получше вариант получени€ позиции объекта карты
            await Task.Yield();
        }
    }

}
