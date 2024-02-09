using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

public class AbilityTargeting_Scr : MonoBehaviour
{
    //-// ���������� ���� ������������ 
    // TODO: ����� ������ ��������� ���� scriptable object ������
    public TargetingType targetingType;
    public AreaType _areaType;
    public AreaType areaType
    {
        get { return _areaType; }
        set { _areaType = value; OnAreaTypeChange(); }
    }
    public int areaSize = 2;

    //-// ���������� ��������
    public Transform marker;
    private Vector3 previousMarkerPosition = Vector3.zero;
    public List<Transform> markersList = new List<Transform>();

    //-// ���������� ��� �������������� � ������� ���������
    public List<Vector3Int> sendTargetsList = new List<Vector3Int>();

    //-// ���������� ��������
    public LayerMask layerMask;
    [Space(30)]
    public string objectName;
    public Vector3 hitPoint;
    public Vector3 hitNormal;

    bool kavo = true;


    // TODO: ��������� � ���������� � ����������


    private void Start()
    {
        OnAreaTypeChange();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (kavo)
            {
                areaType = AreaType.SingleCell;
                kavo = !kavo;
            }
            else
            {
                areaType = AreaType.Circle;
                kavo = !kavo;
            }
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            objectName =  hit.transform.gameObject.name;
            hitPoint = hit.point;
            hitNormal = hit.normal;

            Vector3 markerPosition = hitPoint + hitNormal * 0.5f;
            markerPosition = new Vector3(Mathf.Round(markerPosition.x), Mathf.Round(markerPosition.y), Mathf.Round(markerPosition.z));
            // TODO: ���������� ���� �������� ���� � �������? (�� ��������)
            if (markerPosition != previousMarkerPosition)
            {
                previousMarkerPosition = markerPosition;
                areaHighlight(markerPosition);
            }
            if (!Input.GetKeyDown(KeyCode.Mouse0))
                return;
            SendTargetsAndExecuteAbility();
        }  
    }



    private Action<Vector3> areaHighlight;
    /// <summary>
    /// ����� ��� ��������� ���� - ����� �������
    /// (������������ � ���������)
    /// </summary>
    /// <param name="targetPos">������� ������� ���������� ��������</param>
    private void SingleCellAreaHighlight(Vector3 targetPos) // TODO: �������� - RemovePositionsInAir
    {
        sendTargetsList = new List<Vector3Int>();
        sendTargetsList.Add(Field_Scr.WorldToMapPosition(targetPos));
        markersList[0].position = targetPos;
    }
    /// <summary>
    /// ����� ��� ��������� ���� - �������� ��������
    /// (������������ � ���������)
    /// </summary>
    /// <param name="targetPos">����� �������� �������</param>
    private void DiamondAreaHighlight(Vector3 targetPos) // TODO: ��������� �� ����� � ������� ���������
    {
        List<Vector3> positions = DiamondAreaPositions(targetPos);
        positions = RemoveIncorrectPositions(positions);// TODO: ������� ������������?
        sendTargetsList = new List<Vector3Int>();
        foreach (Vector3 pos in positions)
            sendTargetsList.Add(Field_Scr.WorldToMapPosition(pos));
        for (int i = 0; i < markersList.Count; i++)
        {
            if (i < positions.Count)
                markersList[i].position = positions[i];
            else
                markersList[i].position = new Vector3(0, 100, 0);
        }

    }
    /// <summary>
    /// ����� ��� ���������� ������� �������� � ����
    /// </summary>
    /// <param name="targetPos">����� �������� �������</param>
    /// <returns>���� ���������� �������</returns>
    List<Vector3> DiamondAreaPositions (Vector3 targetPos) // TODO: �������������� - ������� ������ ��������
    {
        List<Vector3> positions = new List<Vector3>();
        int maxDist = areaSize - 1;
        for (int i = -maxDist; i <= maxDist; i++)
            for (int j = -maxDist; j <= maxDist; j++)
                if (Mathf.Abs(i) + Mathf.Abs(j) <= maxDist)
                    positions.Add(targetPos + new Vector3(i, 0, j));

        return positions;
    }
    private void CircleAreaHighlight(Vector3 targetPos)
    {
        List<Vector3> positions = CircleAreaPositions(targetPos);
        positions = RemoveIncorrectPositions(positions);// TODO: ������� ������������?
        sendTargetsList = new List<Vector3Int>();
        foreach (Vector3 pos in positions)
            sendTargetsList.Add(Field_Scr.WorldToMapPosition(pos));
        for (int i = 0; i < markersList.Count; i++)
        {
            if (i < positions.Count)
                markersList[i].position = positions[i];
            else
                markersList[i].position = new Vector3(0, 100, 0);
        }
    }
    List<Vector3> CircleAreaPositions (Vector3 targetPos)
    {
        List<Vector3> positions = new List<Vector3>();
        positions.Add(targetPos);
        for (int i = 1; i <= areaSize; i++)
            for (int j = 0; j <= areaSize; j++)
                if (Mathf.Sqrt(i * i + j * j) <= areaSize)
                {
                    positions.Add(targetPos + new Vector3(i, 0, j));
                    positions.Add(targetPos + new Vector3(-j, 0, i));
                    positions.Add(targetPos + new Vector3(j, 0, -i));
                    positions.Add(targetPos + new Vector3(-i, 0, -j));
                }

        return positions;
    }



    /// <summary>
    /// ����� ��� ���������� ��������������� ����������� �����(���) n-��� �������
    /// </summary>
    /// <param name="num">������� �������� ���</param>
    /// <returns>������� ���</returns>
    int CenteredSqareNumber(int num)
    {
        num = Mathf.Max(num, 0);
        int ans = ((int)Mathf.Pow((2*num - 1), 2) + 1) / 2;
        return ans;
    }
    int CellsInCircleNumber(int radius)
    {
        int ans = 0;
        for (int i = 1; i <= radius; i++)
            for (int j = 0; j <= radius; j++)
            {
                if (Mathf.Sqrt(i*i + j*j) <= radius) 
                    ans++;
            }
        Debug.Log(ans * 4 + 1);
        return ans * 4 + 1;
    }
    /// <summary>
    /// ����� ��� �������� �� ����� ������������ �������
    /// </summary>
    /// <param name="positions">���� �� �������� ���������� ������� �������</param>
    /// <returns>��������� ���� �������</returns>
    List<Vector3> RemoveIncorrectPositions(List<Vector3> positions)
    {
        List<Vector3> newPositions = new List<Vector3>();
        foreach (Vector3 pos in positions)
        {
            Vector3Int mapPos = Field_Scr.WorldToMapPosition(pos);
            if (Field_Scr.CheckMapBounds(mapPos) && 
                Field_Scr.GetMapCell(mapPos).objID >= 0 && 
                (mapPos.z == 0 || Field_Scr.GetMapCell(mapPos - new Vector3Int(0,0,1)).objID < 0))
                newPositions.Add(pos);
        }
        return newPositions;
    }
    /// <summary>
    /// ����� ��� ��������\�������� ������������ ���������� �������� ��� �������� ���� �������
    /// </summary>
    /// <param name="count">����������� ���-�� �������� � �����</param>
    void PrepareMarkers(int count)
    {
        while(markersList.Count != count)
        {
            if (markersList.Count < count)
            {
                Transform newMarker = Instantiate(marker, new Vector3(0, 100, 0), Quaternion.identity, transform);
                markersList.Add(newMarker);
                continue;
            }
            GameObject objToRemove = markersList[markersList.Count - 1].gameObject;
            markersList.RemoveAt(markersList.Count - 1);
            Destroy(objToRemove);
        }
    }
    /// <summary>
    /// �����, ���������� ��� ��������� ���������� areaType
    /// </summary>
    void OnAreaTypeChange() // TODO: �������� ��� ��������������
    {
        if (_areaType == AreaType.Diamond)
        {
            PrepareMarkers(CenteredSqareNumber(areaSize));
            areaHighlight = DiamondAreaHighlight;
        }
        if (_areaType == AreaType.SingleCell)
        {
            PrepareMarkers(1);
            areaHighlight = SingleCellAreaHighlight;
        }
        if (_areaType == AreaType.Circle)
        {
            PrepareMarkers(CellsInCircleNumber(areaSize));
            areaHighlight = CircleAreaHighlight;
        }
    }

    private void SendTargetsAndExecuteAbility()
    {
        Debug.Log("�������� " + sendTargetsList.Count + " ����� ��������������");

        AbilityInterpreter_Scr.instance.targetsList = sendTargetsList;
        AbilityInterpreter_Scr.instance.ExecuteAbility();
        // TODO: ���������� ���������� ������ � AbilityInterpreter
        // TODO: ��������� � AbilityInterpreter'e ���������� ������
    }
}
