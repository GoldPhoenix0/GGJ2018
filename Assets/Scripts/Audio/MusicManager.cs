using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    private static MusicManager _instance;
    public static MusicManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instHolder = new GameObject("MusicManager");
                _instance = instHolder.AddComponent<MusicManager>();
            }

            return _instance;
        }
    }

    public AudioSource[] AudioSources { get; protected set; }

    public int NumAudioSources = 4;
    protected int lastAudioSource = 0;

    public AudioClip MainMusicClip;
    public AudioClip AltMusicClip;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(_instance.gameObject);

        AudioSources = new AudioSource[NumAudioSources];

        Transform thisTransform = transform;
        for (int i = 0; i < AudioSources.Length; i++)
        {
            GameObject asHolder = new GameObject("Source_" + i);
            asHolder.transform.parent = thisTransform;
            AudioSources[i] = asHolder.AddComponent<AudioSource>();
            AudioSources[i].loop = true;
        }

        AudioClip[] musicOrder = null;

        if (Random.value > 0.5f)
            musicOrder = new AudioClip[] { MainMusicClip, AltMusicClip };
        else
            musicOrder = new AudioClip[] { AltMusicClip, MainMusicClip };

        LoopMultipleTracks(musicOrder);

        //if(MainMusicClip != null)
        //{
        //    PlayMusicClip(MainMusicClip);
        //    AudioSource source = FindClipPlaying(MainMusicClip);
        //    FadeAudioSource(source, startVolume: 0f);
        //}

        //if (OverlayMusicClip != null)
        //{
        //    PlayMusicClip(OverlayMusicClip);
        //    AudioSource source = FindClipPlaying(OverlayMusicClip);
        //    source.volume = 0;
        //}
    }

    public AudioSource FindClipPlaying(AudioClip checkClip)
    {
        if (checkClip == null)
            return null;

        for (int i = 0; i < AudioSources.Length; i++)
        {
            if (AudioSources[i].clip == checkClip)
                return AudioSources[i];
        }

        return null;
    }

    public Coroutine LoopMultipleTracks(AudioClip[] audioClips, int numLoops = -1)
    {
        if (audioClips == null || audioClips.Length <= 0)
            return null;

        return _instance.StartCoroutine(LoopMultipleTracksAsync(audioClips, numLoops));
    }

    private IEnumerator LoopMultipleTracksAsync(AudioClip[] audioClips, int numLoops)
    {
        bool loopEndlessly = numLoops <= 0;
        int currentLoops = 0;
        AudioSource source = null;

        while(loopEndlessly || currentLoops > numLoops)
        {
            for (int i = 0; i < audioClips.Length; i++)
            {
                AudioClip clip = audioClips[i];

                if(source == null)
                {
                    PlayMusicClip(clip);
                    source = FindClipPlaying(clip);
                    source.loop = false;
                }
                else
                {
                    source.clip = clip;
                    source.Play();
                }

                while (source != null && source.isPlaying)
                    yield return null;
            }

            currentLoops++;

            if (source == null)
                break;
        }

        if(source != null)
            source.loop = true;
    }

    public Coroutine PlayMusicClip(AudioClip newAudioClip)
    {
        if (newAudioClip == null)
            return null;

        return StartCoroutine(PlayMusicClipAsync(newAudioClip));
    }

    protected IEnumerator PlayMusicClipAsync(AudioClip newAudioClip)
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
    }

    public static Coroutine FadeAudioSource(AudioSource audioSource, float fadeDuration = 1f, float startVolume = -1, float targetVolume = 1f)
    {
        if (audioSource == null)
            return null;

        if (startVolume < 0)
            startVolume = audioSource.volume;


        return _instance.StartCoroutine(FadeAudioSourceAsync(audioSource, fadeDuration, startVolume, targetVolume));
    }

    private static IEnumerator FadeAudioSourceAsync(AudioSource audioSource, float fadeDuration, float startVolume, float targetVolume)
    {
        float currentDuration = 0f;

        audioSource.volume = startVolume;

        while(currentDuration < fadeDuration)
        {
            yield return null;
            currentDuration += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentDuration / fadeDuration);
        }

        audioSource.volume = targetVolume;
    }
}
