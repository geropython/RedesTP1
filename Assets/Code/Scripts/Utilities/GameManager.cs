using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //cada vez que actualizo una vuelta, se envia un RPC a todo el mundo para actualiar la cantidad de vueltas de cada auto.
    public GameObject _panelWin;

    public static GameManager Instance { get; private set; }
    private int laps = 0;
    //public CountDownTimer countdownTimer;

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

    //STARTS COUNTDOWN TIMER: --> ESPERA A LOS 3 JUGADORES PARA COMENZAR. HACER RPC
    private void Start()
    {
        //StartCoroutine(countdownTimer.StartCountdown());
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
    //NEVIAR ULONG A RPC DE WIN PARA DEMOSTRAR QUIEN GANO LA PARTIDA. NOTIFICAR.
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