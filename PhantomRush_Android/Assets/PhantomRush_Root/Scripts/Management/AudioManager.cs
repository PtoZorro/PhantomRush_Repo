using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source Reference")]
    [SerializeField] AudioSource musicSource; 
    [SerializeField] AudioSource sfxSource; 

    [Header("Audio Clip Arrays")]
    [SerializeField] AudioClip[] musicList; 
    [SerializeField] AudioClip[] sfxList; 

    [Header("Audio Stats")]
    public float musicVolume;
    public float sfxVolume;

    private void Awake()
    {
        //Singleton : Referencia a si mismo y que no se destruya entre escenas.
        if (instance == null)
        {
            instance = this; //El singleton se autoreferencia
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject); //Si hay otro audio manager, se destruye el "nuevo/duplicado"
        }
    }

    public void Start()
    {
        PlayMusic(0);
    }

    public void PlayMusic(int musicToPlay)
    {
        musicSource.clip = musicList[musicToPlay];
        musicSource.Play();
    }

    public void PlaySFX(int soundToPlay)
    {
        sfxSource.PlayOneShot(sfxList[soundToPlay]);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

}

