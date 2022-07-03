using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//classe n� usado pelo grafo de menores caminhos
public class Node {
    //custo g (custo do n� atual at� o proximo) e custo h (custo at� o destino usando heur�stica)
    public int gCost, hCost;
    //defini��o se o n� � um obstaculo para que seja ou n�o ignorado na busca
    public bool obstacle;
    //posi��o do n� no mundo do game
    public Vector3 worldPosition;
    //posi��o do n� dentro do Grafo
    public int GridX, GridY;
    //n� pai atual
    public Node parent;

    //creator
    public Node(bool _obstacle, Vector3 _worldPos, int _gridX, int _gridY) {
        obstacle = _obstacle;
        worldPosition = _worldPos;
        GridX = _gridX;
        GridY = _gridY;
    }
    // custo total do n� sendo considerado o peso principal
    public int FCost {
        get {
            return gCost + hCost;
        }

    }


    public void SetObstacle(bool isOb) {
        obstacle = isOb;
    }
}