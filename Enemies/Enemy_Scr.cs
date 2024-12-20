using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TBS;

using static DataBase_Scr;
using System.Threading;

public abstract class Enemy_Scr : MonoBehaviour
{
    CancellationTokenSource cts;
    CancellationToken cToken; 

    public static List<Enemy_Scr> enemiesList = new List<Enemy_Scr>();

    public TBSToken tbsToken;
    protected Task turnTask = null;

    protected Animator enemyAnimator;

    protected CreatureSoundController_Scr soundCtrl;

    [SerializeField] protected int healthMax = 10, healthCur = 10;
    [SerializeField] protected int enemyDamage = 2;
    public Vector3Int enemyMapPos; // TODO: ��������� ��� ����� �������� �������

    [SerializeField] private GameObject corpseBlock;

    private EnemyHealthbar_Scr enemyHealthbar;

    // TODO: ��������� �� ������� ��� � Field_Scr

    private void OnEnable()
    {
        enemyHealthbar = GetComponentInChildren<EnemyHealthbar_Scr>();
        if (enemyHealthbar != null)
            enemyHealthbar.UpdateHealthbar(healthMax, healthCur);
        enemyAnimator = GetComponentInChildren<Animator>();
        soundCtrl = GetComponent<CreatureSoundController_Scr>();
        enemiesList.Add(this);
    }
    private void OnDisable() => enemiesList.Remove(this);




