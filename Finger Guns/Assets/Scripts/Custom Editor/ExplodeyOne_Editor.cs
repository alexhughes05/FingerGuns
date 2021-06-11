#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
[CustomEditor(typeof(ExplodeyOne))]
public class ExplodeyOne_Editor : Editor
{
    ExplodeyOne script;
    SerializedProperty explosionDelay;
    SerializedProperty lockOntoPlayer;

    void OnEnable()
    {
        script = (ExplodeyOne)target;
        explosionDelay = serializedObject.FindProperty("explosionDelay");
        lockOntoPlayer = serializedObject.FindProperty("lockOntoPlayer");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // for other non-HideInInspector fields
        serializedObject.Update();

        EditorGUILayout.Space();

        lockOntoPlayer.boolValue = EditorGUILayout.Toggle(lockOntoPlayer.displayName, lockOntoPlayer.boolValue);
        if (!lockOntoPlayer.boolValue)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                explosionDelay.floatValue = EditorGUILayout.FloatField(explosionDelay.displayName, explosionDelay.floatValue);

            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
