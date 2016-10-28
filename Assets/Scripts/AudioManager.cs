using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    [SerializeField]
    private AudioClip[] BackSounds;

    [SerializeField]
    private AudioClip[] SoundEffects;

    public enum Music {walk,battle,boss}

    public enum SE { Damaged,EnemyDeath,Reflect,Trans}

    private AudioSource _audioSource;
	// Use this for initialization
	void Start () {
        _audioSource = GetComponent<AudioSource>();
	}
	
    public void OnPlay()
    {
        _audioSource.Play();
    }

    public void SoundEffect(SE effect)
    {
        _audioSource.PlayOneShot(SoundEffects[(int)effect]);
    }

    public void SoundChange(Music scene)
    {
        _audioSource.clip = BackSounds[(int)scene];
        _audioSource.Play();
    }
}
