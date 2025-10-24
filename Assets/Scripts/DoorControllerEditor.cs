#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorController))]
public class DoorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- ギミック情報 ---
        var gimmickID = serializedObject.FindProperty("gimmickID");
        var currentStage = serializedObject.FindProperty("currentStage");

        if (gimmickID != null && currentStage != null)
        {
            EditorGUILayout.LabelField("ギミック設定", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(gimmickID);
            EditorGUILayout.PropertyField(currentStage);
        }
        else
        {
            EditorGUILayout.HelpBox("親クラス(GimmickBase)のギミック情報を使用中", MessageType.Info);
        }

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

        // システムテキストが存在するか安全チェック
        var systemTextWhenLocked = serializedObject.FindProperty("systemTextWhenLocked");
        var systemTextWhenUnlocked = serializedObject.FindProperty("systemTextWhenUnlocked");

        if (systemTextWhenLocked != null || systemTextWhenUnlocked != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("システムテキスト", EditorStyles.boldLabel);
            if (systemTextWhenLocked != null)
                EditorGUILayout.PropertyField(systemTextWhenLocked);
            if (systemTextWhenUnlocked != null)
                EditorGUILayout.PropertyField(systemTextWhenUnlocked);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
