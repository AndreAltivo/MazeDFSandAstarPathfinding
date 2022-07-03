using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gizmos = Popcron.Gizmos;

public class Grid : MonoBehaviour{
    //tamanho do  grafo no mundo
    public Vector3 gridWorldSize;
    //raio do n� no mundo
    public float nodeRadius;
    //array para os grafos
    public Node[,] grid;
    //mascara que analisa se o n� n�o � um obstaculo
    public LayerMask unwalkableMask;
    //menor caminho encontrado
    public List<Node> path;
    //posi��o mais abaixo e a esquerda
    Vector3 worldBottomLeft;
    //variavel de controle da unity
    public bool gameStart;

    //tamanho do n�
    float nodeDiameter;
    //quantidades de n�s por X e Y
    public int gridSizeX, gridSizeY;

    public GameObject player, enemy;


    //Fun��o unity que � lida antes da fun��o Start iniciar
    private void Awake() {
        gameStart = false;
    }

    //cria��o do grafo
    public void CreateGrid() {
        //gera o espa�o do n�
        nodeDiameter = nodeRadius * 2;
        //quantidade de n�s do eixo x e y de acordo com o tamanho do grafo dividido pelo tamanho do n�
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        //cria��o da Array de n�s
        grid = new Node[gridSizeX, gridSizeY];
        //posi��o do primeiro n�
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        //popular a Array de n�s
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                grid[x, y] = new Node(false, worldPoint, x, y);

                if (Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask))
                    grid[x, y].SetObstacle(true);
                else
                    grid[x, y].SetObstacle(false);


            }
        }
        gameStart = true;
    }


    //obt�m os n�s vizinhos nas 4 dire��es cardeais
    public List<Node> GetNeighbors(Node node) {
        List<Node> neighbors = new List<Node>();

        //verifica e adiciona o vizinho superior
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(grid[node.GridX, node.GridY + 1]);

        //verifica e adiciona o vizinho inferior
        if (node.GridX >= 0 && node.GridX < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(grid[node.GridX, node.GridY - 1]);

        //verifica e adiciona o vizinho direito
        if (node.GridX + 1 >= 0 && node.GridX + 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(grid[node.GridX + 1, node.GridY]);

        //verifica e adiciona o vizinho esquerdo
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY >= 0 && node.GridY < gridSizeY)
            neighbors.Add(grid[node.GridX - 1, node.GridY]);


        //retorna os vizinhos
        return neighbors;
    }
    //da a localiza��o do n� no grafo de acordo com a sua posi��o do mundo
    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        int x = Mathf.RoundToInt(worldPosition.x - 1 + (gridSizeX / 2));
        int y = Mathf.RoundToInt(worldPosition.y + (gridSizeY / 2));
        if( x < 0) {
            x = 0;
        }else if (x > gridSizeX) {
            x = gridSizeX-1;
        }
        if (y< 0) {
            y = 0;
        } else if (y > gridSizeY) {
            y = gridSizeY-1;
        }
        return grid[x, y];
    }



    //Desenha a representa��o visual do grafo
    void Update() {
        Gizmos.Cube(transform.position,transform.rotation, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null) {
            foreach (Node n in grid) {

                if (n.obstacle)
                    Gizmos.Cube(n.worldPosition, transform.rotation, Vector3.one * (nodeRadius),Color.red);
                else
                    Gizmos.Cube(n.worldPosition, transform.rotation, Vector3.one * (nodeRadius), Color.white);
                
               
                if (path != null && path.Contains(n))
                    Gizmos.Cube(n.worldPosition, transform.rotation, Vector3.one * (nodeRadius), Color.black);
                Node playerN = NodeFromWorldPoint(new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y));
                Node enemyN = NodeFromWorldPoint(new Vector3(enemy.transform.position.x - transform.position.x, enemy.transform.position.y - transform.position.y));
                if (n == playerN) {
                    Gizmos.Cube(n.worldPosition, transform.rotation, Vector3.one * (nodeRadius), Color.cyan);
                }
                if (n == enemyN) {
                    Gizmos.Cube(n.worldPosition, transform.rotation, Vector3.one * (nodeRadius), Color.green);
                }

            }
        }
    }
}