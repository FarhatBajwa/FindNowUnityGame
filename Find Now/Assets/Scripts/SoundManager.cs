using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {  get; set; }
    public AudioSource MainPlayingSource;
    public AudioClip Spot;
    public AudioClip Win;
    public AudioClip Lose;
    public AudioSource BGmusicSource;
    public AudioClip Bgmusic;


    public bool Active = true;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
