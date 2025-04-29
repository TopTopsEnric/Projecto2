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
        // Muestra un mensaje en la consola para confirmar que se llamó al método
        Debug.Log("DetectarNodo llamado");

        // Si no hay selección activa o no hay recurso seleccionado, salir del método
        if (tipoSeleccionActual == TipoSeleccion.Ninguno || recurso == null) return;

        

        // Lanza un rayo desde la cámara del jugador hacia donde está el mouse
        Ray ray = camaraJugador.ScreenPointToRay(Input.mousePosition);

        // Si el rayo impacta con algo en las capas seleccionadas...
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            // Verifica que el objeto tocado tenga el tag "Nodo"
            if (hit.collider.CompareTag("Nodo"))
            {
                

                Node nodo = null;

                

                // Verifica si el jugador tiene suficiente dinero para colocar el recurso
                if (!economiaJugador.PuedePagar(recurso.Price))
                {
                    Debug.Log("No tienes suficiente dinero para colocar este recurso.");
                    return;
                }

                // Valida si el recurso es del tipo correcto según la fase de selección
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

                // Si es un utensilio, puede requerir varios nodos. Se calcula cuántos nodos necesita.
                int maxNodos = 1;
                if (tipoSeleccionActual == TipoSeleccion.Utensilio)
                {
                    Utensilio utensilioActual = recurso as Utensilio;
                    if (utensilioActual != null)
                    {
                        maxNodos = utensilioActual.nodosRequeridos;
                    }
                }

                // Si el nodo aún no está en la lista de seleccionados
                if (!nodosSeleccionados.Contains(nodo))
                {
                    // Si no se ha alcanzado el límite de nodos requeridos
                    if (nodosSeleccionados.Count < maxNodos)
                    {
                        // Se añade el nodo a la lista de seleccionados
                        nodosSeleccionados.Add(nodo);
                        Debug.Log("Nodo seleccionado: " + nodoPosicion);

                        cambiarvisual(); // Actualiza visualmente los nodos seleccionados

                        // Si es ingrediente o efecto, se coloca inmediatamente
                        if (tipoSeleccionActual == TipoSeleccion.Ingrediente || tipoSeleccionActual == TipoSeleccion.Efecto)
                        {
                            economiaJugador.LessMoney(recurso.Price);

                            if (tipoSeleccionActual == TipoSeleccion.Ingrediente)
                            {
                                nodo.SetIngrediente(recurso);
                            }
                            else if (tipoSeleccionActual == TipoSeleccion.Efecto)
                            {
                                ((Efectos)recurso).ActivarEfecto(null, nodo);
                            }

                            LimpiarSeleccion(); // Limpia la selección tras aplicar
                        }
                        // Si es un utensilio y ya están todos los nodos seleccionados
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
        nodeMapPropio.DetectorFormaciones();
        nodeMapPropio.ejecutarPasiva();
        nodeMapPropio.EjecutarEfectosTemporales();
        EjecutarEfectoBlanco();
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

    public void EjecutarEfectoBlanco()
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

        
    }

    public void LimpiarSeleccion()
    {
        nodosSeleccionados.Clear();
        Debug.Log("Selección limpiada");
    }
}
