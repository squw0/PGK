// using UnityEngine;

// public class Damage : MonoBehaviour

//     public interface damage = 2;
// {
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         // Check if the collided object is the player
//         if (collision.collider.CompareTag("Player"))
//         {
//             PlayerController playerController = collision.collider.GetComponent<PlayerController>();
//             if (playerController != null)
//             {
//                 // Start damaging the player
//                 StartCoroutine(DamagePlayer(playerController));
//             }
//         }
//     }
// }
