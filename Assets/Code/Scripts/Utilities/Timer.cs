using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private bool timerStarted = false;
    private float elapsedTime = 0.0f;

    public void StartTimer()
    {
        timerStarted = true;
    }

    private void Update()
    {
        if (!timerStarted) return;

        elapsedTime += Time.deltaTime;

        string minutes = ((int)elapsedTime / 60).ToString();
        string seconds = (elapsedTime % 60).ToString("f2");

        timerText.text = minutes + ":" + seconds;
    }
}