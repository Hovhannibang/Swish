using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{

    private Queue<GameObject> audioSourcePool = new Queue<GameObject>();
    private GameObject audioSourceObj;
    public AudioClip[] explosions;
    public AudioClip[] bounces;
    public AudioSource backgroundSource;
    private AudioSource gameOverSource;
    public GameObject audioSourcePrefab;
    private float musicVolume;
    private float sfxVolume;
    public Slider musicSlider;
    public Slider sfxSlider;


    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < 40; i++)
        {
            audioSourcePool.Enqueue(Instantiate(audioSourcePrefab));
        }

        gameOverSource = GetComponent<AudioSource>();
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.2f);
        }
        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("sfxVolume", 0.3f);
        }
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        backgroundSource.volume = musicVolume;
    }


    public void playExplosion()
    {
        playSound(explosions[Random.Range(0, explosions.Length)], sfxVolume);
        backgroundSource.Stop();
    }

    public void playBounce()
    {
        playSound(bounces[Random.Range(0, bounces.Length)], sfxVolume);
    }

    public void playSound(AudioClip sound, float volume)
    {
        audioSourceObj = audioSourcePool.Dequeue();
        audioSourcePool.Enqueue(audioSourceObj);
        audioSourceObj.GetComponent<AudioSource>().PlayOneShot(sound, volume);
    }

    public void setMusicVol()
    {
        musicVolume = musicSlider.value;
        backgroundSource.volume = musicVolume;
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }
    public void setSFXVol()
    {
        sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }
}
