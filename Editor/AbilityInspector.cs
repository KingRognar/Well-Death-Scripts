using System;
using UnityEngine;
using UnityEditor;
using Abilities;

[CustomEditor(typeof(AbilitySO))]
public class AbilityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //OldVersion();
        NewVersion();
    }

    private void OldVersion()
    {
        /*//base.OnInspectorGUI(); // DO NOT DELETE FOR NOW

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

        EditorUtility.SetDirty(abilitySO);*/
    }

    // TODO: добавить остальные параметры и вариативность, мб ещё каких плюшек насыпать 
    private void NewVersion()
    {
        serializedObject.Update();

        float padding = 10;

        ////---//// Визуал ////---////
        SerializedProperty iconSP = serializedObject.FindProperty("abilityIcon");
        Texture2D texture = AssetPreview.GetAssetPreview(iconSP.objectReferenceValue);
        if (texture != null) GUILayout.Label(texture, EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(padding);
        EditorGUILayout.PropertyField(iconSP);

        ////---//// Выбор цели ////---////
        EditorGUILayout.Space(padding);
        SerializedProperty targetingEnumSP = serializedObject.FindProperty("targetingType");
        EditorGUILayout.PropertyField(targetingEnumSP);
        if (targetingEnumSP.enumValueIndex == (int)TargetingType.Ground)
        {
            GUILayout.Label("описание?");
        }

        ////---//// Область действия ////---////
        EditorGUILayout.Space(padding);
        SerializedProperty areaEnumSP = serializedObject.FindProperty("areaType");
        EditorGUILayout.PropertyField(areaEnumSP);
        if (areaEnumSP.enumValueIndex != (int)AreaType.SingleCell && 
            areaEnumSP.enumValueIndex != (int)AreaType.Line)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("areaSize"));
        }

        ////---//// Урон ////---////
        EditorGUILayout.Space(padding);
        GUILayout.Label("Damage", EditorStyles.boldLabel);

        SerializedProperty dmgTypesSP = serializedObject.FindProperty("damageTypes");

        string[] damageNames = new string[4] { "Phy", "Fir", "Lig", "Ice" };
        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 30;
        for (int i = 0; i < dmgTypesSP.arraySize; i++)
        {
            EditorGUILayout.PropertyField(dmgTypesSP.GetArrayElementAtIndex(i), new GUIContent(damageNames[i]), GUILayout.MaxWidth(100));
        }
        EditorGUIUtility.labelWidth = 0;
        GUILayout.EndHorizontal();

        SerializedProperty dmgNumbersSP = serializedObject.FindProperty("damageNumbers");
        for (int i = 0; i < dmgNumbersSP.arraySize/2; i++)
        {
            if (!dmgTypesSP.GetArrayElementAtIndex(i).boolValue)
                continue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(damageNames[i]);
            EditorGUIUtility.labelWidth = 30;
            EditorGUILayout.PropertyField(dmgNumbersSP.GetArrayElementAtIndex(i), new GUIContent("Min"), GUILayout.MaxWidth(150));
            EditorGUILayout.PropertyField(dmgNumbersSP.GetArrayElementAtIndex(i + 4), new GUIContent("Max"), GUILayout.MaxWidth(150));
            EditorGUIUtility.labelWidth = 0;
            GUILayout.EndHorizontal();
        }

        FixDamageValues(dmgNumbersSP);

        ////---//// Статусные эффекты ////---////
        EditorGUILayout.Space(padding);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("statusEffects"));

        serializedObject.ApplyModifiedProperties();

    }

    private void FixDamageValues(SerializedProperty dmgSP)
    {
        for (int i = 0; i < dmgSP.arraySize; i++)
        {
            dmgSP.GetArrayElementAtIndex(i).intValue = Mathf.Max(0, dmgSP.GetArrayElementAtIndex(i).intValue);
        }
    }
}
