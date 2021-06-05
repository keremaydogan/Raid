using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovementBasics))]
[CanEditMultipleObjects]
public class MovementBasicsEditor : Editor
{
    SerializedProperty inpMngSelection;
    SerializedProperty playerControls;
    SerializedProperty enemyTag;
    SerializedProperty absSightLen, absSightLayers, norSightLen, norSightLayers, memSightLen;
    SerializedProperty walkSpeed, crouchEffector;

    SerializedProperty showGizmos;
    
    private void OnEnable()
    {
        inpMngSelection = serializedObject.FindProperty("inpMngSelection");
        playerControls = serializedObject.FindProperty("playerControls");

        enemyTag = serializedObject.FindProperty("enemyTag");

        absSightLen = serializedObject.FindProperty("absSightLen");
        absSightLayers = serializedObject.FindProperty("absSightLayers");
        norSightLen = serializedObject.FindProperty("norSightLen");
        norSightLayers = serializedObject.FindProperty("norSightLayers");
        memSightLen = serializedObject.FindProperty("memSightLen");

        walkSpeed = serializedObject.FindProperty("walkSpeed");
        crouchEffector = serializedObject.FindProperty("crouchEffector");

        showGizmos = serializedObject.FindProperty("showGizmos");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(inpMngSelection);

        EditorGUILayout.PropertyField(enemyTag);

        if (inpMngSelection.intValue == 0)
        {
            EditorGUILayout.PropertyField(playerControls);
        }

        if (inpMngSelection.intValue == 1)
        {
            EditorGUILayout.PropertyField(absSightLen);
            EditorGUILayout.PropertyField(absSightLayers);
            EditorGUILayout.PropertyField(norSightLen);
            EditorGUILayout.PropertyField(norSightLayers);
            EditorGUILayout.PropertyField(memSightLen);
        }

        EditorGUILayout.PropertyField(walkSpeed);
        EditorGUILayout.PropertyField(crouchEffector);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(showGizmos);

        serializedObject.ApplyModifiedProperties();
    }
}
