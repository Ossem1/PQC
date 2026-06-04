using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ObjectBase), true)]
public class InspectorModifier : Editor
{
    SerializedProperty isMovingProp;
    SerializedProperty pointsProp;

    SerializedProperty isMovingProp_q;

    SerializedProperty movingSpeedProp;

    private readonly string[] filter = { "isMoving", "points", "isMoving_q", "movingSpeed" };

    void OnEnable()
    {
        isMovingProp = serializedObject.FindProperty("isMoving");
        pointsProp = serializedObject.FindProperty("points");

        isMovingProp_q = serializedObject.FindProperty("isMoving_q");
        movingSpeedProp = serializedObject.FindProperty("movingSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            if (System.Array.Exists(filter, name => name == iterator.name))
            {
                if (iterator.name == "isMoving")
                {
                    EditorGUILayout.PropertyField(iterator);

                    if (iterator.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("points"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("movingSpeed"));
                        EditorGUI.indentLevel--;
                    }
                }
                if (iterator.name == "isMoving_q")
                {
                    EditorGUILayout.PropertyField(iterator);

                    if (iterator.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("points"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("movingSpeed"));
                        EditorGUI.indentLevel--;
                    }
                }
            }
            else
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
            enterChildren = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
