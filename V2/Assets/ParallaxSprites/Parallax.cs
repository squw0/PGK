using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
   
    public float FollowSpeed = 2f; // Prędkość śledzenia kamery
    public Camera cam; // Kamera, która ma śledzić obiekt
    public Transform subject; // Obiekt używany do wyznaczenia paralaksy

    Vector2 startPosition;
    float startZ;

    Vector2 travel => (Vector2)cam.transform.position - startPosition;
    float distanceFromSubject => subject == null ? 0 : transform.position.z - subject.position.z;
    float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));
    float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;

    private bool isGameOver = false; // Flaga oznaczająca zakończenie gry

    public void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    public void LateUpdate()
    {
        // Zatrzymanie paralaksy, jeśli gra się zakończyła lub obiekt nie istnieje
        if (isGameOver || subject == null) return;

        // Aktualizacja efektu paralaksy dla tła
        Vector2 newPos = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);

        
    }

    // Wywoływana, gdy gra się kończy (np. postać gracza ginie)
    public void OnGameOver()
    {
        isGameOver = true;
    }
}
