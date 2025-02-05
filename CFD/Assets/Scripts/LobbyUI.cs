using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _project1;
    [SerializeField] private Button _project2;
    [SerializeField] private Button _project3;

    private SceneLoader _sceneLoader = new SceneLoader();

    private void Awake()
    {
        _project1.onClick.AddListener(()=> LoadScene("Demo1"));
        _project2.onClick.AddListener(()=> LoadScene("Demo2"));
        _project3.onClick.AddListener(()=> LoadScene("Demo3"));
    }

    private async void LoadScene(string scene)
    {
        var ceva = _sceneLoader.LoadSceneAsync(scene, true);
        await ceva;
        _sceneLoader.SetSceneActiveState("Lobby", false);
        _sceneLoader.SetActiveScene(scene);
    }
}
