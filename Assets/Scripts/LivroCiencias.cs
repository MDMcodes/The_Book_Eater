using UnityEngine;

public class LivroCiencias : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Player colidiu com o livro de Ciências!");

        }
    }
}
