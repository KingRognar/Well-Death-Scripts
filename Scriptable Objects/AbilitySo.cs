using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Abilities", order = 1)]
public class AbilitySO: ScriptableObject
{
    public TargetingType targetingType;
    [HideInInspector] public int range;
    [HideInInspector] public int targetsCount;
}

namespace Abilities
{
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