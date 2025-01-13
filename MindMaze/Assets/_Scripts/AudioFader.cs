using UnityEngine;
using System.Collections;

public class AudioFader : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource[] audioSources; // Assign 3 AudioSources in the Inspector
    public float fadeDuration = 1.5f;  // Duration of fade in seconds
    [Range(0f, 1f)] public float volume = 0.2f; // Background music volume (slider in Inspector)

    private Coroutine currentCoroutine;

    void Start()
    {
        // Apply the initial volume to all audio sources
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.volume = volume;
            }
        }
    }

    public void PlayTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= audioSources.Length)
        {
            Debug.LogError("Invalid track index!");
            return;
        }

        // Find the currently playing track
        AudioSource currentPlaying = null;
        foreach (var source in audioSources)
        {
            if (source.isPlaying)
            {
                currentPlaying = source;
                break;
            }
        }

        // If the requested track is already playing, do nothing
        if (currentPlaying == audioSources[trackIndex])
            return;

        // Start fading between tracks
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(FadeTracks(currentPlaying, audioSources[trackIndex]));
    }

    private IEnumerator FadeTracks(AudioSource fadeOutSource, AudioSource fadeInSource)
    {
        float timer = 0f;

        if (fadeInSource != null)
        {
            fadeInSource.volume = 0f;
            fadeInSource.Play();
        }

        float fadeOutStartVolume = fadeOutSource != null ? fadeOutSource.volume : 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;

            if (fadeOutSource != null)
            {
                fadeOutSource.volume = Mathf.Lerp(fadeOutStartVolume, 0f, progress);
            }

            if (fadeInSource != null)
            {
                fadeInSource.volume = Mathf.Lerp(0f, volume, progress); // Respect the volume setting
            }

            yield return null;
        }

        if (fadeOutSource != null)
        {
            fadeOutSource.Stop();
        }

        if (fadeInSource != null)
        {
            fadeInSource.volume = volume; // Ensure volume stays at the set level
        }
    }

    void OnValidate()
    {
        // Update volume dynamically in the Editor
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.volume = volume;
            }
        }
    }
}
