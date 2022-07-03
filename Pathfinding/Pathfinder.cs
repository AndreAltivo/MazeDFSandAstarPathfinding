using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour{
    //objeto de origem e destino
    public Transform seeker, target;
    Grid grid;
    //posição da origem e do destino no grafo
    Node seekerNode, targetNode;
    public GameObject GridOwner;


    void Start() {
        //Instantiate grid
        grid = GridOwner.GetComponent<Grid>();
    }

    //Update é chamada uma vez por quadro, esse jogo roda a 60 quadros por segundo
    private void Update() {
        //verifica se o jogo iniciou para não acontecer erros de Nulo
        if (grid.gameStart) {
            //se sim busca o caminho
            FindPath(seeker.position, target.position);
        }
    }
    //buscador de caminhos
    public void FindPath(Vector3 startPos, Vector3 targetPos) {
        //obter a posição do jogador e do alvo nas coordenadas do grafo
        seekerNode = grid.NodeFromWorldPoint(new Vector3(startPos.x-grid.transform.position.x, startPos.y - grid.transform.position.y));
        targetNode = grid.NodeFromWorldPoint(new Vector3(targetPos.x - grid.transform.position.x, targetPos.y - grid.transform.position.y));

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(seekerNode);

        //calcula o caminho para encontrar o menor caminho
        while (openSet.Count > 0) {

            //itera através do openSet e encontra o menor custo F
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                
                if (openSet[i].FCost <= node.FCost) {//se o custo F for menor ou igual ele entra aqui
                    if (openSet[i].hCost < node.hCost) //caso o custo seja igual, ele avalia o custo H
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            //Se o destino for encontrado, refaça o caminho
            if (node == targetNode) {
                RetracePath(seekerNode, targetNode);
                return;
            }

            //adiciona nós vizinhos ao openSet
            foreach (Node neighbour in grid.GetNeighbors(node)) {
                if (neighbour.obstacle || closedSet.Contains(neighbour)) { //se o vizinho for obstaculo ou estiver fechado ignora
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);//pega o custo do nó atual pro nó viznho
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);//aqui pega o custo do nó vizinho até o alvo
                    neighbour.parent = node;//se transforma no pai do vizinho atual

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    //inverte o caminho calculado para que o primeiro nó esteja mais próximo do buscador
    void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        //retorna o caminho para o grafo para que aplique os efeitos visuais
        grid.path = path;

    }

    //obtém a distância entre 2 nós para calcular o custo usando a Distancia de Manhattan
    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}


