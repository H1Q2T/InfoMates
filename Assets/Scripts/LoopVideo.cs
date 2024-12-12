using UnityEngine;
using UnityEngine.Video;

public class LoopVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.isLooping = true; // Habilitar el bucle por código
        videoPlayer.Play();
    }
}
