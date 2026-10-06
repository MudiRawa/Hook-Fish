using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioClipInfo
    {
        public string name;
        public AudioClip clip;
    }

    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopingSfxSource;
    [SerializeField] private AudioSource loopingSfxSource2;  // Untuk Reel SFX

    [SerializeField] private AudioClipInfo[] bgmClips;
    [SerializeField] private AudioClipInfo[] sfxClips;
    [SerializeField] private AudioClipInfo[] loopingSfxClips;

    private int currentReelSfx = -1;  // Track which reel sfx is playing

    [SerializeField] private float bgmVolume = 0.7f;
    [SerializeField] private float sfxVolume = 0.8f;
    [SerializeField] private float loopingSfxVolume = 0.6f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup audio sources jika belum ada
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (loopingSfxSource == null)
        {
            loopingSfxSource = gameObject.AddComponent<AudioSource>();
        }
        if (loopingSfxSource2 == null)
        {
            loopingSfxSource2 = gameObject.AddComponent<AudioSource>();
        }

        // Set volume
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
        loopingSfxSource.volume = loopingSfxVolume;
        loopingSfxSource2.volume = 0.5f;  // Reel SFX volume
        bgmSource.loop = true;
        loopingSfxSource.loop = true;
        loopingSfxSource2.loop = true;
    }

    // ===== BGM Functions =====

    /// <summary>Putar BGM berdasarkan nama atau index</summary>
    public void PlayBGM(string clipName)
    {
        AudioClip clip = GetAudioClip(bgmClips, clipName);
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{clipName}' tidak ditemukan!");
        }
    }

    public void PlayBGM(int index)
    {
        if (index >= 0 && index < bgmClips.Length)
        {
            bgmSource.clip = bgmClips[index].clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM index {index} tidak valid!");
        }
    }

    /// <summary>Stop BGM</summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>Pause BGM</summary>
    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    /// <summary>Resume BGM</summary>
    public void ResumeBGM()
    {
        bgmSource.Play();
    }

    /// <summary>Fade out BGM</summary>
    public void FadeOutBGM(float duration = 1f)
    {
        StartCoroutine(FadeAudio(bgmSource, bgmVolume, 0f, duration));
    }

    /// <summary>Fade in BGM</summary>
    public void FadeInBGM(float duration = 1f)
    {
        bgmSource.volume = 0f;
        StartCoroutine(FadeAudio(bgmSource, 0f, bgmVolume, duration));
    }

    // ===== SFX Functions =====

    /// <summary>Putar SFX berdasarkan nama atau index</summary>
    public void PlaySFX(string clipName)
    {
        AudioClip clip = GetAudioClip(sfxClips, clipName);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning($"SFX '{clipName}' tidak ditemukan!");
        }
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[index].clip, sfxVolume);
        }
        else
        {
            Debug.LogWarning($"SFX index {index} tidak valid!");
        }
    }

    /// <summary>Stop semua SFX</summary>
    public void StopSFX()
    {
        sfxSource.Stop();
    }

    // ===== Volume Control =====

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;

    public void MuteBGM(bool mute)
    {
        bgmSource.mute = mute;
    }

    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
    }

    // ===== LOOPING SFX Functions =====

    /// <summary>Putar Looping SFX berdasarkan index</summary>
    public void PlayLoopingSFX(int index)
    {
        if (index >= 0 && index < loopingSfxClips.Length)
        {
            loopingSfxSource.clip = loopingSfxClips[index].clip;
            loopingSfxSource.loop = true;
            loopingSfxSource.Play();
            Debug.Log($"▶️ Looping SFX [{index}]: {loopingSfxClips[index].name}");
        }
        else
        {
            Debug.LogWarning($"Looping SFX index {index} tidak valid!");
        }
    }

    /// <summary>Putar Looping SFX berdasarkan nama</summary>
    public void PlayLoopingSFX(string clipName)
    {
        AudioClip clip = GetAudioClip(loopingSfxClips, clipName);
        if (clip != null)
        {
            loopingSfxSource.clip = clip;
            loopingSfxSource.loop = true;
            loopingSfxSource.Play();
            Debug.Log($"▶️ Looping SFX: {clipName}");
        }
        else
        {
            Debug.LogWarning($"Looping SFX '{clipName}' tidak ditemukan!");
        }
    }

    /// <summary>Stop Looping SFX</summary>
    public void StopLoopingSFX()
    {
        loopingSfxSource.Stop();
        Debug.Log("⏹️ Looping SFX stopped");
    }

    /// <summary>Fade out Looping SFX</summary>
    public void FadeOutLoopingSFX(float duration = 1f)
    {
        StartCoroutine(FadeAudio(loopingSfxSource, loopingSfxVolume, 0f, duration));
        Debug.Log($"🔉 Looping SFX fade out ({duration}s)");
    }

    /// <summary>Fade in Looping SFX</summary>
    public void FadeInLoopingSFX(float duration = 1f)
    {
        loopingSfxSource.volume = 0f;
        StartCoroutine(FadeAudio(loopingSfxSource, 0f, loopingSfxVolume, duration));
        Debug.Log($"🔊 Looping SFX fade in ({duration}s)");
    }

    /// <summary>Set Looping SFX Volume</summary>
    public void SetLoopingSFXVolume(float volume)
    {
        loopingSfxVolume = Mathf.Clamp01(volume);
        loopingSfxSource.volume = loopingSfxVolume;
        Debug.Log($"🔊 Looping SFX volume: {volume}");
    }

    /// <summary>Get Looping SFX Volume</summary>
    public float GetLoopingSFXVolume() => loopingSfxVolume;

    /// <summary>Mute/Unmute Looping SFX</summary>
    public void MuteLoopingSFX(bool mute)
    {
        loopingSfxSource.mute = mute;
    }

    // ===== Helper Functions =====

    private AudioClip GetAudioClip(AudioClipInfo[] clips, string name)
    {
        foreach (var clip in clips)
        {
            if (clip.name == name)
                return clip.clip;
        }
        return null;
    }

    private System.Collections.IEnumerator FadeAudio(AudioSource source, float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }
        source.volume = endVolume;
    }

    // ===== REEL LOOPING SFX Functions =====

    /// <summary>Putar Reel Looping SFX berdasarkan index (Pulling atau Losing)</summary>
    public void PlayReelLoopingSFX(int index)
    {
        if (loopingSfxClips.Length == 0 || index >= loopingSfxClips.Length)
        {
            Debug.LogWarning($"Reel Looping SFX index {index} out of range");
            return;
        }

        // Jika sudah main sfx yang sama, jangan restart
        if (currentReelSfx == index && loopingSfxSource2.isPlaying)
            return;

        currentReelSfx = index;
        loopingSfxSource2.Stop();
        loopingSfxSource2.clip = loopingSfxClips[index].clip;
        loopingSfxSource2.loop = true;
        loopingSfxSource2.volume = 0.5f;
        loopingSfxSource2.Play();
        Debug.Log($"▶️ Reel SFX [{index}]: {loopingSfxClips[index].name}");
    }

    /// <summary>Stop Reel Looping SFX</summary>
    public void StopReelLoopingSFX()
    {
        currentReelSfx = -1;
        loopingSfxSource2.Stop();
        Debug.Log("⏹️ Reel SFX stopped");
    }

    /// <summary>Fade out Reel Looping SFX</summary>
    public void FadeOutReelLoopingSFX(float duration = 0.5f)
    {
        StopCoroutine(FadeOutReelCoroutine(duration));
        StartCoroutine(FadeOutReelCoroutine(duration));
    }

    private System.Collections.IEnumerator FadeOutReelCoroutine(float duration)
    {
        float elapsed = 0f;
        float startVolume = loopingSfxSource2.volume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loopingSfxSource2.volume =
                Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        loopingSfxSource2.Stop();
        loopingSfxSource2.volume = 0.5f;
        Debug.Log($"🔉 Reel SFX fade out ({duration}s)");
    }
}