using UnityEngine;

[System.Serializable]
public enum TipoMateria
{
    Ciencias,
    Matematica,
    Historia,
    Portugues,
    Geografia
}

public class LivroGenerico : MonoBehaviour, ILivroInterativo
{
    [Header("Configura��o do Livro")]
    public TipoMateria materia;
    public string nomeLivro;

    [Header("Configura��o da Pergunta")]
    public PerguntaData perguntaDoLivro;

    [Header("Estado do Livro")]
    private bool jaResolvido = false;

    [Header("Configura��o Visual")]
    public Color corOriginal = Color.white;
    public Color corResolvido = Color.green;
    private Renderer rendererLivro;

    void Start()
    {
        rendererLivro = GetComponent<Renderer>();
        if (rendererLivro != null)
        {
            corOriginal = rendererLivro.material.color;
        }

        // Se n�o configurou pergunta no inspector, usar pergunta padr�o
        if (string.IsNullOrEmpty(perguntaDoLivro.pergunta))
        {
            ConfigurarPerguntaPadrao();
        }

        // Definir cor baseada na mat�ria
        DefinirCorPorMateria();
    }

    void ConfigurarPerguntaPadrao()
    {
        switch (materia)
        {
            case TipoMateria.Ciencias:
                perguntaDoLivro.pergunta = "Qual � o planeta mais pr�ximo do Sol?";
                perguntaDoLivro.respostas = new string[] { "Merc�rio", "V�nus", "Terra", "Marte" };
                perguntaDoLivro.respostaCorreta = 0;
                break;

            case TipoMateria.Matematica:
                perguntaDoLivro.pergunta = "Quanto � a ra�z quadrada de 9?";
                perguntaDoLivro.respostas = new string[] { "2", "4", "3", "9" };
                perguntaDoLivro.respostaCorreta = 2;
                break;

            case TipoMateria.Historia:
                perguntaDoLivro.pergunta = "Quem foi o respons�vel pela chegada dos portugueses ao Brasil em 1500?";
                perguntaDoLivro.respostas = new string[] { "Crist�v�o Colombo", "Pedro �lvares Cabral", "Fern�o de Maga�h�es", "Pedro Pata" };
                perguntaDoLivro.respostaCorreta = 1;
                break;

            case TipoMateria.Portugues:
                perguntaDoLivro.pergunta = "Qual � o sujeito da frase: Os alunos chegaram cedo � escola";
                perguntaDoLivro.respostas = new string[] { "Chegaram", "Os alunos", "� escola", "Cedo" };
                perguntaDoLivro.respostaCorreta = 1;
                break;

            case TipoMateria.Geografia:
                perguntaDoLivro.pergunta = "Qual � o maior pa�s do mundo em extens�o territorial?";
                perguntaDoLivro.respostas = new string[] { "Canad�", "China", "R�ssia", "Estados Unidos" };
                perguntaDoLivro.respostaCorreta = 2;
                break;
        }
    }

    void DefinirCorPorMateria()
    {
        switch (materia)
        {
            case TipoMateria.Ciencias:
                corResolvido = Color.green;
                break;
            case TipoMateria.Matematica:
                corResolvido = Color.blue;
                break;
            case TipoMateria.Historia:
                corResolvido = Color.red;
                break;
            case TipoMateria.Portugues:
                corResolvido = Color.yellow;
                break;
            case TipoMateria.Geografia:
                corResolvido = Color.cyan;
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && !jaResolvido)
        {
            string nomeExibir = string.IsNullOrEmpty(nomeLivro) ? materia.ToString() : nomeLivro;
            Debug.Log($"Player colidiu com o livro de {nomeExibir}!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AbrirPergunta(perguntaDoLivro, this);
            }
            else
            {
                Debug.LogError("GameManager n�o encontrado!");
            }
        }
        else if (jaResolvido)
        {
            Debug.Log("Este livro j� foi resolvido!");
        }
    }

    public void MarcarComoResolvido()
    {
        jaResolvido = true;
        string nomeExibir = string.IsNullOrEmpty(nomeLivro) ? materia.ToString() : nomeLivro;
        Debug.Log($"{nomeExibir} foi marcado como resolvido!");

        if (rendererLivro != null)
        {
            rendererLivro.material.color = corResolvido;
        }
    }

    public bool EstaResolvido()
    {
        return jaResolvido;
    }

    [ContextMenu("Resetar Livro")]
    public void ResetarLivro()
    {
        jaResolvido = false;
        if (rendererLivro != null)
        {
            rendererLivro.material.color = corOriginal;
        }
        string nomeExibir = string.IsNullOrEmpty(nomeLivro) ? materia.ToString() : nomeLivro;
        Debug.Log($"{nomeExibir} foi resetado!");
    }
}