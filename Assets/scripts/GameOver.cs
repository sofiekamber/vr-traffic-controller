using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    [SerializeField]
    private GameObject GameOverOverlay;

    

    private IEnumerator WaitTimeGameOver()
    {

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSecondsRealtime(5);
        //unfreeze game
        Time.timeScale = 1;
        // load main menu
        SceneManager.LoadScene(0);
        // reset counter
        Score.counter = 0;
        // reset lanes
        for (int i = 0; i < userAction.lane_stop.Length; i++)
        {
            userAction.lane_stop[i] = false;
        }
    }

    public void finito()
    {
        GameOverOverlay.SetActive(true);
        //freeeze game
        Time.timeScale = 0;

        //wait 5 seconds
        StartCoroutine(WaitTimeGameOver());
    }
}
