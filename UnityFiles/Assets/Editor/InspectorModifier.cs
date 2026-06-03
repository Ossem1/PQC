using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ObjectBase), true)]
public class InspectorModifier : Editor
{
    SerializedProperty isMovingProp;
    SerializedProperty point1Prop;
    SerializedProperty point2Prop;

    SerializedProperty isMovingProp_q;
    SerializedProperty point1Prop_q;
    SerializedProperty point2Prop_q;

    private readonly string[] filter = { "isMoving", "point1", "point2", "isMoving_q", "point1_q", "point2_q" };

    void OnEnable()
    {
        isMovingProp = serializedObject.FindProperty("isMoving");
        point1Prop = serializedObject.FindProperty("point1");
        point2Prop = serializedObject.FindProperty("point2");

        isMovingProp_q = serializedObject.FindProperty("isMoving_q");
        point1Prop_q = serializedObject.FindProperty("point1_q");
        point2Prop_q = serializedObject.FindProperty("point2_q");
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
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("point1"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("point2"));
                        EditorGUI.indentLevel--;
                    }
                }
                if (iterator.name == "isMoving_q")
                {
                    EditorGUILayout.PropertyField(iterator);

                    if (iterator.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("point1_q"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("point2_q"));
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
