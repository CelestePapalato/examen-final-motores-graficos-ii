using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Esperar : Estado
{
    [SerializeField] float tiempo;
    [Tooltip("Opcional. Si Personaje no recibe un Estado no nulo, simplemente activa su primer estado.")]
    [SerializeField] Estado siguienteEstado;

    Movement movement;

    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);
        StartCoroutine(EsperarYCambiarEstado());
    }

    IEnumerator EsperarYCambiarEstado()
    {
        if(movement) { movement.Direction = Vector2.zero; }
        yield return new WaitForSeconds(tiempo);
        if (personaje)
        {
            personaje.CambiarEstado(siguienteEstado);
        }
    }
}
