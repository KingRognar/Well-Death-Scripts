using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationsDirector_Scr : MonoBehaviour
{
    public static AnimationsDirector_Scr instance;

    public ParryBar_Scr parryBar;

    Transform playerTransform;
    Animator playerAnimator;
    CreatureSoundController_Scr playerSoundCtrl;
    Vector3 playerPos;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void GetPlayer()
    {
        playerTransform = Player_Scr.instance.transform;
        playerAnimator = Player_Scr.instance.GetComponentInChildren<Animator>();
        playerSoundCtrl = Player_Scr.instance.GetComponent<CreatureSoundController_Scr>();
    }

    public async Task DirectPlayerGetHit(Transform enemyTransform, Animator enemyAnimator, CreatureSoundController_Scr enemySoundCtrl)
    {
        if (playerTransform == null || playerAnimator == null || playerSoundCtrl == null)
            GetPlayer();

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;

        Vector3 enemyStartPos = enemyTransform.position;

        parryBar.gameObject.SetActive(true);

        Task[] tasksList = new Task[2];
        tasksList[0] = AnimationsDB_Scr.instance.DBAttackPrepAnim(enemyTransform, enemyAnimator, Field_Scr.MapToWorldPosition(Field_Scr.playerMapPos), enemySoundCtrl, token);
        tasksList[1] = parryBar.TryToCounter();
        await Task.WhenAny(tasksList);


        bool result = parryBar.lastResult;
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        token = cancellationTokenSource.Token;

        enemyTransform.position = enemyStartPos;

        tasksList = new Task[2];
        if (result)
        {
            tasksList[0] = AnimationsDB_Scr.instance.DBParryAnim(playerTransform, playerAnimator, enemyTransform.position, playerSoundCtrl);
        } else
        {
            tasksList[0] = AnimationsDB_Scr.instance.DBGetHitAnim(playerTransform, playerAnimator, enemyTransform.position, playerSoundCtrl);
        }
        tasksList[1] = AnimationsDB_Scr.instance.DBAttackOnlyAnim(enemyTransform, enemyAnimator, Field_Scr.MapToWorldPosition(Field_Scr.playerMapPos), enemySoundCtrl, token);
        await Task.WhenAll(tasksList);

        enemyTransform.position = enemyStartPos;
    }

}
