using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities", order = 1)]
public class AbilitySO: ScriptableObject
{
    public TargetingType targetingType;
    public Targeting targeting;
}

namespace Abilities
{
    [System.Serializable]
    public class Targeting
    {
        public int range;
        public int targetsCount;
    }

    public enum TargetingType
    {
        Ground,
        Enemy,
        MultipleEnemy,
        Direction
    }
    public enum AreaType
    {

    }
}