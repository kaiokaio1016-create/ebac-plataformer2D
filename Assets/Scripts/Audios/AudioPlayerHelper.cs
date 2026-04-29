using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class AudioPlayerHelper : MonoBehaviour
{
    public KeyCode keycode = KeyCode.P;
    public AudioSource audioSource;
    public AudioClip somPulo;


    void Update()
    {
        if (Input.GetKeyDown(keycode))
        {
            Play();
        }
    }

    public void Play()
    {
        audioSource.Play();
    }

    void Pular()
    {
        // lógica do pulo aqui

        audioSource.PlayOneShot(somPulo);
    }
}