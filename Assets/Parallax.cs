using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class Parallax : MonoBehaviour{
// public Transform target;

//     public float FollowSpeed = 2f;

//     public Camera cam;
//     public Transform subject;

//     Vector2 startPosition;
//     float startZ;

//     Vector2 travel => (Vector2)cam.transform.position - startPosition;

//     float distanceFromSubject => transform.position.z - subject.position.z;
//     float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0? cam.farClipPlane : cam.nearClipPlane));

//     float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;

//     public void Start(){
//         startPosition = transform.position;
//         startZ = transform.position.z;
//     }
//     public void Update(){
//         Vector2 newPos = startPosition + travel * parallaxFactor;
//         transform.position = new Vector3(newPos.x, newPos.y, startZ);
//         Vector3 targetPosition = new Vector3(target.position.x, target.position.y, cam.transform.position.z);
//         cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, FollowSpeed * Time.deltaTime);
//     }

// }

public class Parallax : MonoBehaviour {
    public Transform target; // Obiekt, za którym podąża kamera

    public float FollowSpeed = 2f; // Prędkość śledzenia kamery
    public Camera cam; // Kamera, która ma śledzić obiekt
    public Transform subject; // Obiekt używany do wyznaczenia paralaksy

    Vector2 startPosition;
    float startZ;

    Vector2 travel => (Vector2)cam.transform.position - startPosition;
    float distanceFromSubject => transform.position.z - subject.position.z;
    float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0 ? cam.farClipPlane : cam.nearClipPlane));

    float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;

    public void Start() {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    public void Update() {
        // Aktualizacja efektu paralaksy dla tła
        Vector2 newPos = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);

        // Śledzenie kamery za postacią
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, cam.transform.position.z);
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, FollowSpeed * Time.deltaTime);
    }
}
