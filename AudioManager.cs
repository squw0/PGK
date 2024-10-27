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

    }

    public void PlayCoinSound(){
        //Fetch the coin sound
        //PlayAudioClip(coinSound);
    }
}
//     private void PlayAudioClip(AudioClip coinSound)
//     {
//         throw new NotImplementedException();
//     }
// }
