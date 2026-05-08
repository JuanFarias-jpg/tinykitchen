using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource audioSource;

    [Header("Fade")]
    public float fadeDuration = 1.5f;

    [Header("Volumen")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeInMusic());
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }

    public void FadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        if (audioSource == null)
            yield break;

        float startVolume =
            audioSource.volume;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            audioSource.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    time / fadeDuration);

            yield return null;
        }

        audioSource.volume = 0f;

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeInMusic()
    {
        if (audioSource == null)
            yield break;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            audioSource.volume =
                Mathf.Lerp(
                    0f,
                    musicVolume,
                    time / fadeDuration);

            yield return null;
        }

        audioSource.volume = musicVolume;
    }
}