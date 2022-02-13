using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroundGenerator))]
public class GroundGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("Generate Ground"))
        {
            (serializedObject.targetObject as GroundGenerator).GenerateGround();
        }

        if (GUILayout.Button("Delete Ground"))
        {
            (serializedObject.targetObject as GroundGenerator).DeleteGround();
        }

        if (GUILayout.Button("Hide Ground"))
        {
            (serializedObject.targetObject as GroundGenerator).HideGround();
        }

        if (GUILayout.Button("Show Ground"))
        {
            (serializedObject.targetObject as GroundGenerator).ShowGround();
        }

        serializedObject.ApplyModifiedProperties();
    }
}