using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoOpenScene
{
    static AutoOpenScene()
    {
        EditorApplication.delayCall += OpenSpaceShooterScene;
    }

    static void OpenSpaceShooterScene()
    {
        const string scenePath = "Assets/Scenes/SpaceShooter.unity";
        if (EditorSceneManager.GetActiveScene().path == scenePath) return;
        if (EditorSceneManager.GetActiveScene().isDirty)
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        if (AssetDatabase.AssetPathExists(scenePath))
            EditorSceneManager.OpenScene(scenePath);
    }

    [MenuItem("Assets/Setup/Sahneyi Ac: SpaceShooter")]
    static void ManualOpen() => OpenSpaceShooterScene();
}