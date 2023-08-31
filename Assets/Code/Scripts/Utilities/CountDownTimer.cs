using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI countdownText;
   [SerializeField] private CarController carController;

    public IEnumerator StartCountdown()
    {
        carController.enabled = false;

        countdownText.text = "3";
        yield return new WaitForSeconds(1);

        countdownText.text = "2";
        yield return new WaitForSeconds(1);

        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        countdownText.text = "GO!";
        carController.enabled = true;

        yield return new WaitForSeconds(1);

        countdownText.text = "";
    }


}
