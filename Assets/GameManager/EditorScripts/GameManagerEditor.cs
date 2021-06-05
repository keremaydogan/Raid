using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]

public class GameManagerEditor : Editor
{
    SerializedProperty p1UpKey, p1DownKey, p1RightKey, p1LeftKey, p1AttackKey, p1CrouchKey;
    SerializedProperty p2UpKey, p2DownKey, p2RightKey, p2LeftKey, p2AttackKey, p2CrouchKey;

    protected static bool showPContr;
    protected static bool showP1Contr;
    protected static bool showP2Contr;
    private void OnEnable()
    {
        p1UpKey = serializedObject.FindProperty("p1UpKey");
        p1DownKey = serializedObject.FindProperty("p1DownKey");
        p1RightKey = serializedObject.FindProperty("p1RightKey");
        p1LeftKey = serializedObject.FindProperty("p1LeftKey");
        p1AttackKey = serializedObject.FindProperty("p1AttackKey");
        p1CrouchKey = serializedObject.FindProperty("p1CrouchKey");

        p2UpKey = serializedObject.FindProperty("p2UpKey");
        p2DownKey = serializedObject.FindProperty("p2DownKey");
        p2RightKey = serializedObject.FindProperty("p2RightKey");
        p2LeftKey = serializedObject.FindProperty("p2LeftKey");
        p2AttackKey = serializedObject.FindProperty("p2AttackKey");
        p2CrouchKey = serializedObject.FindProperty("p2CrouchKey");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showPContr = EditorGUILayout.Foldout(showPContr, "Player Controls", true, EditorStyles.foldoutHeader);


        if (showPContr)
        {
            showP1Contr = EditorGUILayout.Foldout(showP1Contr, "Player 1 Controls", true, EditorStyles.foldoutHeader);

            if (showP1Contr)
            {
                EditorGUILayout.PropertyField(p1UpKey);
                EditorGUILayout.PropertyField(p1DownKey);
                EditorGUILayout.PropertyField(p1RightKey);
                EditorGUILayout.PropertyField(p1LeftKey);
                EditorGUILayout.PropertyField(p1AttackKey);
                EditorGUILayout.PropertyField(p1CrouchKey);
            }

            showP2Contr = EditorGUILayout.Foldout(showP2Contr, "Player 2 Controls", true, EditorStyles.foldoutHeader);

            if (showP2Contr)
            {
                EditorGUILayout.PropertyField(p2UpKey);
                EditorGUILayout.PropertyField(p2DownKey);
                EditorGUILayout.PropertyField(p2RightKey);
                EditorGUILayout.PropertyField(p2LeftKey);
                EditorGUILayout.PropertyField(p2AttackKey);
                EditorGUILayout.PropertyField(p2CrouchKey);
            }
        }
        


        serializedObject.ApplyModifiedProperties();
    }
}
