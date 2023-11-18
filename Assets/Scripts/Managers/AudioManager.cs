using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);

            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    // For interrupting the current playing sound
    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        audioSource.clip = clip;

        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.Play();
    }

    // For playing without interruption: Two sounds will exist at the same time
    public void PlayOneShot(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.PlayOneShot(clip);
    }  
}
