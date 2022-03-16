using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum ObjectTypes
{
    Objects,
    Material,
    Mesh,
    Prefab,
    Scene,
    Sprite
}
public enum ActionTypes
{
    FindAndReplace,
    SetName
}

public class BatchRename : EditorWindow
{
    ObjectTypes objectTypes;
    ActionTypes actionTypes;

    delegate void Action();
    List<Action> DrawGUIActions = new List<Action>();

    [MenuItem("Tools/Batch Rename %F2")]
    public static void ShowWindow()
    {
        BatchRename batchRenameWindow = GetWindow<BatchRename>("Batch Rename");
        batchRenameWindow.AddActions();
    }

    bool selectedEnabaled = true;
    bool allEnabled = false;

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        selectedEnabaled = GUILayout.Toggle(SwitchToggle(allEnabled), "Selected");
        allEnabled = GUILayout.Toggle(SwitchToggle(selectedEnabaled), "All");
        objectTypes = (ObjectTypes)EditorGUILayout.EnumPopup(objectTypes);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Type");
        actionTypes = (ActionTypes)EditorGUILayout.EnumPopup(actionTypes);
        EditorGUILayout.EndHorizontal();

        //load GUI for selected action type
        DrawGUIActions[(Int32)actionTypes]();

        GUILayout.Label($"Rename {0} Object(s)");
        if (GUILayout.Button("Rename"))
        {
            // Adding Button Actions
        }
    }

    string findName;
    string replacedName;

    void Draw_FindAndReplace_GUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Find\t");
        findName = GUILayout.TextField(findName);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Replace\t");
        replacedName = GUILayout.TextField(replacedName);
        EditorGUILayout.EndHorizontal();
    }

    bool newToggleSelected = true;
    bool prefixToggleSelected = false;
    bool suffixToggleSelected = false;
    string inputName;

    void Draw_SetName_GUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Method");
        newToggleSelected = GUILayout.Toggle(SwitchToggle(prefixToggleSelected, suffixToggleSelected), "New");
        prefixToggleSelected = GUILayout.Toggle(SwitchToggle(newToggleSelected, suffixToggleSelected), "Prefix");
        suffixToggleSelected = GUILayout.Toggle(SwitchToggle(newToggleSelected, prefixToggleSelected), "Suffix");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        inputName = GUILayout.TextField(inputName);
        EditorGUILayout.EndHorizontal();

    }

    bool SwitchToggle(params bool[] otherToggles)
    {
        foreach (bool toggle in otherToggles)
            if (toggle)
                return false;
        return true;
    }
    
    void AddActions()
    {
        DrawGUIActions.Add(Draw_FindAndReplace_GUI);
        DrawGUIActions.Add(Draw_SetName_GUI);
    }

}

