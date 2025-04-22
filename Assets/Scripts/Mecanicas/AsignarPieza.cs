using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsignarPieza : MonoBehaviour
{
    public Economia economiaJugador;
    public Camera camaraJugador;
    public NodeMap nodeMapPropio;
    public NodeMap nodeMapEnemigo;
    public int jugadorActual = 1;
    public ResourcesSO recurso;
   
    public bool inputHabilitado = true;

    private List<Node> nodosSeleccionados = new List<Node>();
    private List<(Utensilio utensilio, List<Node> nodos)> accionesUtensiliosPendientes = new List<(Utensilio, List<Node>)>();

    private enum TipoSeleccion
    {
        Ninguno,
        Ingrediente,
        Efecto,
        Utensilio
    }

    private TipoSeleccion tipoSeleccionActual = TipoSeleccion.Ninguno;

    // Variables específicas para el efecto S_Blanca
    private Node nodoOrigenSBlanca = null;
    private Node nodoDestinoSBlanca = null;
    private List<EfectoBlancoActivo> efectosBlancosActivos = new List<EfectoBlancoActivo>();

    void Update()
    {
        if (!inputHabilitado) return;

        if (Input.GetMouseButtonDown(0))
        {
            DetectarNodo();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EjecutarAccionesPendientes();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LimpiarSeleccion();
        }
    }

    public void SetRecurso(ResourcesSO nuevoRecurso)
    {
        Debug.Log("SetRecurso llamado con recurso: " + nuevoRecurso.Name);
        if (nuevoRecurso == null)
        {
            recurso = null;
            tipoSeleccionActual = TipoSeleccion.Ninguno;
            LimpiarSeleccion();
            Debug.Log("Recurso nulo, selección limpiada.");
            return;
        }
        // Verificar si hay dinero suficiente
        if (!economiaJugador.PuedePagar(nuevoRecurso.Price))
        {
            Debug.Log("No tienes suficiente dinero para seleccionar este recurso.");
            return;
        }

        if (nuevoRecurso != recurso)
        {
            LimpiarSeleccion();

            if (nuevoRecurso is IngredientesSO)
                tipoSeleccionActual = TipoSeleccion.Ingrediente;
            else if (nuevoRecurso is Efectos)
                tipoSeleccionActual = TipoSeleccion.Efecto;
            else if (nuevoRecurso is Utensilio)
                tipoSeleccionActual = TipoSeleccion.Utensilio;
            else
                tipoSeleccionActual = TipoSeleccion.Ninguno;
        }

        recurso = nuevoRecurso;
    }

    // Nuevos métodos para cambiar tipo de selección según fase
    public void SetTipoSeleccionIngredientes()
    {
        recurso = null; // Permitir solo ingredientes
        tipoSeleccionActual = TipoSeleccion.Ingrediente;
        inputHabilitado = true;
    }

    public void SetTipoSeleccionUtensiliosEfectos()
    {
        recurso = null; // Por defecto ninguno, se selecciona recurso externo
        tipoSeleccionActual = TipoSeleccion.Ninguno;
        inputHabilitado = true;
    }

    public void DetectarNodo()
    {
        Debug.Log("DetectarNodo llamado");  
        if (tipoSeleccionActual == TipoSeleccion.Ninguno || recurso == null) return;
        Debug.Log("no esta nada vacio el recurso");
        int layerMask = LayerMask.GetMask("Water", "Ignore Raycast");
        Ray ray = camaraJugador.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.CompareTag("Nodo"))
            {
                int nodoPosicion = int.Parse(hit.collider.gameObject.name);

                Node nodo = null;

                if (nodeMapPropio.nodes.ContainsKey(nodoPosicion))
                {
                    nodo = nodeMapPropio.nodes[nodoPosicion];
                }
                else
                {
                    Debug.LogWarning("Nodo no pertenece al tablero propio.");
                    return;
                }

                // Verificar si el jugador tiene suficiente dinero para colocar el recurso
                if (!economiaJugador.PuedePagar(recurso.Price))
                {
                    Debug.Log("No tienes suficiente dinero para colocar este recurso.");
                    return;
                }

                // Validaciones según tipoSeleccionActual y recurso
                if (tipoSeleccionActual == TipoSeleccion.Ingrediente)
                {
                    if (!(recurso is IngredientesSO))
                    {
                        Debug.Log("Solo puedes colocar ingredientes en esta fase.");
                        return;
                    }
                }
                else if (tipoSeleccionActual == TipoSeleccion.Utensilio)
                {
                    if (!(recurso is Utensilio))
                    {
                        Debug.Log("Solo puedes colocar utensilios en esta fase.");
                        return;
                    }
                }
                else if (tipoSeleccionActual == TipoSeleccion.Efecto)
                {
                    if (!(recurso is Efectos))
                    {
                        Debug.Log("Solo puedes colocar efectos en esta fase.");
                        return;
                    }
                }

                // Si es utensilio y requiere varios nodos, manejar selección múltiple
                int maxNodos = 1;
                if (tipoSeleccionActual == TipoSeleccion.Utensilio)
                {
                    Utensilio utensilioActual = recurso as Utensilio;
                    if (utensilioActual != null)
                    {
                        maxNodos = utensilioActual.nodosRequeridos;
                    }
                }

                if (!nodosSeleccionados.Contains(nodo))
                {
                    if (nodosSeleccionados.Count < maxNodos)
                    {
                        nodosSeleccionados.Add(nodo);
                        Debug.Log("Nodo seleccionado: " + nodoPosicion);

                        cambiarvisual();

                        // Si ya se seleccionaron todos los nodos requeridos, aplicar el recurso
                        if (tipoSeleccionActual == TipoSeleccion.Ingrediente || tipoSeleccionActual == TipoSeleccion.Efecto)
                        {
                            // Descontar dinero y aplicar recurso
                            economiaJugador.LessMoney(recurso.Price);

                            if (tipoSeleccionActual == TipoSeleccion.Ingrediente)
                            {
                                nodo.SetIngrediente(recurso);
                            }
                            else if (tipoSeleccionActual == TipoSeleccion.Efecto)
                            {
                                ((Efectos)recurso).ActivarEfecto(null, nodo);
                            }

                            LimpiarSeleccion();
                        }
                        else if (tipoSeleccionActual == TipoSeleccion.Utensilio && nodosSeleccionados.Count == maxNodos)
                        {
                            economiaJugador.LessMoney(recurso.Price);

                            Utensilio utensilio = (Utensilio)recurso;
                            accionesUtensiliosPendientes.Add((utensilio, new List<Node>(nodosSeleccionados)));
                            Debug.Log($"Acción de utensilio '{utensilio.name}' guardada con {nodosSeleccionados.Count} nodos.");
                            LimpiarSeleccion();
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Ya has seleccionado el máximo de nodos permitidos ({maxNodos}) para este recurso.");
                    }
                }
                else
                {
                    Debug.Log("Nodo ya seleccionado: " + nodoPosicion);
                }
            }
        }
    }

    private Vector2Int ConvertirIndiceAPosicion(int indice, int width)
    {
        int x = indice % width;
        int y = indice / width;
        return new Vector2Int(x, y);
    }

    public void cambiarvisual()
    {
        if (recurso == null || nodosSeleccionados.Count == 0) return;

        if (recurso is IngredientesSO ingrediente)
        {
            foreach (var nodo in nodosSeleccionados)
            {
                nodo.nodemap.ChangeNodeSprite(nodo.nodeid, ingrediente);
            }
        }
        else if (recurso is Efectos efecto)
        {
            foreach (var nodo in nodosSeleccionados)
            {
                efecto.ActivarEfecto(null, nodo);
            }
        }
        else if (recurso is Utensilio utensilio)
        {
            foreach (var nodo in nodosSeleccionados)
            {
                nodo.SetUtensilioVisual(utensilio);
            }
        }
    }

    public void EjecutarAccionesPendientes()
    {
        EjecutarTurno();
        if (accionesUtensiliosPendientes.Count == 0)
        {
            Debug.Log("No hay acciones pendientes para ejecutar.");
            return;
        }

        foreach (var accion in accionesUtensiliosPendientes)
        {
            Utensilio utensilio = accion.utensilio;
            List<Node> nodos = accion.nodos;

            Debug.Log($"Ejecutando utensilio '{utensilio.name}' con {nodos.Count} nodos.");

            foreach (var nodo in nodos)
            {
                if (nodo.utensilioSpriteRenderer != null)
                {
                    nodo.utensilioSpriteRenderer.sprite = null;
                    nodo.utensilioSpriteRenderer.enabled = false;
                }
            }

            utensilio.ActivarEfecto(nodos, nodos[0]);
        }

        accionesUtensiliosPendientes.Clear();
        Debug.Log("Todas las acciones pendientes han sido ejecutadas.");
    }

    public void EjecutarTurno()
    {
        // Ejecutar efectos activos de S_Blanca
        for (int i = efectosBlancosActivos.Count - 1; i >= 0; i--)
        {
            var efecto = efectosBlancosActivos[i];

            if (efecto.origen.ingrediente && !efecto.destino.ingrediente)
            {
                efecto.destino.SetIngrediente(efecto.origen.recurso);
                efecto.origen.destruirIngrediente();
            }

            efecto.turnosRestantes--;

            if (efecto.turnosRestantes <= 0)
            {
                efectosBlancosActivos.RemoveAt(i);
            }
        }

        // Aquí puedes añadir la ejecución de otros efectos temporales o acciones pendientes
    }

    public void LimpiarSeleccion()
    {
        nodosSeleccionados.Clear();
        Debug.Log("Selección limpiada");
    }
}
