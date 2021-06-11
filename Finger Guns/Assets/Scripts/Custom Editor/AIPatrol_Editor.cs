using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
[CustomEditor(typeof(AIPatrol))]
public class AIPatrol_Editor : Editor
{
    SerializedProperty detectionRange;
    SerializedProperty patrolling;
    SerializedProperty nearestEdge;
    SerializedProperty walkSpeed;
    SerializedProperty turnAroundDistance;
    AIPatrol script;

    void OnEnable()
    {
        script = (AIPatrol)target;
        detectionRange = serializedObject.FindProperty("detectionRange");
        patrolling = serializedObject.FindProperty("patrolling");
        nearestEdge = serializedObject.FindProperty("walkToNearestEdge");
        walkSpeed = serializedObject.FindProperty("walkSpeed");
        turnAroundDistance = serializedObject.FindProperty("turnAroundDistance");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields
        serializedObject.Update();

        EditorGUILayout.Space();

        detectionRange.floatValue = EditorGUILayout.Slider(detectionRange.displayName, detectionRange.floatValue, 0, 100);

        EditorGUILayout.Space();

        patrolling.boolValue = EditorGUILayout.Toggle(patrolling.displayName, patrolling.boolValue);
        if (patrolling.boolValue)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                walkSpeed.floatValue = EditorGUILayout.FloatField(walkSpeed.displayName, walkSpeed.floatValue);
            }
        }

        EditorGUILayout.Space();

        nearestEdge.boolValue = EditorGUILayout.Toggle(nearestEdge.displayName, nearestEdge.boolValue);
        if (!nearestEdge.boolValue)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                turnAroundDistance.floatValue = EditorGUILayout.FloatField(turnAroundDistance.displayName, turnAroundDistance.floatValue);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif