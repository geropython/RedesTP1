using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject _panelWin;
    public static GameManager Instance { get; private set; }
    private int laps = 0;
    public CountDownTimer countdownTimer;

    //SINGLETON PATTERN
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //STARTS COUNTDOWN TIMER:
    private void Start()
    {
        StartCoroutine(countdownTimer.StartCountdown());
    }

    public void IncreaseLap()
    {
        laps++;
        if (laps >= 3)
        {
            _panelWin.SetActive(true);
        }
    }

    public void Win()
    {
        SceneManager.LoadScene(0);
    }
    public void NextTrack()
    {
        //proximo nivel
    }
    public int GetLaps()
    {
        return laps;
    }
}
