using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataBase_Scr;

public class Field_Scr : MonoBehaviour
{
    public static int mapHeight = 5, mapWidth = 5, mapDepth = 5;
    //public static int[,,] objectMap = new int[0, 0, 0];
    //public static int[,,] pathMap = new int[0, 0, 0];
    private static MapCell[,,] gameMap = new MapCell[0, 0, 0];
    public static Vector3Int playerMapPos = new Vector3Int();

    public GameObject levelPrefab;

    void Start()
    {
        InitializeGameMap();
        GetLevelSize();
    }


    private void GetLevelSize()
    {
        MeshFilter levelMF = levelPrefab.GetComponent<MeshFilter>();
        Vector2 xDim = new Vector2(0, 0);
        Vector2 yDim = new Vector2(0, 0);
        Vector2 zDim = new Vector2(0, 0);

        foreach (Vector3 vertex in levelMF.mesh.vertices)
        {
            xDim = CompareToMinMaxDimension(vertex.x, xDim);
            yDim = CompareToMinMaxDimension(vertex.y, yDim);
            zDim = CompareToMinMaxDimension(vertex.z, zDim);
        }

        mapHeight = (int)(xDim.y - xDim.x) + 2;
        mapWidth = (int)(zDim.y - zDim.x) + 2;
        mapDepth = (int)(yDim.y - yDim.x) + 2;
    } // TODO: ���������
    private void GetLevelWalls() // TODO: ��������� ��� �� meshFilter'� �������� ���� � ��� ����� ����� �� ����� ���� �������� ��� �����
    {

    }
    private Vector2 CompareToMinMaxDimension(float comparable, Vector2 dim)
    {
        if (comparable < dim.x)
            dim.x = comparable;
        if (comparable > dim.y)
            dim.y = comparable;
        return dim;
    }

    
    /// <summary>
    /// ������������� ����� �������� ���� ���������� � ���������� ��������
    /// </summary>
    private void InitializeGameMap()
    {
        // TODO: ���� ���������
        Vector3 playerPos = Player_Scr.instance.transform.position;

        GetLevelSize();

        //�������������� ����� �������� � ����������� �� ��� ������
        //objectMap = new int[mapWidth, mapHeight, mapDepth];
        //gameMap = new MapCell[mapWidth, mapHeight, mapDepth];
        gameMap = InitializeArray<MapCell>(mapWidth, mapHeight, mapDepth);
        playerMapPos = WorldToMapPosition(playerPos);
        //objectMap[playerMapPos.x, playerMapPos.y, 0] = IDDict[IDkeys.player];
        GetMapCell(playerMapPos).objID = IDDict[IDkeys.player];
        //gameMap[playerMapPos.x, playerMapPos.y, playerMapPos.z].objID = IDDict[IDkeys.player];

        Debug.Log("map height - " + mapHeight + " width - " + mapWidth + " player pos - " + playerMapPos.x + ", " + playerMapPos.y + ", " + playerMapPos.z);

        // TODO: �� ������� ��� ����� �������, ����� �� �����������?
        //����������� ����� � ������������ � ������������ // TODO: ������������
/*        for (int i = 0; i < mapWidth; i++)
        {
            bool hitted = Physics.Raycast(new Vector3(i, 0, mapHeight - 1f), Vector3.back, out hitInfo, mapHeight);
            if (!hitted)
                continue; // TODO: ��� ���� ������ ������� �������� ��������

            int ceil = Mathf.CeilToInt(hitInfo.distance);
            //Debug.Log(" v " + ceil);
            for (int j = ceil; j < mapHeight; j++)
                for (int k = 0; k < mapDepth; k++)
                    gameMap[i, j, k].objID = IDDict[IDkeys.wall];
                    //objectMap[i, j, k] = IDDict[IDkeys.wall];

            Physics.Raycast(new Vector3(i, 0, 0), Vector3.forward, out hitInfo, mapHeight);
            ceil = mapHeight - Mathf.CeilToInt(hitInfo.distance);
            //Debug.Log(" ^ " + ceil);
            for (int j = 0; j < ceil; j++)
                for (int k = 0; k < mapDepth; k++)
                    gameMap[i, j, k].objID = IDDict[IDkeys.wall];
                    //objectMap[i, j, k] = IDDict[IDkeys.wall];
        }
        for (int i = 0; i < mapHeight; i++)
        {
            bool hitted = Physics.Raycast(new Vector3(0, 0, i), Vector3.right, out hitInfo, mapWidth);
            if (!hitted)
                continue;

            int ceil = Mathf.CeilToInt(hitInfo.distance);
            //Debug.Log(" > " + ceil);
            for (int j = ceil; j < mapWidth; j++)
                for (int k = 0; k < mapDepth; k++)
                    gameMap[j, i, k].objID = IDDict[IDkeys.wall];
                    //objectMap[j, i, k] = IDDict[IDkeys.wall];

            Physics.Raycast(new Vector3(mapWidth - 1f, 0, i), Vector3.left, out hitInfo, mapWidth);
            ceil = mapWidth - Mathf.CeilToInt(hitInfo.distance);
            //Debug.Log(" < " + ceil);
            for (int j = 0; j < ceil; j++)
                for (int k = 0; k < mapDepth; k++)
                    gameMap[j, i, k].objID = IDDict[IDkeys.wall];
                    //objectMap[j, i, k] = IDDict[IDkeys.wall];
        }*/

        AddObstacles();
        AddEnemiesOnStart();
        UpdatePathMap();
    }
    /// <summary>
    /// ��������� �� ����� �������� ��� ����������� � ����� �����������
    /// </summary>
    private void AddObstacles()
    {
        foreach (GameObject obstacle in Obstacle_Scr.obstaclesList)
        {
            Vector3Int obstaclePos = WorldToMapPosition(obstacle.transform.position);
            GetMapCell(obstaclePos).objID = IDDict[IDkeys.obstacle];
            //gameMap[obstaclePos.x, obstaclePos.y, obstaclePos.z].objID = IDDict[IDkeys.obstacle];
        }
    }
    /// <summary>
    /// ��������� �� ����� �������� ���� ����������� � ����� �����������
    /// </summary>
    private void AddEnemiesOnStart() // TODO: �� ��������
    {
        foreach (Enemy_Scr enemy in Enemy_Scr.enemiesList)
        {
            AddEnemy(enemy);
        }
    }
    public static void AddEnemy(Enemy_Scr enemy) // TODO: ����� �� �������� �� static?
    {
        Vector3Int curEnemyMapPos = WorldToMapPosition(enemy.transform.position);
        enemy.enemyMapPos = curEnemyMapPos;
        GetMapCell(curEnemyMapPos).objID = IDDict[IDkeys.enemy];
        GetMapCell(curEnemyMapPos).enemyRef = enemy;
        //gameMap[curEnemyMapPos.x, curEnemyMapPos.y, curEnemyMapPos.z].objID = IDDict[IDkeys.enemy];
        //gameMap[curEnemyMapPos.x, curEnemyMapPos.y, curEnemyMapPos.z].enemyRef = enemy;
    }

