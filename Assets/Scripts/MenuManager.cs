using UnityEngine;
using UnityEngine.SceneManagement; // Importar para gerenciar cenas

public class MenuManager : MonoBehaviour
{
    // Fun��o para ser chamada pelo bot�o "Jogar"
    public void PlayGame()
    {
        // Carrega a pr�xima cena (a cena do seu jogo)
        // Substitua "NomeDaSuaCenaDoJogo" pelo nome real da sua cena de jogo
        SceneManager.LoadScene("SampleScene");
        // Ou, para carregar a pr�xima cena na ordem de build, use:
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Fun��o para ser chamada pelo bot�o "Sair"
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        // Para sair do aplicativo (funciona apenas em builds, n�o no editor)
        Application.Quit();

        // Se voc� quiser parar o editor no Play Mode (para testes), pode usar:
        // UnityEditor.EditorApplication.isPlaying = false;
    }
}