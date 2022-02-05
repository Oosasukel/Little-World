using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetGenerator))]
public class PlanetGeneratorEditor : Editor
{

    void OnEnable()
    {
        // Setup the SerializedProperties.
        // m_Material = serializedObject.FindProperty("m_Material");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("Test"))
        {
            (serializedObject.targetObject as PlanetGenerator).Test();
        }

        serializedObject.ApplyModifiedProperties();
    }
}