    /// <summary>
    /// �����, ���������� �������� ���� � ����� ����
    /// </summary>
    private static void ClearPathMap()
    {
        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
                for (int k = 0; k < mapDepth; k++)
                    gameMap[i, j, k].pathValue = 0;
    }
    /// <summary>
    /// ����� ����������� ����� ����
    /// </summary>
    public static void UpdatePathMap()
    {
        ClearPathMap();
        //���������� �� ������� ������ 1 - ����� ����
        GetMapCell(playerMapPos).pathValue = 1;
        //gameMap[playerMapPos.x, playerMapPos.y, playerMapPos.z].pathValue = 1; // TODO: ����� ������� ���

        //�������������� ���� ������ �� �������� � ��������� � ���� ������ ������ - ������ ������
        List<Vector3Int> cellsToCheck = new List<Vector3Int>();
        cellsToCheck.Add(playerMapPos);

        //���� ���� �� ������ ��������� ��� ������ � ��
        while(cellsToCheck.Count > 0)
        {
            List<Vector3Int> cellsToCheckNext = new List<Vector3Int>();
            foreach (Vector3Int cell in cellsToCheck)
                CheckCellPath(cell, cellsToCheckNext);

            cellsToCheck = cellsToCheckNext;
        }
    }
    /// <summary>
    /// ����� ��������� ��� ������ ������ ��������� � ��������� ���������� � ��������� ������ �� ��������
    /// </summary>
    /// <param name="cell">��������� ������</param>
    /// <param name="nextList">��������� ������ ������ �� ��������</param>
    private static void CheckCellPath(Vector3Int cell, List<Vector3Int> nextList) // TODO: �������� ����� ���������� ������
    {
        //��������� � ���� 4 �������� ������, ������� ����� ���������
        List<Vector3Int> possibleCells = new List<Vector3Int>();
        possibleCells.Add(cell + new Vector3Int(1, 0, 0));
        possibleCells.Add(cell + new Vector3Int(-1, 0, 0));
        possibleCells.Add(cell + new Vector3Int(0, 1, 0));
        possibleCells.Add(cell + new Vector3Int(0, -1, 0));

        foreach (Vector3Int possibleCell in possibleCells)
        {
            for (int i = -1; i <= 1; i++)
            {
                Vector3Int cellToCheck = possibleCell + new Vector3Int(0,0,i);
                //�������� �� ������� ������� �����
                if (!CheckMapBounds(cellToCheck))
                    continue;

                // TODO: ����� ��������� � ���������
                if (GetMapCell(cellToCheck).objID >= 0 && GetMapCell(cellToCheck).pathValue == 0 &&
                    (cellToCheck.z == 0 || GetMapCell(cellToCheck - new Vector3Int(0, 0, 1)).objID < 0))
                /*if (gameMap[cellToCheck.x, cellToCheck.y, cellToCheck.z].objID >= 0 && 
                    gameMap[cellToCheck.x, cellToCheck.y, cellToCheck.z].pathValue == 0 &&
                    (cellToCheck.z == 0 || gameMap[cellToCheck.x, cellToCheck.y, cellToCheck.z - 1].objID < 0))*/
                {
                    if (i == -1 && GetMapCell(possibleCell).objID < 0)
                    //if (i == -1 && gameMap[possibleCell.x, possibleCell.y, possibleCell.z].objID < 0)
                        continue;
                    GetMapCell(cellToCheck).pathValue = GetMapCell(cell).pathValue + 1;
                    //gameMap[cellToCheck.x, cellToCheck.y, cellToCheck.z].pathValue = gameMap[cell.x, cell.y, cell.z].pathValue + 1;
                    int clor = GetMapCell(cell).pathValue;
                    Debug.DrawLine(new Vector3(cell.x, cell.z, 9 - cell.y), MapToWorldPosition(cellToCheck), new Color(1 - clor * 0.1f, 0, clor * 0.1f), 5f);
                    nextList.Add(cellToCheck);
                    break;
                }
            }
        }
    }



