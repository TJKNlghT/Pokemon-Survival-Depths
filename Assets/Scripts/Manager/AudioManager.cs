using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip forestMusic;
    public AudioClip desertMusic;
    public AudioClip gameOverMusic;
    public AudioClip victoryMusic;
    public AudioClip bossMusic;

    [Header("SFX Clips")]
    public AudioClip selectSound;
    public AudioClip potionPickupSound;
    public AudioClip levelUpSound;
    public AudioClip charmanderFaintSound;
    public AudioClip squirtleFaintSound;
    public AudioClip pikachuFaintSound;
    public AudioClip fireballSound;
    public AudioClip poisonStingSound;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadVolumeSettings();
        PlayMainMenuMusic();
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    void ResetToDefaultVolumes()
    {
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("MusicVolume");
        PlayerPrefs.DeleteKey("SFXVolume");
    }
    
    public void PlayMainMenuMusic()
    {
        PlayMusicOnLoop(mainMenuMusic);
    }
    
    public void PlayForestMusic()
    {
        PlayMusicOnLoop(forestMusic);
    }
    
    public void PlayDesertMusic()
    {
        PlayMusicOnLoop(desertMusic);
    }

    public void PlayBossMusic()
    {
        PlayMusicOnLoop(bossMusic);
    }

    public void PlayGameOverMusic()
    {
        if (musicSource == null || gameOverMusic == null)
            return;

        // stop whatever was playing before (forest/desert/main menu)
        musicSource.Stop();

        musicSource.loop = false;         // <-- one shot, no loop
        musicSource.clip = gameOverMusic;
        musicSource.Play();
    }

    public void PlayVictoryMusic()
    {
        PlayMusicWithoutLoop(victoryMusic);
    }

    void PlayMusicWithoutLoop(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        // Avoid restarting same track if already playing
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.loop = false;
        musicSource.clip = clip;
        musicSource.Play();
    }

    void PlayMusicOnLoop(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        // Avoid restarting same track if already playing
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.loop = true;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
    }

    public void PlaySelectSound()
    {
        if (sfxSource != null && selectSound != null)
        {
            sfxSource.PlayOneShot(selectSound, 2.0f);
        }
    }

    public void PlayPotionPickupSound()
    {
        if (sfxSource != null && potionPickupSound != null)
        {
            sfxSource.PlayOneShot(potionPickupSound);
        }
    }

    public void PlayLevelUpSound()
    {
        if (sfxSource != null && levelUpSound != null)
        {
            sfxSource.PlayOneShot(levelUpSound);
        }
    }

    public void PlayCharmanderFaintSound()
    {
        if (sfxSource != null && charmanderFaintSound != null)
        {
            sfxSource.PlayOneShot(charmanderFaintSound);
        }
    }

    public void PlaySquirtleFaintSound()
    {
        if (sfxSource != null && squirtleFaintSound != null)
        {
            sfxSource.PlayOneShot(squirtleFaintSound);
        }
    }

    public void PlayPikachuFaintSound()
    {
        if (sfxSource != null && pikachuFaintSound != null)
        {
            sfxSource.PlayOneShot(pikachuFaintSound);
        }
    }

    public void PlayFireballSound()
    {
        if (sfxSource != null && fireballSound != null)
        {
            sfxSource.PlayOneShot(fireballSound);
        }
    }

    public void PlayPoisonStingSound()
    {
        if (sfxSource != null && poisonStingSound != null)
        {
            sfxSource.PlayOneShot(poisonStingSound);
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    
    void LoadVolumeSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.7f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }
}