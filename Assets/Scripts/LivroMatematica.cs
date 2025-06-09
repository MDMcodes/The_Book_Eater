using UnityEngine;

public class LivroMatematica : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            Debug.Log("Player colidiu com o livro de Matemática!");

        }
    }
}
