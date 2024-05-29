using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using static AnimationsDB_Scr;
using static DataBase_Scr;

public class GroundEnemy_Scr : Enemy_Scr
{
    

    // TODO: ��� �������� � ������� ����� ���������� ������������ � �������

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
            Debug.Log("���� " + gameObject.name + " �� ����� ����� ���� �� ������");
            return;
        }
        if (curPathNumber == 2)
        {
            await MakeAttack();
            return;
        }

        List<Vector3Int> possiblePositions = new List<Vector3Int>();

        // ��������� ��� ��������� ������ ����� ����������
        for (int i = -1; i <= 1; i++) 
            for (int j = -1; j <= 1; j++)
                for (int k = -1; k <= 1; k++)
                {
                    if (i == j && i == -j) // ����������, ���� ������� �������� �� ��������� ��� �� ���������� �����
                        continue;
                    Vector3Int possiblePosition = enemyMapPos + new Vector3Int(i, j, k);
                    if (!Field_Scr.CheckMapBounds(possiblePosition)) // �������� �� ������� �������
                        continue;

                    // ���� ����������� ������� �� ����� ���� ����� �������� �� 1 ������ ������ � �� ������ ������� ����������, ��������� � � ������ ���������
                    if (Field_Scr.GetMapCell(possiblePosition).pathValue == curPathNumber - 1 &&
                        Field_Scr.GetMapCell(possiblePosition).objID == 0) 
                        possiblePositions.Add(new Vector3Int(possiblePosition.x, possiblePosition.y, possiblePosition.z));
                }

        // ���� ��������� ������� ��� ������������ ��� ���������� ���
        if (possiblePositions.Count == 0)
            return;
        // �������� ��������� ������� �� ���������, ������� ��������, � ����� ��������� ������� �� �����
        Vector3Int nextPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

        await MakeMove(nextPosition);

        Field_Scr.UpdatePathMap(); // TODO: �� ������ ��� ����������� � ��������� ���������� ����� ����
    } // TODO: ��������� ������ ���� � ������������
    protected override async Task MakeAttack()
    {
        // TODO: �������� �������� �� ����������� �����

        Task[] tasksList = new Task[2];
        tasksList[0] = Player_Scr.instance.TakeDamage(enemyDamage, enemyMapPos);
        turnTask = AnimationsDB_Scr.instance.DBAttcakAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(Field_Scr.playerMapPos), soundCtrl);
        tasksList[1] = turnTask;
        await Task.WhenAll(tasksList);

        turnTask = null;
    }
    public override async Task GetPushed(Vector3Int direction)
    {
        // TODO: �������� ���� ������ - ��� ������ ����, ��� ������ ������������ ��������� + ��� ������ �������� ������ ��� ������������, �� ������ ����� ��������� ���� ��� ������������

        Vector3Int targetPos = enemyMapPos + direction;

        // ������������ � ������ �������� ��� � ��������� �����
        if (!Field_Scr.CheckMapBounds(targetPos) || Field_Scr.GetMapCell(targetPos).objID != 0)
        {
            Debug.Log(name + " ���������� � ������ ��������");
            int collisionDamage = 3;
            await TakeDamage(collisionDamage, enemyMapPos - direction);
            // ������������ � ������ ������
            if (Field_Scr.GetMapCell(targetPos).objID == IDDict[IDkeys.enemy])
            {
                Debug.Log(name + " ���������� � ������ ������");
                Enemy_Scr secondEnemy = Field_Scr.GetMapCell(targetPos).enemyRef;
                await secondEnemy.TakeDamage(collisionDamage, enemyMapPos);
            }
        } else {
            UpdateEnemyMapPos(targetPos);
            turnTask = AnimationsDB_Scr.instance.DBMovementAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(targetPos)); // TODO: �������� �� �������� ������������
            await turnTask;
            turnTask = null;
            turnTask = Fall();
            await turnTask;
            turnTask = null;
        }
    }
    public override async Task PushFromAbove() // TODO: ������ ������, ���������� ���������
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
        //���� ������ ������ � �����, ���� ��� ����� ����������� ������� ���������� - ������ �� ������
        Vector3Int lowestPos = GetLowestPosition(enemyMapPos);
        if (lowestPos == enemyMapPos)
            return;
        //������ ����� �� ������� // TODO: ����� ��������
        int falldamage = enemyMapPos.z - lowestPos.z;
        //���� �� ����� ������� ���� ������ ����
        if (Field_Scr.GetMapCell(lowestPos).objID == IDDict[IDkeys.enemy])
        {
            Enemy_Scr anotherEnemy = Field_Scr.GetMapCell(lowestPos).enemyRef;
            bool isEnemyDied = false; // TODO: ��� ������ ����� ������ ���� ����
            await anotherEnemy.PushFromAbove();
        }
        UpdateEnemyMapPos(lowestPos);
        turnTask = AnimationsDB_Scr.instance.DBFallAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(lowestPos));
        await turnTask;
        turnTask = null;
        await TakeDamage(falldamage, enemyMapPos + new Vector3Int(0, 0, 1));
    }


}
