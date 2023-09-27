using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Song
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
}

public class SongManager : MonoBehaviour
{
    public Song[] songs;

    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        AudioSource audioSource = GetComponent<AudioSource>();

        // Randomly select a song
        Song selectedSong = songs[Random.Range(0, songs.Length)];

        // Assign the selected song to the AudioSource
        audioSource.clip = selectedSong.clip;

        // Set the volume of the AudioSource based on the selected song's volume
        audioSource.volume = selectedSong.volume;

        // Set the loop property to true for continuous playback
        audioSource.loop = true;

        // Play the song
        audioSource.Play();
    }
}
