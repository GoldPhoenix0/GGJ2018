using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private static SFXManager _instance;
    public static SFXManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instHolder = new GameObject("SFXManager");
                _instance = instHolder.AddComponent<SFXManager>();
                DontDestroyOnLoad(instHolder);
            }

            return _instance;
        }
    }

    public AudioSource[] AudioSources { get; protected set; }

    public int NumAudioSources = 32;
    protected int lastAudioSource = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        AudioSources = new AudioSource[NumAudioSources];

        Transform thisTransform = transform;
        for (int i = 0; i < AudioSources.Length; i++)
        {
            GameObject asHolder = new GameObject("Source_" + i);
            asHolder.transform.parent = thisTransform;
            AudioSources[i] = asHolder.AddComponent<AudioSource>();
        }
    }

    public Coroutine PlayAudioClip(AudioClip newAudioClip)
    {
        if (newAudioClip == null)
            return null;

        return StartCoroutine(PlayAudioClipAsync(newAudioClip));
    }

    protected IEnumerator PlayAudioClipAsync(AudioClip newAudioClip)
    {
        AudioSource foundSource = null;

        for (int i = 0; i < AudioSources.Length; i++)
        {
            int checkIndex = (lastAudioSource + i) % AudioSources.Length;

            foundSource = AudioSources[checkIndex];

            if (!foundSource.isPlaying)
                break;
        }

        lastAudioSource++;

        if (foundSource == null)
            yield break;

        foundSource.clip = newAudioClip;
        foundSource.Play();

        while (foundSource.isPlaying)
            yield return null;

        foundSource.Stop();
    }
}
