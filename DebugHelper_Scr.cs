using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper_Scr : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private Vector3 spawnPosition = new Vector3(4, 3, 4);

    public AbilitySO abilitySO;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        Vector3Int mapPosition = Field_Scr.WorldToMapPosition(spawnPosition);
        if (Field_Scr.GetMapCell(mapPosition).objID != 0)
            spawnPosition += new Vector3(0, 1, 0);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Enemy_Scr spawnedEnemyScr = newEnemy.GetComponent<Enemy_Scr>();
        Field_Scr.AddEnemy(spawnedEnemyScr); // TODO: можно ли переместить эту шл€пу в сам Enemy_Scr?

        TBS.TBSToken newToken = new TBS.TBSToken(10, Random.Range(0, 100), false, spawnedEnemyScr); // TODO: мб сделать как-то лучше
        spawnedEnemyScr.tbsToken = newToken;
        TBS_Scr.AddToTokensToAdd(newToken);
    }
}
