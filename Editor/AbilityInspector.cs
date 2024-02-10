using System;
using UnityEngine;
using UnityEditor;
using Abilities;

[CustomEditor(typeof(AbilitySO))]
public class AbilityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI(); // DO NOT DELETE FOR NOW

        AbilitySO abilitySO = (AbilitySO)target;

        ////---//// Визуал ////---////
        float iconPadding = 10;
        float iconSize = 100;
        Rect iconRect = new Rect(Screen.width / 2 - iconSize / 2, iconPadding, iconSize, iconSize);
        abilitySO.abilityIcon = (Sprite)EditorGUI.ObjectField(iconRect, abilitySO.abilityIcon, typeof(Sprite), false);
        EditorGUILayout.Space(iconSize + iconPadding * 2);

        ////---//// Выбор цели ////---////
        abilitySO.targetingType = (TargetingType)EditorGUILayout.EnumPopup("Targeting type", abilitySO.targetingType);
        if (abilitySO.targetingType == TargetingType.MultipleEnemy)
        {
            //GUILayout.Label("Testing");
            EditorGUILayout.HelpBox("Description for Multiple enemy targeting", MessageType.None);
            abilitySO.range = Mathf.Max(0, EditorGUILayout.IntField("Range", abilitySO.range));
            abilitySO.targetsCount = Mathf.Max(0, EditorGUILayout.IntField("Targets count", abilitySO.targetsCount));
        }

        ////---//// Область действия ////---////
        EditorGUILayout.Space();

        abilitySO.areaType = (AreaType)EditorGUILayout.EnumPopup("Area type", abilitySO.areaType);

        ////---//// Урон ////---////
        EditorGUILayout.Space();
        GUILayout.Label("Damage", EditorStyles.boldLabel);

        string[] damageNames = new string[4] { "Физ", "Огн", "Эле", "Хол" };
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Damage types:", GUILayout.MaxWidth(140));
        EditorGUIUtility.labelWidth = 30;
        for (int i = 0; i < abilitySO.damageTypes.Length; i++)
        {
            abilitySO.damageTypes[i].isUsed = EditorGUILayout.Toggle(damageNames[i], abilitySO.damageTypes[i].isUsed, GUILayout.MaxWidth(70));
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < abilitySO.damageTypes.Length; i++)
        {
            if (abilitySO.damageTypes[i].isUsed)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(damageNames[i], GUILayout.MaxWidth(40));
                abilitySO.damageTypes[i].min = EditorGUILayout.IntField("Min", abilitySO.damageTypes[i].min);
                abilitySO.damageTypes[i].max = EditorGUILayout.IntField("Max", abilitySO.damageTypes[i].max);
                abilitySO.damageTypes[i].max = Mathf.Max(0, abilitySO.damageTypes[i].max);
                abilitySO.damageTypes[i].min = Mathf.Clamp(abilitySO.damageTypes[i].min, 0, abilitySO.damageTypes[i].max);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUIUtility.labelWidth = 0;

        ////---//// Статусные эффекты ////---////
        EditorGUILayout.Space();

        GUILayout.Label("Status effects", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Number of status effects");
        int statusEffectsCount = Mathf.Max(0, EditorGUILayout.IntField(abilitySO.statusEffects.Count, GUILayout.MaxWidth(50)));
        EditorGUILayout.EndHorizontal();

        while (statusEffectsCount > abilitySO.statusEffects.Count)
            abilitySO.statusEffects.Add(new StatusEffectData());
        while (statusEffectsCount < abilitySO.statusEffects.Count)
            abilitySO.statusEffects.RemoveAt(abilitySO.statusEffects.Count - 1);
        
        for (int i = 0; i < abilitySO.statusEffects.Count; i++)
        {
            StatusEffectData statusEffect = abilitySO.statusEffects[i];
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            statusEffect.statusEffectType = (StatusEffectType)EditorGUILayout.EnumPopup("Type", statusEffect.statusEffectType);
            statusEffect.duration = EditorGUILayout.IntField("duration", statusEffect.duration);
            EditorGUILayout.EndVertical();
        }
        
        EditorUtility.SetDirty(abilitySO);
    }
}
