using UnityEngine;
using UnityEngine.SceneManagement; // Importar para gerenciar cenas

public class MenuManager : MonoBehaviour
{
    // Função para ser chamada pelo botão "Jogar"
    public void PlayGame()
    {
        // Carrega a próxima cena (a cena do seu jogo)
        // Substitua "NomeDaSuaCenaDoJogo" pelo nome real da sua cena de jogo
        SceneManager.LoadScene("SampleScene");
        // Ou, para carregar a próxima cena na ordem de build, use:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Função para ser chamada pelo botão "Sair"
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        // Para sair do aplicativo (funciona apenas em builds, não no editor)
        Application.Quit();

        // Se você quiser parar o editor no Play Mode (para testes), pode usar:
        // UnityEditor.EditorApplication.isPlaying = false;
    }
}