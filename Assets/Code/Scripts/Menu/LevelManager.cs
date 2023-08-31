using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject _panelNet;
    public GameObject _panelSplash;
    public void QuitGame()
    {
        Application.Quit();
    }
    private void Start()
    {
        _panelNet.SetActive(false);
    }
    private void Update()
    {
        if (_panelNet== true && Input.GetKeyDown(KeyCode.Escape))
        {
            _panelNet.SetActive(false);
        }
    }
    
    public void PlayGame()
    {
        _panelNet.SetActive(true);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
    public void Splash()
    {
        _panelSplash.SetActive(false);
    }
}