using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusicToggle : MonoBehaviour
{
    [SerializeField] private Image _soundOnImage;
    [SerializeField] private Image _soundMutedImage;

    private bool _musicOn;

    private void Awake()
    {
        _musicOn = true;
    }

    public void Toggle()
    {
        _musicOn = !_musicOn;
        ApplicationEvents.InvokeMusicStateChange(_musicOn);

        _soundOnImage.enabled = _musicOn;
        _soundMutedImage.enabled = !_musicOn;
    }
}
