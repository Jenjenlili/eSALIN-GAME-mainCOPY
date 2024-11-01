using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseF : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void Pause() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Main() {
        SceneManager.LoadScene("LoadReturnMain");
        Time.timeScale = 1;
    }

    public void Resume() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void chooseDiff () {
        SceneManager.LoadScene("LoadReturnDiff");
        Time.timeScale = 1;
    }
}
