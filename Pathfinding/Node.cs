using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//classe nó usado pelo grafo de menores caminhos
public class Node {
    //custo g (custo do nó atual até o proximo) e custo h (custo até o destino usando heurística)
    public int gCost, hCost;
    //definição se o nó é um obstaculo para que seja ou não ignorado na busca
    public bool obstacle;
    //posição do nó no mundo do game
    public Vector3 worldPosition;
    //posição do nó dentro do Grafo
    public int GridX, GridY;
    //nó pai atual
    public Node parent;

    //creator
    public Node(bool _obstacle, Vector3 _worldPos, int _gridX, int _gridY) {
        obstacle = _obstacle;
        worldPosition = _worldPos;
        GridX = _gridX;
        GridY = _gridY;
    }
    // custo total do nó sendo considerado o peso principal
    public int FCost {
        get {
            return gCost + hCost;
        }

    }


    public void SetObstacle(bool isOb) {
        obstacle = isOb;
    }
}