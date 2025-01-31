using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private string _activeScene;
    private List<string> _scenesLoads = new List<string>();

    public string ActiveScene => _activeScene;

    public async void LoadScene(string sceneName, bool isAdditive = false)
    {
        if (!isAdditive)
        {
            _scenesLoads.Clear();
        }

        _activeScene = sceneName;
        Debug.Log($"[GAME] Loading scene:{sceneName}, is additive: {isAdditive}");
        _scenesLoads.Add(sceneName);
        SceneManager.LoadScene(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);

        await UniTask.WaitUntil(() => IsFullySceneLoaded(sceneName));
        SetActiveScene(sceneName);
    }

    public AsyncOperation LoadSceneAsync(string sceneName, bool isAdditive = false)
    {
        Debug.Log($"[GAME] Loading scene:{sceneName}, is additive: {isAdditive}");

        if (!isAdditive)
        {
            _scenesLoads.Clear();
        }

        _activeScene = sceneName;
        _scenesLoads.Add(sceneName);

        var loadOp = SceneManager.LoadSceneAsync(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        loadOp.completed += (AsyncOperation) =>
        {
            SetActiveScene(sceneName);
        };

        return loadOp;
    }

    public void SetSceneActiveState(string sceneName, bool newState)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        foreach (var item in scene.GetRootGameObjects())
        {
            item.gameObject.SetActive(newState);
        }
    }

    public void SetActiveScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
    }

    public bool IsFullySceneLoaded(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }

    public AsyncOperation UnloadScene(string sceneName)
    {
        _scenesLoads.Remove(sceneName);
        return SceneManager.UnloadSceneAsync(sceneName);
    }

    public static string SceneIdToString(SceneId sceneId)
    {
        switch (sceneId)
        {
            case SceneId.Preload:
                return "00.Preload";
            case SceneId.Lobby:
                return "01.Lobby";
            case SceneId.Demo1:
                return "Demo1";
            case SceneId.Demo2:
                return "Demo2";
            default:
                return "00.Preload";
        }
    }

    public static SceneId SceneNameToId(string sceneName)
    {
        switch (sceneName)
        {
            case "00.Preload":
                return SceneId.Preload;
            case "01.Lobby":
                return SceneId.Lobby;
            case "Demo1":
                return SceneId.Demo1;
            case "Demo2":
                return SceneId.Demo2;
            default:
                return SceneId.Preload;
        }
    }

    public bool IsSceneLoaded(string sceneName)
    {
        return _scenesLoads.Contains(sceneName);
    }
}
