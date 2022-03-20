using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ScenesToBuild : EditorWindow
{
    List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
    bool enabledAssetsinBuild = true;

    [MenuItem("Tools/Include Scenes to Build")]
    public static void ShowWindow()
    {
        GetWindow<ScenesToBuild>("Scenes to Build");
    }

    private void OnGUI()
    {
        enabledAssetsinBuild = EditorGUILayout.Toggle("Enabled in Build", enabledAssetsinBuild);

        GUILayout.Label("Scenes to include in build: Selected Scenes");
        if (GUILayout.Button("Add Selected Scenes to Build."))
        {
            editorBuildSettingsScenes = EditorBuildSettings.scenes.ToList();
            AddScenesIntoScenesInBuild();
        }
    }

    private void AddScenesIntoScenesInBuild()
    {
        foreach (var sceneItem in Selection.objects)
        {
            SceneAsset sceneAsset = (SceneAsset)sceneItem;
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

            bool AlreadyExistInBuild = false;

            foreach (var EditorBuildSettingScene in EditorBuildSettings.scenes)
            {
                if (EditorBuildSettingScene.path == scenePath)
                    AlreadyExistInBuild = true;
            }
            if (!AlreadyExistInBuild)
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, enabledAssetsinBuild));            
        }

        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
    }
}
