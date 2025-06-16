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

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        canvasPergunta.SetActive(true);

        textoPergunta.text = pergunta.pergunta;

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

                int indiceResposta = i;
                botoesResposta[i].onClick.RemoveAllListeners();
                botoesResposta[i].onClick.AddListener(() => ResponderPergunta(indiceResposta));
            }
        }

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

            if (perguntaAtual.livroOrigem != null && perguntaAtual.livroOrigem is ILivroInterativo)
            {
                ((ILivroInterativo)perguntaAtual.livroOrigem).MarcarComoResolvido();
            }

            if (perguntasCorretas >= totalPerguntas)
            {
                Debug.Log("Jogo completado!");
                VoltarMenuVitoria();
                return;
            }
        }
        else
        {
            Debug.Log("Resposta incorreta!");
            vidasRestantes--;

            if (vidasRestantes <= 0)
            {
                Debug.Log("Game Over!");
                VoltarMenuDerrota();
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
        Time.timeScale = 1f;
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

    void VoltarMenuVitoria()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }
    void VoltarMenuDerrota()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public class PerguntaData
{
    public string pergunta;
    public string[] respostas;
    public int respostaCorreta;
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