using UnityEngine;
using System.Collections;

/// <summary>
/// 音楽処理、対応した音声の再生
/// </summary>

public class AudioManager : MonoBehaviour {
    #region[SerializeParameter]
    [SerializeField]
    private AudioClip[] BackSounds;//BGM

    [SerializeField]
    private AudioClip[] PlayerSoundEffects;//PlayerSE

    [SerializeField]
    private AudioClip[] EnemySoundEffects;//EnemySE
    #endregion

    #region[EnumList]
    public enum Music {walk,battle,boss}

    public enum SE { Damaged,Reflect,Trans}

    public enum EnemySE {Damaged,Guard, Throw,Tackle,Death}
    #endregion
    private AudioSource _audioSource;//オーディオ
	// Use this for initialization
	void Start () {
        if(Application.loadedLevelName == "Main")
        _audioSource = transform.parent.GetComponent<AudioSource>();
        else
            _audioSource = transform.GetComponent<AudioSource>();
        
	}
	
    //BGM再生
    public void OnPlay()
    {
        _audioSource.Play();
    }

    //PlayerSE再生
    public void SoundEffect(SE effect)
    {
        _audioSource.PlayOneShot(PlayerSoundEffects[(int)effect]);
    }

    //EnemySE再生
    public void EnemySoundEffect(EnemySE effect)
    {
        _audioSource.PlayOneShot(EnemySoundEffects[(int)effect]);
    }

    //BGM変更
    public void SoundChange(Music scene)
    {
        _audioSource.clip = BackSounds[(int)scene];
        _audioSource.Play();
    }
}
