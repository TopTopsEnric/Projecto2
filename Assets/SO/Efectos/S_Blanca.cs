using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "S_Blanca", menuName = "Tools/resources/efectos/S_Blanca")]
public class S_Blanca : Efectos
{
    public override void ActivarEfecto(List<Node> neighbors, Node nodoOrigen)
    {
        // No se usa en este caso
    }

    // Método para crear una instancia activa del efecto
    public EfectoBlancoActivo CrearInstancia(Node origen, Node destino)
    {
        return new EfectoBlancoActivo
        {
            origen = origen,
            destino = destino,
            turnosRestantes = 3,
            efecto = this
        };
    }
}
