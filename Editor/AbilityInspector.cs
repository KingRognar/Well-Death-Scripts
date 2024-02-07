using UnityEngine;
using UnityEditor;
using Abilities;

[CustomEditor(typeof(AbilitySO))]
public class AbilityInspector : Editor
{
/*    Dictionary<string, Dictionary<string, bool>> showVariablesMap = new Dictionary<string, Dictionary<string, bool>>();
    string[] variablesNames = new string[] 
    {
        "range",
        "targets count",
        "area size",
        "damage"
    };

    private void OnEnable()
    {
        
    }*/

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI(); // DO NOT DELETE FOR NOW
        // TODO: разделить на блоки


        AbilitySO abilitySO = (AbilitySO)target;

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

/*    void InitializeShowVariablesMap (Dictionary<string, Dictionary<string, bool>> map)
    {
        foreach (string name in variablesNames)
        {
            map.Add(name,new)
        }
    }*/
}
