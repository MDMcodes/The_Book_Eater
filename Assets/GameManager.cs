using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Sistema de Vidas")]
    public int vidasRestantes = 3;
    public int perguntasCorretas = 0;
    public int totalPerguntas = 5;

    [Header("UI References")]
    public GameObject canvasPergunta;
    public UnityEngine.UI.Text textoPergunta;
    public UnityEngine.UI.Button[] botoesResposta;
    public UnityEngine.UI.Text textoVidas;
    public UnityEngine.UI.Text textoProgresso;

    [Header("Sistema de Perguntas")]
    private PerguntaAtual perguntaAtual;
    private bool jogadorRespondendo = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AtualizarUI();
        canvasPergunta.SetActive(false);
    }

    public void AbrirPergunta(PerguntaData pergunta, MonoBehaviour livroScript)
    {
        if (jogadorRespondendo) return;

        // Verificações de segurança
        if (canvasPergunta == null)
        {
            Debug.LogError("Canvas da pergunta não está conectado no GameManager!");
            return;
        }

        if (textoPergunta == null)
        {
            Debug.LogError("Texto da pergunta não está conectado no GameManager!");
            return;
        }

        if (botoesResposta == null || botoesResposta.Length == 0)
        {
            Debug.LogError("Botões de resposta não estão conectados no GameManager!");
            return;
        }

        perguntaAtual = new PerguntaAtual(pergunta, livroScript);
        jogadorRespondendo = true;

        // Pausar o jogo e liberar cursor
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ativar canvas
        canvasPergunta.SetActive(true);

        // Configurar pergunta
        textoPergunta.text = pergunta.pergunta;

        // Configurar botões de resposta
        for (int i = 0; i < botoesResposta.Length && i < pergunta.respostas.Length; i++)
        {
            if (botoesResposta[i] != null)
            {
                botoesResposta[i].gameObject.SetActive(true);

                UnityEngine.UI.Text textoButton = botoesResposta[i].GetComponentInChildren<UnityEngine.UI.Text>();
                if (textoButton != null)
                {
                    textoButton.text = pergunta.respostas[i];
                }

                // Remover listeners antigos e adicionar novo
                int indiceResposta = i;
                botoesResposta[i].onClick.RemoveAllListeners();
                botoesResposta[i].onClick.AddListener(() => ResponderPergunta(indiceResposta));
            }
        }

        // Desativar botões extras se houver menos de 4 respostas
        for (int i = pergunta.respostas.Length; i < botoesResposta.Length; i++)
        {
            if (botoesResposta[i] != null)
            {
                botoesResposta[i].gameObject.SetActive(false);
            }
        }
    }

    public void ResponderPergunta(int indiceResposta)
    {
        if (!jogadorRespondendo) return;

        bool acertou = indiceResposta == perguntaAtual.data.respostaCorreta;

        if (acertou)
        {
            Debug.Log("Resposta correta!");
            perguntasCorretas++;

            // Marcar o livro como resolvido usando interface
            if (perguntaAtual.livroOrigem != null && perguntaAtual.livroOrigem is ILivroInterativo)
            {
                ((ILivroInterativo)perguntaAtual.livroOrigem).MarcarComoResolvido();
            }

            // Verificar se completou o jogo
            if (perguntasCorretas >= totalPerguntas)
            {
                Debug.Log("Jogo completado!");
                VoltarMenuInicial();
                return;
            }
        }
        else
        {
            Debug.Log("Resposta incorreta!");
            vidasRestantes--;

            // Verificar se perdeu todas as vidas
            if (vidasRestantes <= 0)
            {
                Debug.Log("Game Over!");
                VoltarMenuInicial();
                return;
            }
        }

        FecharPergunta();
        AtualizarUI();
    }

    void FecharPergunta()
    {
        canvasPergunta.SetActive(false);
        jogadorRespondendo = false;

        // Despausar o jogo e restaurar cursor (se necessário)
        Time.timeScale = 1f;

        // Se seu jogo usa cursor travado, descomente as linhas abaixo:
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void AtualizarUI()
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidasRestantes;
        else
            Debug.LogWarning("TextoVidas não está conectado no GameManager!");

        if (textoProgresso != null)
            textoProgresso.text = "Progresso: " + perguntasCorretas + "/" + totalPerguntas;
        else
            Debug.LogWarning("TextoProgresso não está conectado no GameManager!");
    }

    void VoltarMenuInicial()
    {
        Time.timeScale = 1f; // Garantir que o tempo volte ao normal
        SceneManager.LoadScene(0); // Assumindo que o menu inicial é a primeira cena
    }
}

[System.Serializable]
public class PerguntaData
{
    public string pergunta;
    public string[] respostas;
    public int respostaCorreta; // Índice da resposta correta (0-3)
}

public class PerguntaAtual
{
    public PerguntaData data;
    public MonoBehaviour livroOrigem;

    public PerguntaAtual(PerguntaData perguntaData, MonoBehaviour livro)
    {
        data = perguntaData;
        livroOrigem = livro;
    }
}