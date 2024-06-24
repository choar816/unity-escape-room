using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RecognizeItemWithAIBarracuda))]
[CanEditMultipleObjects]
public class Editor_RecognizeItemWithAIBarracuda : Editor
{
    public RecognizeItemWithAIBarracuda aIBarracuda;

    private void OnEnable()
    {
        aIBarracuda = target as RecognizeItemWithAIBarracuda;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Read class txt"))
            aIBarracuda.ReadText();

        base.OnInspectorGUI();
    }
}
