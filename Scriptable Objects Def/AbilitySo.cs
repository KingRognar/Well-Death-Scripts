using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities", order = 1)]
public class AbilitySO : ScriptableObject
{
    public Sprite abilityIcon;

    public TargetingType targetingType = TargetingType.Ground;
    public int range = 1;
    public int targetsCount = 1;

    public AreaType areaType = AreaType.SingleCell;
    public int areaSize = 1;

    // TODO: постоянно слетает - надо исправить, мб не использовать кортежи?
    public (bool isUsed, int min, int max)[] damageTypes = new (bool, int, int)[4];
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
        Diamond,
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