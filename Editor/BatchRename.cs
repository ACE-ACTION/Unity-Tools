using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public enum ObjectTypes
{
    Objects,
    AniamtionClip,
    AudioClip,
    Font,
    Material,
    Model,
    Prefab,
    Scene,
    Shader,
    Sprite,
    Texture
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
    List<Action> drawGUIActions = new List<Action>();

    [MenuItem("Tools/Batch Rename %F2")]
    public static void ShowWindow()
    {
        BatchRename batchRenameWindow = GetWindow<BatchRename>("Batch Rename");
        batchRenameWindow.AddDrawActions();
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
        drawGUIActions[(int)actionTypes]();
        
        if (GUILayout.Button("OK!"))
        {
            SpecifyObjects();
            Rename();
        }
    }

    string AddPrefix(Object assetObject)
    {
        return $"{newName} {assetObject.name}";
    }

    string AddSuffix(Object assetObject)
    {
        return $"{assetObject.name} {newName}";
    }
    
    string GetNewName()
    {
        return newName;
    }

    void Rename()
    {

        if (selectedEnabaled)
        {
            foreach (var assetObject in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(assetObject);
                string newName = prefixToggleSelected ? AddSuffix(assetObject) : suffixToggleSelected ? AddSuffix(assetObject) : GetNewName();
                string pathWithNewName = path.Replace(assetObject.name, newName);
                pathWithNewName = AssetDatabase.GenerateUniqueAssetPath(pathWithNewName);
                string pathWithoutFileName = path.Replace(assetObject.name, string.Empty);
                newName = pathWithNewName.Replace(pathWithoutFileName, string.Empty);
                AssetDatabase.RenameAsset(path, newName);
            }
        }
        else
        {
            //
        }
    }

    List<string> objectsPath = new List<string>(); // for All enabled
    List<Object> selectedObjects = new List<Object>(); // for selected enabled

    void SpecifyObjects()
    {
        if (selectedEnabaled)
        {
            System.Type type = System.Type.GetType(objectTypes.ToString());
            selectedObjects = Selection.GetFiltered(type, SelectionMode.Editable).ToList();
        }
        else
        {
            string filter = $"t:{objectTypes.ToString()} ";
            if (actionTypes == ActionTypes.FindAndReplace)
                filter += findName;
            objectsPath = AssetDatabase.FindAssets(filter).ToList();
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
    string newName;

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
        newName = GUILayout.TextField(newName);
        EditorGUILayout.EndHorizontal();
    }

    bool SwitchToggle(params bool[] otherToggles)
    {
        foreach (bool toggle in otherToggles)
            if (toggle)
                return false;
        return true;
    }
    
    void AddDrawActions()
    {
        drawGUIActions.Add(Draw_FindAndReplace_GUI);
        drawGUIActions.Add(Draw_SetName_GUI);
    }


}

