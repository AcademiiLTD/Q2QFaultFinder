using UnityEngine;

public class BackgroundMusicToggleListener : MonoBehaviour
{
    [SerializeField] private AudioSource _musicAudioSource;

    private void OnEnable()
    {
        ApplicationEvents.MusicStateChange += MusicStateChange;
    }

    private void OnDisable()
    {
        ApplicationEvents.MusicStateChange -= MusicStateChange;
    }

    private void MusicStateChange(bool musicOn)
    {
        _musicAudioSource.enabled = musicOn;
    }
}
