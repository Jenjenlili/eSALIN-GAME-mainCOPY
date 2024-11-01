using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // To load new scenes

public class VideoSceneSwitcher : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public string nextSceneName;    // Name of the next scene to load after the video ends

    void Start()
    {
        // Subscribe to the loopPointReached event
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Load the next scene when the video ends
        SceneManager.LoadScene(nextSceneName);
    }
}
