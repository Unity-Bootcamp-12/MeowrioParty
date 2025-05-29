using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{

    AudioSource _bgmSource;
    AudioSource _sfxSource;

    private Dictionary<BGMType, AudioClip> _bgmClips = new Dictionary<BGMType, AudioClip>();
    private Dictionary<SFXType, AudioClip> _sfxClips = new Dictionary<SFXType, AudioClip>();

    private bool _isInitialized = false;


    public void Init()
    {
        if (_isInitialized)
        {
            return;
        }

        GameObject obj = SoundManager.Instance.gameObject;
        GameObject bgmObj = new GameObject("BGM");
        SoundManager.Instance._bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmObj.transform.SetParent(obj.transform);

        GameObject sfxObj = new GameObject("SFX");
        SoundManager.Instance._sfxSource = sfxObj.AddComponent<AudioSource>();
        sfxObj.transform.SetParent(obj.transform);

        AudioClip[] BGMClips = Resources.LoadAll<AudioClip>("Sound/BGM");
        foreach (AudioClip clip in BGMClips)
        {
            try
            {
                BGMType type = (BGMType)Enum.Parse(typeof(BGMType), clip.name);
                SoundManager.Instance._bgmClips.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("bgm enum 필요 : " + clip.name);
            }
        }
        AudioClip[] SFXClips = Resources.LoadAll<AudioClip>("Sound/SFX");
        foreach (AudioClip clip in SFXClips)
        {
            try
            {
                SFXType type = (SFXType)Enum.Parse(typeof(SFXType), clip.name);
                SoundManager.Instance._sfxClips.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("bgm enum 필요 : " + clip.name);
            }
        }

        _isInitialized = true;

    }
    public void PlayBGM(BGMType type)
    {
        if (!_isInitialized)
        {
            Init();
        }

        _bgmSource.clip = _bgmClips[type];
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;       
        _bgmSource.Play();
        _bgmSource.volume = 0.5f;
    }
    public void PlaySFX(SFXType type)
    {
        if (!_isInitialized)
        {
            Init();
        }

        _sfxSource.PlayOneShot(_sfxClips[type]);
    }
}
public enum BGMType
{
    GameBGM,
}
public enum SFXType
{
    Coin, Star, RollingDice, StopDice, Click, Cancle, Clear
}