    ////---//// ������ ������� �������� ////---////
    /// <summary>
    /// �����, �������������� ��������� ������ ����� � ������ ��������������� ��������
    /// </summary>
    /// <param name="damage">���������� ����������� �����</param>
    /// <param name="directionOfHit">������� � ������� ��������� ����</param>
    public async Task TakeDamage(int damage, Vector3Int directionOfHit)
    {
        cts = new CancellationTokenSource();
        cToken = cts.Token;

        healthCur -= damage;
        if (enemyHealthbar != null)
            enemyHealthbar.UpdateHealthbar(healthMax, healthCur);
        await AnimationsDB_Scr.instance.DBGetHitAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(directionOfHit), soundCtrl, cToken);
        if (healthCur <= 0)
            await Die();
    }
    /// <summary>
    /// �����, �������������� ������ ����� - ������� ��� �� ����� ������, ��������� ������� � ����� ����, ��������� ��������������� ��������
    /// </summary>
    protected async Task Die() // TODO: ��������� PathMap
    {
        //enemyAnimator.Play("Death State");
        RemoveFromMap();
        await DropCorpse();
        enemiesList.Remove(this);
        TBS_Scr.AddToTokensToRemove(tbsToken);
        //enabled = false;
        Destroy(gameObject);
    }
    /// <summary>
    /// (����������) Task, ����������� ��� �����
    /// </summary>
    public abstract Task MakeTurn();
    protected async Task MakeMove(Vector3Int nextPosition)
    {
        UpdateEnemyMapPos(nextPosition);
        turnTask = AnimationsDB_Scr.instance.DBMovementAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(nextPosition));
        await turnTask;
        turnTask = null;
    }
    /// <summary>
    /// (����������) Task, ����������� ����� �����
    /// </summary>
    protected abstract Task MakeAttack();
    /// <summary>
    /// (����������) Task, ����������� ������ ����� � ��������� �����������
    /// </summary>
    /// <param name="direction">����������� ������, ������ ������������ ������� �����</param>
    public abstract Task GetPushed(Vector3Int direction);
    /// <summary>
    /// (����������) Task, ����������� ������ �����, ��� ������� �� ���� ����-����
    /// </summary>
    public abstract Task PushFromAbove();
    private async Task DropCorpse()
    {
        List<Vector3Int> freeCells = new List<Vector3Int>();
        Vector3Int rollingEnemyPos = enemyMapPos; // TODO: �� ���-�� ��������� � ���� ������
        while (enemyMapPos.z != 0 && CheckNearFloor(rollingEnemyPos, out freeCells))
        {
            Vector3Int chosenCell = freeCells[Random.Range(0, freeCells.Count)];
            chosenCell = GetLowestPosition(chosenCell);
            rollingEnemyPos = chosenCell;
            await MakeCorpseRoll(chosenCell);
        }
        await CreateCorpseBlock(rollingEnemyPos);
    }
    /// <summary>
    /// Task, ����������� Task'� MakeMove(), �� ����������� ����, ��� �� ����������� ������� �� ����� ����. ������������ ��� ������������ ����� ��� ��� ����������
    /// </summary>
    /// <param name="nextPosition">�������, � ������� ������ ��������� ����</param>
    protected async Task MakeCorpseRoll(Vector3Int nextPosition) // TODO: �������� ��������, �� ���������� � MakeMove()?
    {
        //UpdateEnemyMapPos(nextPosition);
        turnTask = AnimationsDB_Scr.instance.DBMovementAnim(transform, enemyAnimator, Field_Scr.MapToWorldPosition(nextPosition));
        await turnTask;
        turnTask = null;
    }
    protected async Task CreateCorpseBlock(Vector3Int position) // TODO: ������?
    {
        GameObject corpseInst = Instantiate(corpseBlock, Field_Scr.MapToWorldPosition(position), Quaternion.identity);
        await corpseInst.GetComponent<CorpseBlock_Scr>().RaiseBlock(position);
    }
    /// <summary>
    /// ��������� �������� 4 ������ �� 1 ����, �������� �� ���
    /// </summary>
    /// <param name="position">�������, ��� ������� ����������� ��������</param>
    /// <param name="freeCells">���� ��������� ������</param>
    /// <returns>���������� True, ���� ��������� ������ >= 2 � False � �������� ������</returns>
    private bool CheckNearFloor(Vector3Int position, out List<Vector3Int> freeCells) // TODO: �������
    {
        freeCells = new List<Vector3Int>();
        List<Vector3Int> cellsToCheck = new List<Vector3Int>();
        cellsToCheck.Add(position + new Vector3Int(1, 0, -1));
        cellsToCheck.Add(position + new Vector3Int(-1, 0, -1));
        cellsToCheck.Add(position + new Vector3Int(0, 1, -1));
        cellsToCheck.Add(position + new Vector3Int(0, -1, -1));

        foreach (Vector3Int cellToCheck in cellsToCheck)
        {
            //������� ������, ���� ��� ������� �� ������� ������� �����
            //���� ��� ������ ����� ��� ������ ���������� ��������
            //���� ��� ������ �������
            if (!Field_Scr.CheckMapBounds(cellToCheck))
                continue;
            if (Field_Scr.GetMapCell(cellToCheck).objID < 0)
                continue;
            if (Field_Scr.GetMapCell(cellToCheck).objID == IDDict[IDkeys.player])
                continue;
            freeCells.Add(cellToCheck);
        }

        if (freeCells.Count >= 2)
            return true;
        return false;
    }
    /// <summary>
    /// ��������� ����� �� 1 ������ ����, ���� �� ���� ���� ����
    /// </summary>
    public async Task Raise()
    {
        Vector3Int newPosition = enemyMapPos + new Vector3Int(0, 0, 1);
        UpdateEnemyMapPos(newPosition);
        await AnimationsDB_Scr.instance.DBRaiseAnim(transform, Field_Scr.MapToWorldPosition(newPosition));
    }


    ////---//// ��������������� ������ ////---////
    /// <summary>
    /// ����� ������������ ������ ������ �� ����� ��� �����������
    /// </summary>
    /// <param name="startingPos">��������� �������, � ������� ���������� ��������</param>
    /// <returns>������ �� �����</returns>
    protected Vector3Int GetLowestPosition(Vector3Int startingPos) // TODO: �������� ���� ����������
    {
        int dropHeight = 1;
        int curZ = startingPos.z;
        while (curZ > 0 && Field_Scr.GetMapCell(startingPos - new Vector3Int(0, 0, dropHeight)).objID >= 0)
        {
            curZ--;
            dropHeight++;
        }
        return startingPos - new Vector3Int(0, 0, --dropHeight);
    }
    /// <summary>
    /// �����, ��������� ����� � ����� ����
    /// </summary>
    protected void RemoveFromMap()
    {
        Field_Scr.GetMapCell(enemyMapPos).objID = 0;
        Field_Scr.GetMapCell(enemyMapPos).enemyRef = null;
    }
    /// <summary>
    /// �����, ����������� ������� ����� �� ����� ����
    /// </summary>
    /// <param name="newPosition"></param>
    protected void UpdateEnemyMapPos(Vector3Int newPosition) // TODO: �������, �������� ���� ����
    {
        Field_Scr.MoveMapCell(enemyMapPos, newPosition);
        enemyMapPos = newPosition;
    }



    //--//--//--// REWORK ZONE //--//--//--//

}
