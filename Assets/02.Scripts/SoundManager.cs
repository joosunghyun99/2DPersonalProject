using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip beep;
    [SerializeField] private AudioClip hit;

    private AudioSource audioSource;


    private void Awake()
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

        audioSource = GetComponent<AudioSource>();
    }

    public void OnButtonClick() 
    {
        audioSource.PlayOneShot(beep);
    }

    public void OnPlayerHit() 
    {
        audioSource.PlayOneShot(hit);
    }

    public void OnCoinAcquire() 
    {
        audioSource.PlayOneShot(coin);
    }
}
