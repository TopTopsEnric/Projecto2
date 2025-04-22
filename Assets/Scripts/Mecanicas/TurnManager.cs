using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaseTurno
{
    ColocacionIngredientes,
    DespliegueUtensiliosEfectos,
    EjecucionAcciones,
    FinTurno
}
public class TurnManager : MonoBehaviour
{
    public int turnoActual = 1;
    public int maxTurnos = 10;

    public FaseTurno faseActual = FaseTurno.ColocacionIngredientes;

    public AsignarPieza jugador1AsignarPieza;
    public AsignarPieza jugador2AsignarPieza;
    public ComprobadorVictoria victoria;

    private bool jugador1Listo = false;
    private bool jugador2Listo = false;
    public Camera camaraJugador1;
    public Camera camaraJugador2;

    public float duracionFaseSegundos = 20f;
    private float timerFase;
    private bool partidaTerminada = false;
    void Start()
    {
        timerFase = duracionFaseSegundos;
        IniciarFase(FaseTurno.ColocacionIngredientes);
    }

    void Update()
    {
        if (faseActual == FaseTurno.FinTurno && partidaTerminada) return;

        timerFase -= Time.deltaTime;

        if (timerFase <= 0 && !partidaTerminada)
        {
            PasarFaseSiguiente();
        }
    }

    public void IniciarFase(FaseTurno nuevaFase)
    {
        if (partidaTerminada) return;

        faseActual = nuevaFase;
        timerFase = duracionFaseSegundos;
        jugador1Listo = false;
        jugador2Listo = false;

        Debug.Log($"Iniciando fase: {faseActual}");
        ActualizarCullingMask();

        switch (faseActual)
        {
            case FaseTurno.ColocacionIngredientes:
                jugador1AsignarPieza.SetTipoSeleccionIngredientes();
                jugador2AsignarPieza.SetTipoSeleccionIngredientes();
                break;
            case FaseTurno.DespliegueUtensiliosEfectos:
                jugador1AsignarPieza.SetTipoSeleccionUtensiliosEfectos();
                jugador2AsignarPieza.SetTipoSeleccionUtensiliosEfectos();
                break;
            case FaseTurno.EjecucionAcciones:
                jugador1AsignarPieza.EjecutarAccionesPendientes();
                jugador2AsignarPieza.EjecutarAccionesPendientes();
                PasarFaseSiguiente();
                break;
            case FaseTurno.FinTurno:
                turnoActual++;
                if (turnoActual > maxTurnos)
                {
                    Debug.Log("Partida terminada");
                    partidaTerminada = true;
                    string resultado = victoria.ComprobarVictoria();
                    // Aquí puedes llamar a un método para mostrar resultado en UI
                    Debug.Log("Resultado: " + resultado);
                }
                else
                {
                    IniciarFase(FaseTurno.ColocacionIngredientes);
                }
                break;
        }
    }

    public void PasarFaseSiguiente()
    {
        switch (faseActual)
        {
            case FaseTurno.ColocacionIngredientes:
                IniciarFase(FaseTurno.DespliegueUtensiliosEfectos);
                break;
            case FaseTurno.DespliegueUtensiliosEfectos:
                IniciarFase(FaseTurno.EjecucionAcciones);
                break;
            case FaseTurno.EjecucionAcciones:
                IniciarFase(FaseTurno.FinTurno);
                break;
            case FaseTurno.FinTurno:
                // Ya manejado en IniciarFase
                break;
        }
    }

    // Método para que los jugadores indiquen que están listos (botón listo)
    public void JugadorListo(int jugadorId)
    {
        if (jugadorId == 1) jugador1Listo = true;
        else if (jugadorId == 2) jugador2Listo = true;

        Debug.Log($"Jugador {jugadorId} está listo.");

        if (jugador1Listo && jugador2Listo)
        {
            PasarFaseSiguiente();
        }
    }

    // Método para obtener tiempo restante (para UI)
    public float GetTiempoRestante()
    {
        return Mathf.Max(timerFase, 0);
    }

    private void ActualizarCullingMask()
    {
        

        if (faseActual == FaseTurno.DespliegueUtensiliosEfectos)
        {
            // Mostrar ambos mapas en ambas cámaras
            camaraJugador1.cullingMask = (1 << 2) | (1 << 4) | (1 << 5);
            camaraJugador2.cullingMask = (1 << 2) | (1 << 4) | (1 << 5);
        }
        else
        {
            camaraJugador1.cullingMask = (1 << 2) | (1 << 5);
            camaraJugador2.cullingMask = (1 << 4) | (1 << 5);
        }
    }
}
