#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorController))]
public class DoorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredKeyID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unlockFlag"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("requiredDirection"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("doorAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("openSE"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lockedSE"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unlockSE"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("systemTextWhenLocked"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("systemTextWhenUnlocked"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif