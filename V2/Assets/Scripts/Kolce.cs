// using UnityEngine;

// public class Trap : MonoBehaviour
// {

//     public float bounceForce = 10f;
//     public int damage = 1;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.gameObject.CompareTag("Player"))
//         {
//             HandlePlayerBounce(collision.gameObject);
//         }
//     }

//     private void HandlePlayerBounce(GameObject player)
//     {
//             Debug.Log("fdgfdgdf");
//             Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

//         if (rb)
//         {
//             Debug.Log("fdgfdgdf");
//             //reset player velocity
//             rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

//             rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
//         }
//     }
// }