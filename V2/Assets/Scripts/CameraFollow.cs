using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Obiekt, za którym podąża kamera
    public float FollowSpeed = 2f;
    public Camera cam;

    private Vector3 lastTargetPosition; // Ostatnia pozycja gracza
    private bool isPlayerDead = false; // Flaga, czy gracz jest martwy

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target (player) is not assigned in the CameraFollow script!");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Jeśli gracz jest martwy, śledź ostatnią pozycję
            if (isPlayerDead)
            {
                Vector3 targetPosition = new Vector3(lastTargetPosition.x, lastTargetPosition.y, cam.transform.position.z);
                cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, FollowSpeed * Time.deltaTime);
            }
            else
            {
                // Śledź aktualną pozycję gracza
                lastTargetPosition = target.position; // Zapisz ostatnią pozycję
                Vector3 targetPosition = new Vector3(target.position.x, target.position.y, cam.transform.position.z);
                cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, FollowSpeed * Time.deltaTime);
            }
        }
        else
        {
            Debug.LogWarning("Target is null. Camera is not following.");
        }
    }

    // Metoda do wywołania, gdy gracz umiera
    public void OnPlayerDeath()
    {
        isPlayerDead = true;
    }
}






// using UnityEngine;

// public class CameraFollow : MonoBehaviour
// {
//      public Transform target; // Obiekt, za którym podąża kamera
//      public float FollowSpeed = 2f;
//      public Camera cam;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
// // Śledzenie kamery za postacią
//         Vector3 targetPosition = new Vector3(target.position.x, target.position.y, cam.transform.position.z);
//         cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, FollowSpeed * Time.deltaTime);
                
//     }
// }