    ////---//// ������ ��� �������������� � gameMap ////---////
    /// <summary>
    /// ����� ����������� ������ ������� ��������� � ������ ��������� �����
    /// </summary>
    /// <param name="worldPosition">������ ������� ���������</param>
    /// <returns>������ ��������� �����</returns>
    public static Vector3Int WorldToMapPosition(Vector3 worldPosition)
    {
        return new Vector3Int((int)worldPosition.x, (int)(mapHeight - worldPosition.z - 1f), (int)worldPosition.y);
    } // TODO: �������� ���� ����
    /// <summary>
    /// ����� ����������� ������ ��������� ����� � ������ ������� ���������
    /// </summary>
    /// <param name="mapPosition">������ ��������� �����</param>
    /// <returns>������ ������� ���������</returns>
    public static Vector3 MapToWorldPosition(Vector3Int mapPosition)
    {
        return new Vector3(mapPosition.x, mapPosition.z, mapHeight - mapPosition.y - 1f);
    } // TODO: �������� ���� ����
    /// <summary>
    /// ����� �����������, �� ������� �� ����������� ������ �� ������� ������� �����
    /// </summary>
    /// <param name="mapPosition">����������� ������</param>
    /// <returns>True - ���� �� ��������, False - � �������� ������</returns>
    public static bool CheckMapBounds(Vector3Int mapPosition)
    {
        // �������� �� ������ ������� ������� �����
        if (mapPosition.x < 0 || mapPosition.y < 0 || mapPosition.z < 0)
            return false;
        // �������� �� ������� ������� ������� �����
        if (mapPosition.x >= mapWidth ||
            mapPosition.y >= mapHeight ||
            mapPosition.z >= mapDepth)
            return false;

        return true;
    }
    /// <summary>
    /// ����� ���������� �������� ������ � ����� ������� � ������
    /// </summary>
    /// <param name="oldPosition">������� ������, ������� ���� ���������</param>
    /// <param name="newPosition">������� ������, ���� ���� ��������� ��������</param>
    public static void MoveMapCell(Vector3Int oldPosition, Vector3Int newPosition) // TODO: �������
    {
        GetMapCell(newPosition).objID = GetMapCell(oldPosition).objID;
        GetMapCell(newPosition).enemyRef = GetMapCell(oldPosition).enemyRef;

        GetMapCell(oldPosition).objID = 0;
        GetMapCell(oldPosition).enemyRef = null;
    }
    /// <summary>
    /// �����, ������������ MapCell, �� �������
    /// </summary>
    /// <param name="position">������� ������� MapCell</param>
    /// <returns>MapCell �� ������� �������</returns>
    public static MapCell GetMapCell(Vector3Int position)
    {
        return gameMap[position.x, position.y, position.z];
    }
    

    public class MapCell
    {
        public bool isGround;
        public int objID;
        public int pathValue;
        public Enemy_Scr enemyRef;

        public MapCell(bool isGround, int objID, int pathValue, Enemy_Scr enemyRef)
        {
            this.isGround = isGround;
            this.objID = objID;
            this.pathValue = pathValue;
            this.enemyRef = enemyRef;
        }
        public MapCell()
        {
            this.isGround = false;
            this.objID = 0;
            this.pathValue = 0;
            this.enemyRef = null;
        }
    }

    T[,,] InitializeArray<T>(int width, int height, int depth) where T : new()
    {
        T[,,] array = new T[width, height, depth];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                for (int k = 0; k < depth; k++)
                    array[i, j, k] = new T();
        return array;
    }
}
