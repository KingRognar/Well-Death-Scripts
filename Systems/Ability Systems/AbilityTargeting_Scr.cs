using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

public class AbilityTargeting_Scr : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform marker;
    public List<Transform> markersList = new List<Transform>();
    public TargetingType targetingType;
    public AreaType _areaType;
    public AreaType areaType
    {
        get {return _areaType;}
        set {_areaType = value; OnAreaTypeChange(); }
    }
    public int circleSize = 2;
    [Space(30)]
    public string objectName;
    public Vector3 hitPoint;
    public Vector3 hitNormal;

    bool kavo = true;


    private void Start()
    {
        OnAreaTypeChange();
    }
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            objectName =  hit.transform.gameObject.name;
            hitPoint = hit.point;
            hitNormal = hit.normal;

            Vector3 markerPosition = hitPoint + hitNormal * 0.5f;
            markerPosition = new Vector3(Mathf.Round(markerPosition.x), Mathf.Round(markerPosition.y), Mathf.Round(markerPosition.z));
            // TODO: пропускать если основная цель в воздухе? (мб опционно)
            areaHighlight(markerPosition); // TODO: можно запускать, только если позиция поменялась
        }
        
        
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
    }

    private Action<Vector3> areaHighlight;
    private void SingleCellAreaHighlight(Vector3 targetPos)
    {
        markersList[0].position = targetPos;
    }
    private void CircleAreaHighlight(Vector3 targetPos)
    {
        List<Vector3> positions = CircleAreaPositions(targetPos);
        positions = RemovePositionsInAir(positions);// TODO: сделать опциональным?
        for (int i = 0; i < markersList.Count; i++)
        {
            if (i < positions.Count)
                markersList[i].position = positions[i];
            else
                markersList[i].position = new Vector3(0, 100, 0);
        }

    }
    List<Vector3> CircleAreaPositions (Vector3 markerPos)
    {
        List<Vector3> positions = new List<Vector3>();
        int maxDist = circleSize - 1;
        for (int i = -maxDist; i <= maxDist; i++)
            for (int j = -maxDist; j <= maxDist; j++)
                if (Mathf.Abs(i) + Mathf.Abs(j) <= maxDist)
                    positions.Add(markerPos + new Vector3(i, 0, j));

        return positions;
    }

    int CenteredSqareNumber(int num)
    {
        num = Mathf.Max(num, 0);
        int ans = ((int)Mathf.Pow((2*num - 1), 2) + 1) / 2;
        return ans;
    }
    List<Vector3> RemovePositionsInAir(List<Vector3> positions)
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
    void OnAreaTypeChange() // TODO: подумоть как оптимизировать
    {
        if (_areaType == AreaType.Circle)
        {
            PrepareMarkers(CenteredSqareNumber(circleSize));
            areaHighlight = CircleAreaHighlight;
        }
        if (_areaType == AreaType.SingleCell)
        {
            PrepareMarkers(1);
            areaHighlight = SingleCellAreaHighlight;
        }
    }
}
