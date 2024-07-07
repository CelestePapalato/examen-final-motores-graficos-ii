using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    [SerializeField] protected Estado firstState;

    [Header("DEBUG")]
    [SerializeField]
    protected Estado currentState;
    protected Estado firstStateBuffer;
    protected Estado lastState;

    protected virtual void Awake()
    {
        if (!firstState)
        {
            firstState = GetComponent<Estado>();
        }

        if (firstState)
        {
            firstStateBuffer = firstState;
            CambiarEstado(firstState);
        }
        else
        {
            Debug.LogWarning("El State Machine " + name + "no posee ni encuentra un Estado al que llamar");
        }
    }

    protected virtual void Update()
    {
        if (currentState)
        {
            currentState.Actualizar();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (currentState)
        {
            currentState.ActualizarFixed();
        }
    }

    public virtual void CambiarEstado(Estado nuevoEstado)
    {
        currentState?.Salir();
        currentState = (nuevoEstado) ? nuevoEstado : firstState;
        currentState?.Entrar(this);
    }

    private void OnEnable()
    {
        if (currentState)
        {
            return;
        }
        firstState = firstStateBuffer;
        CambiarEstado(lastState);
    }

    private void OnDisable()
    {
        lastState = currentState;
        firstStateBuffer = firstState;
        firstState = null;
        currentState?.Salir();
    }

    private void OnDestroy()
    {
        firstState = null;
        currentState?.Salir();
    }
}

public abstract class Estado : MonoBehaviour
{
    protected StateMachine personaje;

    public virtual void Entrar(StateMachine personajeActual)
    {
        personaje = personajeActual;
    }
    public virtual void Salir() { }
    public virtual void Actualizar() { }
    public virtual void ActualizarFixed() { }
    public virtual void DañoRecibido() { }

    private void OnDisable()
    {
        if (!personaje) { return; }
        personaje.CambiarEstado(null);
    }

    private void OnDestroy()
    {
        if (!personaje) { return; }
        personaje.CambiarEstado(null);
    }
}