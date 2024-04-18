using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TBS;
using static DataBase_Scr;

public class Player_Scr : MonoBehaviour
{
    public static Player_Scr instance;

    public Transform playerArmature;

    private Animator playerAnimator;

    public TBSToken tbsToken;
    public Task turnTask = null;

    private CreatureSoundController_Scr sountCtrl;

    [SerializeField]
    private int damage = 4;
    public int healthCur = 20, healthMax = 20;
    bool isKicking = false;

    public AbilitySO[] playerAbilities = new AbilitySO[5];

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
        sountCtrl = GetComponent<CreatureSoundController_Scr>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            isKicking = !isKicking;
            Debug.Log("сменил стойку");
        }

    }

    // TODO: отделить input и поворот камеры от Player_Scr и оставить здесь только геймплейные интеракции
    // TODO: разделить на разделы переменные и подисать эти разделы

    private void UpdatePlayerMapPos(Vector3Int newPos) // TODO: сделать общий метод и запихнуть его в Field_Scr
    {
        Vector3Int oldPos = Field_Scr.playerMapPos;
        Field_Scr.GetMapCell(oldPos).objID = 0;
        Field_Scr.GetMapCell(newPos).objID = IDDict[IDkeys.player];
        Field_Scr.playerMapPos = newPos;
        Field_Scr.UpdatePathMap();
    }

    public async Task MakeMove(Vector2 moveDirection)
    {
        Vector3Int nextPos = Field_Scr.playerMapPos + new Vector3Int((int)moveDirection.x, -(int)moveDirection.y, 0);
        nextPos = PositionAvailability(nextPos);
        Vector3 animPos = Field_Scr.MapToWorldPosition(nextPos);

        InputController_Scr.instance.isAwaitingInput = false;

        if (Field_Scr.GetMapCell(nextPos).objID == IDDict[IDkeys.enemy]) // TODO: переписать
        {
            turnTask = MakeAttack(nextPos);
            await turnTask;
            turnTask = null;
            InputController_Scr.instance.isAwaitingTurn = false;
            return;
        }
        if (nextPos != Field_Scr.playerMapPos)
        {
            UpdatePlayerMapPos(nextPos);
            turnTask = AnimationsDB_Scr.instance.DBMovementAnim(transform, playerAnimator, animPos);
            await turnTask; // TODO: переписать
            turnTask = null;
            InputController_Scr.instance.isAwaitingTurn = false;
        }
        else
        {
            turnTask = BumpAnim(moveDirection);
            await turnTask;
            turnTask = null;
            InputController_Scr.instance.isAwaitingInput = true;
        }
    }

    ////---//// Игровые механики ////---////
    public void TakeDamage(int damage, Vector3Int enemyPosition) // TODO: получать пиздов
    {
        _ = AnimationsDB_Scr.instance.DBGetHitAnim(transform, playerAnimator, Field_Scr.MapToWorldPosition(enemyPosition),sountCtrl);
        healthCur -= damage;
        Healthbar_Scr.instance.updateHealthbar();
        if (healthCur <= 0)
            Die();
    }
    public void TakeDamage(int damage)
    {
        healthCur -= damage;
        Healthbar_Scr.instance.updateHealthbar();
        if (healthCur <= 0)
            Die();
    }
    private void Die()
    {

    }

    ////---//// Обработка инпута игрока ////---////


    Vector3Int PositionAvailability(Vector3Int position) // TODO: переделать для атак
    {
        Vector3Int pPos = Field_Scr.playerMapPos;

        //проверяем не выходит ли позиция за границы X и Y 
        if (!Field_Scr.CheckMapBounds(position))
            return pPos;

        //проверяем свободна ли клетка на проверяемой позиции, если да - проверяем что под ней
        if (Field_Scr.GetMapCell(position).objID >= 0)
        {
            //считаем сколько свободных клеток под проверяемой позицией
            int dropHeight = DropHeight(position);
            if (dropHeight > 2)
                TakeDamage(dropHeight - 2);
            return (position - new Vector3Int(0, 0, dropHeight));
        }

        //проверяем свободны ли клетки над проверяемой позицией и над игроком, если да - возвращаем перую
        if (Field_Scr.GetMapCell(position + new Vector3Int(0, 0, 1)).objID >= 0 &&
            Field_Scr.GetMapCell(pPos + new Vector3Int(0, 0, 1)).objID == 0) 
            return position + new Vector3Int(0, 0, 1);

        return pPos;
    }
    /// <summary>
    /// Метод возвращающий количество свободных клеток под проверяемой позицией
    /// </summary>
    /// <param name="position">Проверяемая позиция</param>
    /// <returns>Количество свободных клеток</returns>
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
            _ = AnimationsDB_Scr.instance.DBAttcakAnim(transform, playerAnimator, Field_Scr.MapToWorldPosition(enemyPosition), sountCtrl);
            await targetEnemy.TakeDamage(damage, Field_Scr.playerMapPos);
        } else {
            _ = targetEnemy.GetPushed(targetEnemy.enemyMapPos - Field_Scr.playerMapPos);
            await AnimationsDB_Scr.instance.DBAttcakAnim(transform, playerAnimator, Field_Scr.MapToWorldPosition(enemyPosition), sountCtrl);

        }


    }


    ////---//// Анимационные Таски ////---////
    async Task BumpAnim(Vector2 direction) // TODO: сделать второй вариант? сделать красиво. подписать
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = Field_Scr.playerMapPos + new Vector3(direction.x, -direction.y, 0);
        targetPos = new Vector3(targetPos.x, targetPos.z, Field_Scr.mapHeight - targetPos.y - 1f); // TODO: поменять
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
            if (destroyCancellationToken.IsCancellationRequested)
                return;

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
    /// <param name="direction">Направление поворота</param>
}
