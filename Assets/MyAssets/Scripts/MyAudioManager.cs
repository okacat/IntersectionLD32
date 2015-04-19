using UnityEngine;
using System.Collections;

public class MyAudioManager : MonoBehaviour {

    // Sound clips
    public AudioClip[] explosionSounds;

    // Music
    public AudioClip music;
    
    // Source of the audio
    public AudioSource audioSource;

    // Audio parameters
    public float volume = 1.0f;

    // Singleton stuff
    private static MyAudioManager instance;
    public static MyAudioManager Instance
    {
        get
        {

            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<MyAudioManager>();
            }

            return instance;
        }
    }

	// Use this for initialization
	void Start () {

        // Get the reference to the audiosource component
        audioSource = this.gameObject.GetComponent<AudioSource>();

        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayExplode()
    {
        int i = Mathf.RoundToInt(Random.Range(0, explosionSounds.Length));
        audioSource.PlayOneShot(explosionSounds[i], volume/3.0f);
    }

    public IEnumerator FadeInMusic()
    {
        audioSource.loop = true;
        audioSource.clip = music;
        audioSource.Play();

        float duration = 1.5f;

        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0.0f, 1.0f, i);
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    public IEnumerator FadeOutMusic()
    {
        float duration = 1.5f;
        
        for (float i = 0; i < duration; i += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(1.0f, 0.0f, i);
            yield return new WaitForEndOfFrame();
        }
        

        audioSource.Stop();
        yield break;
    }


}
