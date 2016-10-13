using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    [SerializeField]
    private AudioClip[] BackSounds;

    [SerializeField]
    private AudioClip[] SoundEffects;

    public enum Music {walk,battle,boss}

    private AudioSource _audioSource;
	// Use this for initialization
	void Start () {
        _audioSource = GetComponent<AudioSource>();
	}
	
    public void SoundChange(Music scene)
    {
        _audioSource.clip = BackSounds[(int)scene];
        _audioSource.Play();
    }
}
