using UnityEngine;

public class LivroHistoria : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Player colidiu com o livro de História!");

        }
    }
}
