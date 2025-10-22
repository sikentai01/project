#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorController))]
public class DoorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("ギミック設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gimmickID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentStage"));

        EditorGUILayout.Space();
        var moveTypeProp = serializedObject.FindProperty("moveType");
        EditorGUILayout.PropertyField(moveTypeProp);
        var moveType = (DoorController.DoorMoveType)moveTypeProp.enumValueIndex;

        if (moveType == DoorController.DoorMoveType.ChangeScene)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetScene"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPointName"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetPoint"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("鍵設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredKeyID"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("開閉方向", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredDirection"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("演出設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("doorAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("openSE"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lockedSE"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unlockSE"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("システムテキスト", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("systemTextWhenLocked"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("systemTextWhenUnlocked"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif