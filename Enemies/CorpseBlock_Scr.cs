using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CorpseBlock_Scr : MonoBehaviour
{
    public async Task RaiseBlock(Vector3Int mapPosition)
    {
        if (Field_Scr.GetMapCell(mapPosition).objID == DataBase_Scr.IDDict[DataBase_Scr.IDkeys.enemy])
        {
            Enemy_Scr enemyOnPos = Field_Scr.GetMapCell(mapPosition).enemyRef;
            _ = enemyOnPos.Raise();
        }
        Transform blockModel = transform.GetChild(0);
        Field_Scr.GetMapCell(mapPosition).objID = DataBase_Scr.IDDict[DataBase_Scr.IDkeys.corpse];
        await AnimationsDB_Scr.instance.DBRaiseAnim(blockModel, new Vector3(0, 0, 0));
    }
}
