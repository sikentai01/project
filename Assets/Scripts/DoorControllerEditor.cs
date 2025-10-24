#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorController))]
public class DoorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- �M�~�b�N��� ---
        var gimmickID = serializedObject.FindProperty("gimmickID");
        var currentStage = serializedObject.FindProperty("currentStage");

        if (gimmickID != null && currentStage != null)
        {
            EditorGUILayout.LabelField("�M�~�b�N�ݒ�", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(gimmickID);
            EditorGUILayout.PropertyField(currentStage);
        }
        else
        {
            EditorGUILayout.HelpBox("�e�N���X(GimmickBase)�̃M�~�b�N�����g�p��", MessageType.Info);
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
        EditorGUILayout.LabelField("���ݒ�", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredKeyID"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("�J����", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredDirection"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("���o�ݒ�", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("doorAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("openSE"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lockedSE"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unlockSE"));

        // �V�X�e���e�L�X�g�����݂��邩���S�`�F�b�N
        var systemTextWhenLocked = serializedObject.FindProperty("systemTextWhenLocked");
        var systemTextWhenUnlocked = serializedObject.FindProperty("systemTextWhenUnlocked");

        if (systemTextWhenLocked != null || systemTextWhenUnlocked != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("�V�X�e���e�L�X�g", EditorStyles.boldLabel);
            if (systemTextWhenLocked != null)
                EditorGUILayout.PropertyField(systemTextWhenLocked);
            if (systemTextWhenUnlocked != null)
                EditorGUILayout.PropertyField(systemTextWhenUnlocked);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
