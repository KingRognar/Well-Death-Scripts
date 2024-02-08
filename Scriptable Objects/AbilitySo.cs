using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities", order = 1)]
public class AbilitySO : ScriptableObject
{
    public Texture2D abilityIcon;

    public TargetingType targetingType = TargetingType.Ground;
    public int range = 1;
    public int targetsCount = 1;

    public AreaType areaType = AreaType.SingleCell;
    public int areaSize = 1;

    public (bool isUsed, string name, int min, int max)[] damageTypes = new (bool, string, int, int)[4]
    {
        (false, "Физ", 0,0), (false, "Огн", 0, 0), (false, "Эле", 0, 0), (false, "Хол", 0, 0)
    };
    //public bool[] damageTypes = new bool[4];
    // 0 - физический урон
    // 1 - огненный урон
    // 2 - электрический урон
    // 3 - урон холодом

    public List<StatusEffectData> statusEffects = new List<StatusEffectData>();
}

namespace Abilities
{
    public enum TargetingType
    {
        Ground,
        Enemy,
        MultipleEnemy,
        Direction,
        Yourself
    }
    public enum AreaType
    {
        SingleCell,
        Line,
        Cone,
        Circle,
        Sphere
    }
    public enum StatusEffectType
    {
        Burn,
        Poison,
        Hex,
        Slow
    }
    [System.Serializable]
    public class StatusEffectData
    {
        public StatusEffectType statusEffectType;
        public int duration;
    }

    
}