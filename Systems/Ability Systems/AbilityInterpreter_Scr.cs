using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AbilityInterpreter_Scr : MonoBehaviour
{
    public static AbilityInterpreter_Scr instance;

    public List<Vector3Int> targetsList = new List<Vector3Int>();
    public List<Enemy_Scr> enemiesList = new List<Enemy_Scr>();

    public int abilityDamage = 4;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }


    // TODO: привз€ать к √√ и к TBS

    public async void ExecuteAbility()
    {
        GetEnemiesList();
        InputController_Scr.instance.isAwaitingInput = false;
        await DamageEnemies(); // TODO: должен подождать пока все враги получат урон
        InputController_Scr.instance.isAwaitingTurn = false;
    }
    private void GetEnemiesList()
    {
        enemiesList = new List<Enemy_Scr>();
        foreach (Vector3Int target in targetsList)
            if (Field_Scr.GetMapCell(target).enemyRef != null)
                enemiesList.Add(Field_Scr.GetMapCell(target).enemyRef);
    }
    private async Task DamageEnemies()
    {
        // TODO: сделать, чтобы TakeDamage выполн€лись синхронно
        foreach (Enemy_Scr enemy in enemiesList)
            await enemy.TakeDamage(abilityDamage, targetsList[0]);
    }
}
