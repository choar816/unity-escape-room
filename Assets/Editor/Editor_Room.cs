using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
[CanEditMultipleObjects]
public class Editor_Room : Editor
{
    public Room room;

    private void OnEnable()
    {
        room = target as Room;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set"))
            room.CollectAllInteractiveDevices();

        if (GUILayout.Button("Random Set"))
            room.RandomSetInteractiveDevices();
    }
}
