using UnityEngine;

public class PlayerNovo : MonoBehaviour
{
    // Referência ao CharacterController da Unity
    private CharacterController controller;

    // Velocidade de movimento
    public float speed = 5f;
    public float runSpeed = 8f; // Velocidade de corrida

    // Referência ao Animator (para animações)
    private Animator anim;

    // Referência à câmera (para rotação com o mouse)
    public Transform cameraTransform;

    // Sensibilidade do mouse
    public float mouseSensitivity = 2f;

    // Acumulador para rotação vertical (câmera)
    private float xRotation = 0f;

    // Estados de movimento
    private bool isRunning = false;
    private bool isCrouching = false;
    private int movementState = 0; // 0 - parado, 1 - andando, 2 - correndo, 3 - agachado, 4 - andando agachado

    void Start()
    {
        // Pegando os componentes necessários na cena
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Verificando se o componente Animator está presente
        if (anim == null)
        {
            Debug.LogError("Animator não encontrado! Verifique se o objeto tem o componente Animator.");
        }
        else
        {
            // Verificando se o parâmetro 'transition' existe no Animator
            AnimatorControllerParameter[] parameters = anim.parameters;
            bool hasTransitionParam = false;
            foreach (AnimatorControllerParameter param in parameters)
            {
                if (param.name == "transition" && param.type == AnimatorControllerParameterType.Int)
                {
                    hasTransitionParam = true;
                    break;
                }
            }

            if (!hasTransitionParam)
            {
                Debug.LogError("Parâmetro 'transition' não encontrado no Animator! Verifique se o parâmetro foi criado corretamente.");
            }
            else
            {
                Debug.Log("Animator e parâmetro 'transition' encontrados com sucesso.");
            }
        }

        // Se a câmera não foi atribuída, tenta encontrar a câmera principal
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                Debug.Log("Camera atribuída automaticamente: " + cameraTransform.name);
            }
            else
            {
                Debug.LogError("Nenhuma câmera encontrada! Por favor, atribua uma câmera no inspector.");
            }
        }

        // Bloqueia e esconde o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Verificar estado de corrida ANTES da movimentação
        // Usa GetKey em vez de GetKeyDown/GetKeyUp para detectar se a tecla está sendo mantida pressionada
        bool wasRunning = isRunning;
        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (wasRunning != isRunning)
        {
            Debug.Log("Estado de corrida alterado: " + (isRunning ? "CORRENDO" : "NÃO CORRENDO"));
        }

        // Verificar estado de agachamento
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            Debug.Log("Estado de agachamento alterado: " + (isCrouching ? "AGACHADO" : "EM PÉ"));
            UpdateMovementState();
        }

        Move();

        // Verifica se a câmera foi atribuída antes de tentar usá-la
        if (cameraTransform != null)
        {
            // ----------- ROTACIONA O PLAYER COM O MOUSE (CÂMERA) ----------------
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Roda o player horizontalmente
            transform.Rotate(Vector3.up * mouseX);

            // Roda a câmera verticalmente (limitada para não girar demais)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void Move()
    {
        // ----------- MOVIMENTAÇÃO ----------------
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Aplicar a velocidade apropriada dependendo do estado
        float currentSpeed = speed;
        if (isRunning && !isCrouching)
        {
            currentSpeed = runSpeed;
            Debug.Log("Correndo: velocidade = " + currentSpeed);
        }
        else if (isCrouching)
        {
            currentSpeed = speed * 0.5f; // Velocidade reduzida quando agachado
            Debug.Log("Agachado: velocidade = " + currentSpeed);
        }
        else
        {
            Debug.Log("Andando: velocidade = " + currentSpeed);
        }

        // Movimentação e atualização do estado de animação
        bool isMoving = (move.magnitude > 0.1f && controller.isGrounded);

        if (isMoving)
        {
            controller.Move(currentSpeed * Time.deltaTime * move); // Aplica movimentação
            Debug.Log("Em movimento: " + (isRunning ? "correndo" : (isCrouching ? "agachado" : "andando")));
        }
        else
        {
            Debug.Log("Parado: " + (isCrouching ? "agachado" : "em pé"));
        }

        UpdateMovementState(isMoving);
    }

    private void UpdateMovementState(bool isMoving = true)
    {
        int previousState = movementState;

        if (!isMoving)
        {
            // Se não está se movendo, mas está agachado
            if (isCrouching)
            {
                movementState = 3; // agachado parado
            }
            else
            {
                movementState = 0; // parado (idle)
            }
        }
        else if (isCrouching)
        {
            movementState = 4; // andando agachado
        }
        else if (isRunning)
        {
            movementState = 2; // correndo
        }
        else
        {
            movementState = 1; // andando
        }

        // Apenas atualiza o animator se o estado mudou
        if (previousState != movementState)
        {
            Debug.Log("Alterando estado de animação: " + previousState + " -> " + movementState);
            anim.SetInteger("transition", movementState);
        }
    }
}