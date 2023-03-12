using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChatGPTForUnity))]
public class ChatGPTForUnityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChatGPTForUnity script = (ChatGPTForUnity)target;

        GUILayout.Space(15);

        if (GUILayout.Button("Ask"))
        {
            script.SendRequest();
        }

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Script"))
        {
            script.SaveScript();
        }

        if (GUILayout.Button("Clear"))
        {
            script.Clear();
        }

        GUILayout.EndHorizontal();
    }

}
