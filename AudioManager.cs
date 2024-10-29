using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour{
    public static AudioManager Instance;
    AudioSource audioSource;

    [SerializeField] AudioClip coinSound, pickupSound;

    private void OnEnable(){
        Coin.OnCoinCollected += PlayCoinSound;
    }

    private void OnDisable(){
        Coin.OnCoinCollected -= PlayCoinSound;
    }

    private void Awake(){
        if(Instance !=null && Instance != this){
            Debug.Log("Destroying duplicate AudioManager Instance");
            Destroy(this);
        }
        else{
            Debug.Log("Setting AudioManager Instance");
            Instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCoinSound(){
        PlayAudioClip(coinSound);
    }

    public void PlayAudioClip(AudioClip clip){
        audioSource.PlayOneShot(clip);
    }
    
    private void PlayPickupSound(){
        PlayAudioClip(pickupSound);
    }
}