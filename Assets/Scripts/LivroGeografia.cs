using UnityEngine;

public class LivroGeografia : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Player colidiu com o livro de Geografia!");

        }
    }
}
