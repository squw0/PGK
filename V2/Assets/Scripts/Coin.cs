using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour, ICollectible{
    public static event Action OnCoinCollected;
    public AudioClip coinSound;

    // public override void Collect(){
    //      Debug.Log("You collected a coin");

    public void Collect(){
        Debug.Log("Coin Collected");
        Destroy(gameObject);
        OnCoinCollected?.Invoke();
    }

    private void OnTriggerEnter(Collider other){
        AudioSource.PlayClipAtPoint(coinSound, transform.position,1f);
    }
}
