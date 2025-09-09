using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM�� ����� �ҽ�(������ �� �ڵ� ����)")]
    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null)
        {
            bgmSource = gameObject.GetComponent<AudioSource>();
            if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
    }

    public void ChangeBackGroundMusic(AudioClip clip, float fadeTime = 0f)
    {
        if (clip == null) return;

        if (fadeTime <= 0f)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
            return;
        }

        // ���� ������ ���̵� (�ڷ�ƾ)
        StartCoroutine(FadeToClip(clip, fadeTime));
    }

    private System.Collections.IEnumerator FadeToClip(AudioClip nextClip, float t)
    {
        float startVol = bgmSource.volume;
        float time = 0f;

        // Fade out
        while (time < t)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, time / t);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = nextClip;
        bgmSource.Play();

        // Fade in
        time = 0f;
        while (time < t)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, startVol, time / t);
            yield return null;
        }
        bgmSource.volume = startVol;
    }
}
