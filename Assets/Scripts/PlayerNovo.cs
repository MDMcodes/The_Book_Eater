using UnityEngine;

public class PlayerNovo : MonoBehaviour
{
    // Refer�ncia ao CharacterController da Unity
    private CharacterController controller;

    // Velocidade de movimento
    public float speed = 5f;
    public float runSpeed = 8f; // Velocidade de corrida

    // Refer�ncia ao Animator (para anima��es)
    private Animator anim;

    // Refer�ncia � c�mera (para rota��o com o mouse)
    public Transform cameraTransform;

    // Sensibilidade do mouse
    public float mouseSensitivity = 2f;

    // Acumulador para rota��o vertical (c�mera)
    private float xRotation = 0f;

    // Estados de movimento
    private bool isRunning = false;
    private bool isCrouching = false;
    private int movementState = 0; // 0 - parado, 1 - andando, 2 - correndo, 3 - agachado, 4 - andando agachado

    void Start()
    {
        // Pegando os componentes necess�rios na cena
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Verificando se o componente Animator est� presente
        if (anim == null)
        {
            Debug.LogError("Animator n�o encontrado! Verifique se o objeto tem o componente Animator.");
        }
        else
        {
            // Verificando se o par�metro 'transition' existe no Animator
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
                Debug.LogError("Par�metro 'transition' n�o encontrado no Animator! Verifique se o par�metro foi criado corretamente.");
            }
            else
            {
                Debug.Log("Animator e par�metro 'transition' encontrados com sucesso.");
            }
        }

        // Se a c�mera n�o foi atribu�da, tenta encontrar a c�mera principal
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                Debug.Log("Camera atribu�da automaticamente: " + cameraTransform.name);
            }
            else
            {
                Debug.LogError("Nenhuma c�mera encontrada! Por favor, atribua uma c�mera no inspector.");
            }
        }

        // Bloqueia e esconde o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Verificar estado de corrida ANTES da movimenta��o
        // Usa GetKey em vez de GetKeyDown/GetKeyUp para detectar se a tecla est� sendo mantida pressionada
        bool wasRunning = isRunning;
        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (wasRunning != isRunning)
        {
            Debug.Log("Estado de corrida alterado: " + (isRunning ? "CORRENDO" : "N�O CORRENDO"));
        }

        // Verificar estado de agachamento
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            Debug.Log("Estado de agachamento alterado: " + (isCrouching ? "AGACHADO" : "EM P�"));
            UpdateMovementState();
        }

        Move();

        // Verifica se a c�mera foi atribu�da antes de tentar us�-la
        if (cameraTransform != null)
        {
            // ----------- ROTACIONA O PLAYER COM O MOUSE (C�MERA) ----------------
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Roda o player horizontalmente
            transform.Rotate(Vector3.up * mouseX);

            // Roda a c�mera verticalmente (limitada para n�o girar demais)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    public void Move()
    {
        // ----------- MOVIMENTA��O ----------------
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

        // Movimenta��o e atualiza��o do estado de anima��o
        bool isMoving = (move.magnitude > 0.1f && controller.isGrounded);

        if (isMoving)
        {
            controller.Move(currentSpeed * Time.deltaTime * move); // Aplica movimenta��o
            Debug.Log("Em movimento: " + (isRunning ? "correndo" : (isCrouching ? "agachado" : "andando")));
        }
        else
        {
            Debug.Log("Parado: " + (isCrouching ? "agachado" : "em p�"));
        }

        UpdateMovementState(isMoving);
    }

    private void UpdateMovementState(bool isMoving = true)
    {
        int previousState = movementState;

        if (!isMoving)
        {
            // Se n�o est� se movendo, mas est� agachado
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
            Debug.Log("Alterando estado de anima��o: " + previousState + " -> " + movementState);
            anim.SetInteger("transition", movementState);
        }
    }
}