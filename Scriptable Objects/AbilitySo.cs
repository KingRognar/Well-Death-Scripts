using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities", order = 1)]
public class AbilitySO: ScriptableObject
{
    public TargetingType targetingType = TargetingType.Ground;
    public int range = 1;
    public int targetsCount = 1;

    public AreaType areaType = AreaType.SingleCell;
    public int areaSize = 1;

    public int damage = 1;

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