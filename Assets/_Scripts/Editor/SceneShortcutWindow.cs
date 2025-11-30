using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneShortcutWindow : EditorWindow
{
    [MenuItem("Tools/Scene Shortcuts")]
    public static void ShowWindow()
    {
        GetWindow<SceneShortcutWindow>("Scene Shortcuts");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Shortcuts", EditorStyles.boldLabel);

        var scenes = EditorBuildSettings.scenes;

        foreach (var scene in scenes)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

            if (GUILayout.Button(sceneName))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scene.path);
                }
            }
        }
    }
}

