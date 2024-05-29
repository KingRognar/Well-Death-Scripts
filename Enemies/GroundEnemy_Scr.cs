using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using static AnimationsDB_Scr;
using static DataBase_Scr;

public class GroundEnemy_Scr : Enemy_Scr
{
    

    // TODO: при падениях и толчках нужно породумать столкновения с игроком

    public override async Task MakeTurn()
    {
        if (turnTask != null)
            await turnTask;
        if (enemyMapPos.z > 0 && Field_Scr.GetMapCell(enemyMapPos - new Vector3Int(0, 0, 1)).objID == 0)
        {
            await Fall();
            return;
        }    

        int curPathNumber = Field_Scr.GetMapCell(enemyMapPos).pathValue;
        if (curPathNumber == 0)
        {
            Debug.Log("Враг " + gameObject.name + " не может найти путь до игрока");
            return;
        }
        if (curPathNumber == 2)
        {
            await MakeAttack();
            return;
        }

        List<Vector3Int> possiblePositions = new List<Vector3Int>();

        // проверяем все возможные клетки возле противника
        for (int i = -1; i <= 1; i++) 
            for (int j = -1; j <= 1; j++)
                for (int k = -1; k <= 1; k++)
                {
                    if (i == j && i == -j) // пропускаем, если позиция меняется по диаогнали или не изменяется вовсе
                        continue;
                    Vector3Int possiblePosition = enemyMapPos + new Vector3Int(i, j, k);
                    if (!Field_Scr.CheckMapBounds(possiblePosition)) // проверка на границы массива
                        continue;

                    // если проверяемая позиция на карте пути имеет значение на 1 меньше нашего и не занята другими существами, добавляем её в список возможных
                    if (Field_Scr.GetMapCell(possiblePosition).pathValue == curPathNumber - 1 &&
                        Field_Scr.GetMapCell(possiblePosition).objID == 0) 
                        possiblePositions.Add(new Vector3Int(possiblePosition.x, possiblePosition.y, possiblePosition.z));
                }

        // если возможных позиций для передвижения нет пропускаем ход
        if (possiblePositions.Count == 0)
            return;
        // выбираем случайную позицию из возможных, пускаем анимацию, а потом обновляем позицию на карте
        Vector3Int nextPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

        await MakeMove(nextPosition);

        Field_Scr.UpdatePathMap(); // TODO: мб убрать для оптимизации и придумать улучшенный поиск пути
    } // TODO: разделить логику хода и передвижения
    protected override async Task MakeAttack()
    {
        // TODO: вставить проверки на возможность атаки

        Task[] tasksList = new Task[2];
        tasksList[0] = Player_Scr.instance.TakeDamage(enemyDamage, enemyMapPos);
        turnTask = AnimationsDB_Scr.instance.DBAttcakAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(Field_Scr.playerMapPos), soundCtrl);
        tasksList[1] = turnTask;
        await Task.WhenAll(tasksList);

        turnTask = null;
    }
    public override async Task GetPushed(Vector3Int direction)
    {
        // TODO: добавить силу толчка - чем больше сила, тем дальше передвинется противник + чем больше осталось клеток для передвижения, те больше будет нанесённый урон при столкновении

        Vector3Int targetPos = enemyMapPos + direction;

        // столкновение с другим объектом или с границами карты
        if (!Field_Scr.CheckMapBounds(targetPos) || Field_Scr.GetMapCell(targetPos).objID != 0)
        {
            Debug.Log(name + " столкнулся с другим объектом");
            int collisionDamage = 3;
            await TakeDamage(collisionDamage, enemyMapPos - direction);
            // столкновение с другим врагом
            if (Field_Scr.GetMapCell(targetPos).objID == IDDict[IDkeys.enemy])
            {
                Debug.Log(name + " столкнулся с другим врагом");
                Enemy_Scr secondEnemy = Field_Scr.GetMapCell(targetPos).enemyRef;
                await secondEnemy.TakeDamage(collisionDamage, enemyMapPos);
            }
        } else {
            UpdateEnemyMapPos(targetPos);
            turnTask = AnimationsDB_Scr.instance.DBMovementAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(targetPos)); // TODO: поменять на анимацию отталкивания
            await turnTask;
            turnTask = null;
            turnTask = Fall();
            await turnTask;
            turnTask = null;
        }
    }
    public override async Task PushFromAbove() // TODO: сделал коряво, переделать нормально
    {
        List<Vector3Int> possiblePushDirections = new List<Vector3Int>();
        List<Vector3Int> pushDirections = new List<Vector3Int>();
        possiblePushDirections.Add(new Vector3Int(1, 0, 0));
        possiblePushDirections.Add(new Vector3Int(-1, 0, 0));
        possiblePushDirections.Add(new Vector3Int(0, 1, 0));
        possiblePushDirections.Add(new Vector3Int(0, -1, 0));

        foreach (Vector3Int possibleDir in possiblePushDirections)
        {
            if (Field_Scr.GetMapCell(enemyMapPos + possibleDir).objID == 0)
                pushDirections.Add(possibleDir);
        }

        if (pushDirections.Count == 0)
        {
            await Die();
            return;
        }

        await GetPushed(pushDirections[Random.Range(0, pushDirections.Count)]);
    }

    private async Task Fall()
    {
        //Ищем первую клетку с землёй, если она равна изначальной позиции противника - ничего не делаем
        Vector3Int lowestPos = GetLowestPosition(enemyMapPos);
        if (lowestPos == enemyMapPos)
            return;
        //Расчёт урона от падения // TODO: можна изменить
        int falldamage = enemyMapPos.z - lowestPos.z;
        //если на новой позиции есть другой враг
        if (Field_Scr.GetMapCell(lowestPos).objID == IDDict[IDkeys.enemy])
        {
            Enemy_Scr anotherEnemy = Field_Scr.GetMapCell(lowestPos).enemyRef;
            bool isEnemyDied = false; // TODO: при смерти врага падать чуть выше
            await anotherEnemy.PushFromAbove();
        }
        UpdateEnemyMapPos(lowestPos);
        turnTask = AnimationsDB_Scr.instance.DBFallAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(lowestPos));
        await turnTask;
        turnTask = null;
        await TakeDamage(falldamage, enemyMapPos + new Vector3Int(0, 0, 1));
    }


}
