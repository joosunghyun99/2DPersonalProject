using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip beep;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip Jump;

    [SerializeField] private AudioSource effectSource;
    [SerializeField] private AudioSource bgmSource;


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

    private void Start()
    {
        BGMOnOff(0);
    }

    public void OnButtonClick() 
    {
        effectSource.PlayOneShot(beep);
    }

    public void OnPlayerHit() 
    {
        effectSource.PlayOneShot(hit);
    }

    public void OnCoinAcquire() 
    {
        effectSource.PlayOneShot(coin);
    }

    public void OnPlayerJump() 
    {
        effectSource.PlayOneShot(Jump);
    }

    public void BGMOnOff(int mode) 
    {
        if (mode == 0)
        {
            bgmSource.Stop();
        }
        else if (mode == 1)
        {
            bgmSource.Play();
        }
    }

    public void SetVolume(int num, float value) 
    {
        if (num == 0)
        {
            effectSource.volume = value;
        }
        else if (num == 1) 
        {
            bgmSource.volume = value;
        }  
    }
}
