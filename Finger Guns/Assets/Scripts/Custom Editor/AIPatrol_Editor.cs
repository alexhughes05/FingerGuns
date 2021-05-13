using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
[CustomEditor(typeof(AIPatrol))]
public class AIPatrol_Editor : Editor
{
    SerializedProperty patrolling;
    SerializedProperty onPlatform;
    SerializedProperty walkSpeed;
    SerializedProperty turnAroundDistance;
    AIPatrol script;

    void OnEnable()
    {
        script = (AIPatrol)target;
        patrolling = serializedObject.FindProperty("patrolling");
        onPlatform = serializedObject.FindProperty("onPlatform");
        walkSpeed = serializedObject.FindProperty("walkSpeed");
        turnAroundDistance = serializedObject.FindProperty("turnAroundDistance");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields

        serializedObject.Update();
        EditorGUILayout.Space();

        patrolling.boolValue = EditorGUILayout.Toggle(patrolling.displayName, patrolling.boolValue);
        if (patrolling.boolValue)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                walkSpeed.floatValue = EditorGUILayout.FloatField(walkSpeed.displayName, walkSpeed.floatValue);
            }
        }

        onPlatform.boolValue = EditorGUILayout.Toggle(onPlatform.displayName, onPlatform.boolValue);
        if (!onPlatform.boolValue)
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