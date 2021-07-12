using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossDirector : MonoBehaviour
{
    [SerializeField] private PlayableDirector _Director;

    [Header("Audio Property")]
    [SerializeField] private AudioClip _BossThema;
    [SerializeField] private AudioSource _AudioSource;

    [ContextMenu("Play Timeline")]
    private void PlayTimeLine()
    {
        _Director.Play();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PlayTimeLine();
    }
    public void SE_Action()
    {
        _AudioSource.clip = _BossThema;
    }
}
