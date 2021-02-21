using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundName
{
    None
}

[Serializable]
public struct SoundSet
{
    public SoundName Name;
    public AudioSource Source;
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private SoundSet[] _SoundSets;

    private Dictionary<SoundName, AudioSource> _SoundLib;

    private void Awake()
    {
        if (FindObjectsOfType<SoundManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);

            _SoundLib = new Dictionary<SoundName, AudioSource>();
            for (int i = 0; i < _SoundSets.Length; ++i)
            {
                _SoundLib.Add(_SoundSets[i].Name, _SoundSets[i].Source);
            }
        }
    }
    public void PlaySound(SoundName name)
    {
        _SoundLib[name].Play();
    }
}
