using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    private AudioSource audio;
    private static MusicScript instance = null;

    public bool playing = true;
    public bool isGame = false;

    /// <summary>
    /// Call when application begins.
    /// Keeps gameobject from being destroyed.
    /// </summary>
    private void Awake() {

        if (instance == null) {
            instance = this;

            DontDestroyOnLoad(transform.gameObject);
            audio = GetComponent<AudioSource>();
        }
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// Play music.
    /// </summary>
   public void PlayMusic() {
        if (!audio.isPlaying)
            audio.Play();
    }

    /// <summary>
    /// Pause music.
    /// </summary>
    public void StopMusic() {
        audio.Stop();
    }
}
