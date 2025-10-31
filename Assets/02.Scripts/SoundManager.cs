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
    }

    public void OnButtonClick() 
    {
        audioSource.clip = beep;
        audioSource.Play();
    }

    public void OnPlayerHit() 
    {
        audioSource.clip = hit;
        audioSource.Play();
    }

    public void OnCoinAcquire() 
    {
        audioSource.clip = coin;
        audioSource.Play();
    }
}
