using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TBS;

public class TBS_Scr : MonoBehaviour
{
    public static List<TBSToken> tokens = new List<TBSToken>();
    private static List<TBSToken> tokensToRemove = new List<TBSToken>();
    private static List <TBSToken> tokensToAdd = new List<TBSToken>();

    private void Start()
    {
        InitializeTBS();
        runTBS();
    }

    private void InitializeTBS()
    {
        TBSToken newToken = new TBSToken(10, Random.Range(0, 100), true); // TODO: вставлять скорость игрока
        Player_Scr.instance.tbsToken = newToken;
        tokens.Add(newToken); 
        foreach (Enemy_Scr enemy in Enemy_Scr.enemiesList)
        {
            newToken = new TBSToken(10, Random.Range(0, 100), false, enemy); // TODO: вставлять скорость врага
            enemy.tbsToken = newToken;
            tokens.Add(newToken);
        }
    }

    private async void runTBS()
    {
        while (!Input.GetKeyDown(KeyCode.P)) // защита от лупы
        {
            foreach(TBSToken token in tokens)
            {
                token.curActionTime += token.actionSpeed;
                if (token.curActionTime < 100)
                    continue;
                token.curActionTime = token.curActionTime % 100;

                if (token.isPalyerToken)
                {
                    Debug.Log("жду ход игрока");
                    await InputController_Scr.instance.AwaitTurn();
                } else {
                    //Debug.Log("жду ход " + token.enemyRef.name);
                    if (token.enemyRef != null)
                        await token.enemyRef.MakeTurn();
                }
            }
            RemoveTokensFromMainList();
            AddTokensToMainList();
        }
    }
    public static void AddToTokensToRemove(TBSToken tokenToRemove)
    {
        tokensToRemove.Add(tokenToRemove); 
    }
    public static void AddToTokensToAdd(TBSToken tokenToAdd)
    {
        tokensToAdd.Add(tokenToAdd);
    }
    private static void RemoveTokensFromMainList()
    {
        foreach (TBSToken token in tokensToRemove)
            tokens.Remove(token);
        tokensToRemove = new List<TBSToken>();
    }
    private static void AddTokensToMainList()
    {
        foreach (TBSToken token in tokensToAdd)
            tokens.Add(token);
        tokensToAdd = new List<TBSToken>();
    }

}

namespace TBS
{
    public class TBSToken
    {
        public int actionSpeed;
        public int curActionTime;
        public bool isPalyerToken;
        public Enemy_Scr enemyRef;

        public TBSToken(int _actionSpeed = 10, int _curActionTime = 0, bool _isPlayerToken = false, Enemy_Scr _enemyRef = null)
        {
            actionSpeed = _actionSpeed;
            curActionTime = _curActionTime;
            isPalyerToken = _isPlayerToken;
            enemyRef = _enemyRef;
        }
    }
}
