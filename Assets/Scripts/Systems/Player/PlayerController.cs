using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] protected bool inputRelatedToCamera = true;
    [SerializeField] protected new Camera camera;
    [Header("Interactables")]
    [SerializeField] protected float interactableRange = 4f;
    [SerializeField] protected Transform origin;
    [SerializeField] protected LayerMask interactableLayer;

    public UnityEvent<Vector2> OnMoveInput;
    public UnityEvent OnAttackInput;
    public UnityEvent OnSpecialAttackInput;
    public UnityEvent OnInteractInput;
    public UnityEvent OnEvadeInput;

    bool canPause = true;

    private void Start()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameSessionEnd += DisablePauseMenuUse;
        LevelManager.OnSceneLoading += DisablePauseMenuUse;
    }

    private void OnDisable()
    {
        GameManager.OnGameSessionEnd -= DisablePauseMenuUse;
        LevelManager.OnSceneLoading -= DisablePauseMenuUse;
    }

    private void OnMovement(InputValue inputValue)
    {
        Vector2 input = (inputValue != null) ? inputValue.Get<Vector2>() : Vector2.zero; 
        if (inputRelatedToCamera)
        {
            input = Quaternion.Euler(0f, 0f, -camera.transform.eulerAngles.y) * input;
        }
        OnMoveInput?.Invoke(input);
    }

    private void OnAttack()
    {
        OnAttackInput?.Invoke();
    }

    private void OnSpecialAttack()
    {
        OnSpecialAttackInput?.Invoke();
    }

    private void OnEvade()
    {
        OnEvadeInput?.Invoke();
    }

    private void OnInteract()
    {
        OnInteractInput?.Invoke();
        /*
        IInteractable interactable = null;
        if(TryGetInteractable(out interactable))
        {
            interactable.Interact();
        }
        */
    }

    private void DisablePauseMenuUse()
    {
        canPause = false;
    }

    private void OnPauseMenu(InputValue input)
    {
        if (Time.timeScale > 0 && canPause)
        {
            MenuManager.Instance.OpenMenu("pause_menu");
        }
        else if (canPause)
        {
            MenuManager.Instance.CloseMenu("pause_menu");
        }
    }

    private bool TryGetInteractable(out IInteractable interactable)
    {
        Debug.Log(interactableLayer.value);
        interactable = null;
        Collider[] interactables = Physics.OverlapSphere(origin.position, interactableRange, interactableLayer.value);
        if (interactables.Length == 0)
        {
            return false;
        }
        float distanceNearest = Mathf.Infinity;
        foreach (Collider collider in interactables)
        {
            float distance = Vector3.Distance(collider.gameObject.transform.position, origin.position);
            if ( distance < distanceNearest)
            {
                distanceNearest = distance;
                IInteractable aux = collider.gameObject.GetComponent<IInteractable>();
                if(aux != null ) { interactable = aux; }
            }
        }

        return interactable != null;
    }
}