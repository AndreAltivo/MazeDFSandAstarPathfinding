using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour{
    //classe de nós
    [System.Serializable]
    public class Cell {
        public bool visited;
        public GameObject north;//1
        public GameObject east;//2
        public GameObject west;//3
        public GameObject south;//4
        public GameObject pos;
    }
    
    public GameObject cam; //classe de objetos de uso da Unity
    public Grid Astar; // classe para o grafo de caminhos minimos
    public GameObject pathPos;//classe de objetos de uso da Unity
    public Camera camerasize;//classe de objetos de uso da Unity
    public GameObject wall;//classe de objetos de uso da Unity
    public GameObject borderWall;//classe de objetos de uso da Unity
    public GameObject center;//classe de objetos de uso da Unity
    public float wallLength;//classe de objetos de uso da Unity, da o tamanho das paredes
    public int xSize = 40;//tamanho x do labirinto/grafo
    public int ySize = 25;//tamanho y do labirinto/grafo
    public int totalCells;//quantidade de nós
    public Cell[] cells; //lista de nós

    private int currentCell = 0; // variavel de controle de nó
    private int visitedCells = 0; //numero de nós visitados para controle
    private int currentNeighbour = 0; //variavel de controle de vizinhos do nó
    private int backingUp=0; //variavel para retornar ao nó anterior
    private int wallToBreak; //variavel para a criação dos caminhos do labirinto
    private Vector2 initialPos; //posição inicial no mapa para a criação das paredes
    private Vector2 initialBorderPos; //posição inicial no mapa para a criação das bordas
    private GameObject wallHolder; //classe de objetos de uso da Unity
    private GameObject borderHolder; //classe de objetos de uso da Unity
    private GameObject centerHolder; //classe de objetos de uso da Unity
    private bool startedBuilding=false; //bool de controle
    private List<int> lastCells; // objeto para armazenar nós visitados

    // Start é chamado antes da atualização do primeiro quadro
    void Start()
    {
        totalCells = xSize * ySize; // adiciona a quantidade de nós
        camerasize = FindObjectOfType<Camera>(); //busca o objeto do tipo camera para a variavel
        camerasize.orthographicSize = (wallLength*xSize)/2; //muda o tamanho da camera
        Astar.gridWorldSize = new Vector2(xSize * wallLength, ySize * wallLength);// adiciona o tamanho que o grid do grafo de caminhos minimos vai ter
        CreateWall();
    }
    //essa função apenas posiciona os objetos de parede
    void CreateWall() {
        wallHolder = new GameObject();
        wallHolder.name = "MazeGame";
        borderHolder = new GameObject();
        borderHolder.name = "Border";
        centerHolder = new GameObject();
        centerHolder.name = "Center Cells";

        initialPos = new Vector2((-xSize / 2) + wallLength / 2, (-ySize / 2) + wallLength / 2);
        initialBorderPos = new Vector2(initialPos.x + (wallLength / 2), initialPos.y);
        Vector2 myPos = initialPos;
        Vector2 centerPos = initialBorderPos;
        // variaveis temporarias para instancia
        GameObject tempCenter; 
        GameObject tempWall;
        //paredes do Eixo X
        for (int i = 0; i < ySize; i++) {
            for (int j = 0; j <= xSize; j++) {
                //posição da parede
                myPos = new Vector2(initialPos.x + (j * wallLength) - wallLength / 2, initialPos.y + (i * wallLength) - wallLength / 2);
                //instancia da parede
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                //coloca no objeto pai para melhor controle
                tempWall.transform.parent = wallHolder.transform;

            }
        }
        //paredes do Eixo Y
        for (int i = 0; i <= ySize; i++) {
            for (int j = 0; j < xSize; j++) {
                //posição da parede
                myPos = new Vector2(initialPos.x + (j * wallLength), initialPos.y + (i * wallLength) - wallLength);
                //instancia da parede
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0f, 0f, 90f)) as GameObject;
                //coloca no objeto pai para melhor controle
                tempWall.transform.parent = wallHolder.transform;
            }
        }
        //posição do centro de cada nó para controle 
        for (int i = 0; i < ySize; i++) {
            for (int j = 0; j < xSize; j++) {
                //posição do centro
                centerPos = new Vector2(initialBorderPos.x + (j * wallLength) - wallLength / 2, initialBorderPos.y + (i * wallLength) - wallLength / 2);
                //instancia do centro
                tempCenter = Instantiate(center, centerPos, Quaternion.identity);
                //coloca no objeto pai para melhor controle
                tempCenter.transform.parent = centerHolder.transform;
                
            }

        }
        //Bordas
        for (int i = -1; i <= ySize; i++) {
            for (int j = -1; j <= xSize; j++) {
                if (i < 0 || i >= ySize || j < 0 || j >= xSize) {
                    //posição da parede
                    myPos = new Vector2(initialBorderPos.x + (j * wallLength) - wallLength / 2, initialBorderPos.y + (i * wallLength) - wallLength / 2);
                    //instancia da parede
                    tempWall = Instantiate(borderWall, myPos, Quaternion.identity) as GameObject;
                    //coloca no objeto pai para melhor controle
                    tempWall.transform.parent = borderHolder.transform;
                }
            }
        }
        CreateCells();

    }
    //criação dos nós
    void CreateCells() {
        lastCells = new List<int>();
        lastCells.Clear();
        
        //objeto temporario que mantem todas as paredes
        GameObject[] allWalls;
        GameObject[] allCenters;
        //conta a quantidade de objetos presentes
        int children = wallHolder.transform.childCount;
        int centers = centerHolder.transform.childCount;
        //preenche os Arrays
        allWalls = new GameObject[children];
        allCenters = new GameObject[centers];
        cells = new Cell[totalCells];
        //variaveis de controle para as posições
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;
        //armazena os objetos nos arrays
        for (int i = 0; i < children; i++) {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < centers; i++) {
            allCenters[i] = centerHolder.transform.GetChild(i).gameObject;
        }
        if (termCount == xSize) {
            eastWestProcess++;
            termCount = 0;
        }
        //criação dos nós
        for (int cellProcess = 0; cellProcess < cells.Length; cellProcess++) {
            if (termCount == xSize) {
                eastWestProcess ++;
                termCount = 0;
            }
            //criação do nó
            cells[cellProcess] = new Cell();
            //população das paredes e do centro do nó
            cells[cellProcess].east = allWalls[eastWestProcess];
            cells[cellProcess].south = allWalls[childProcess + (xSize + 1) * ySize];
            eastWestProcess++;
            termCount++;
            childProcess++;
            cells[cellProcess].west = allWalls[eastWestProcess];
            cells[cellProcess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
            cells[cellProcess].pos = allCenters[cellProcess];
        }
        //posiciona a camera e o pathfinding
        cam.gameObject.transform.position = new Vector3((cells[0].pos.transform.position.x + cells[totalCells - 1].pos.transform.position.x) / 2, (cells[0].pos.transform.position.y + cells[totalCells - 1].pos.transform.position.y) / 2, -10f);
        pathPos.transform.position = new Vector2((cells[0].pos.transform.position.x + cells[totalCells - 1].pos.transform.position.x) / 2, (cells[0].pos.transform.position.y + cells[totalCells - 1].pos.transform.position.y) / 2);
        CreateMaze();
    }
    //Criação de labirinto
    void CreateMaze() {
        while (visitedCells<totalCells) {
            if (startedBuilding) {//caso ja tenha feito a escolha do nó inicial entre aqui
                //essa função entrega todos os nós vizinhos do nó atual
                GiveMeNeighbours();
                //verifica se o vizinho ja foi visitado
                if(cells[currentNeighbour].visited==false && cells[currentCell].visited == true) {
                    //caso o nó não tenha sido visitado ele quebra a parede entre os dois nós
                    BreakWall();
                    //ativa que o vizinho está sendo visitado
                    cells[currentNeighbour].visited = true;
                    //soma no controlador
                    visitedCells++;
                    //coloca o nó atual na lista de nós ja visitados
                    lastCells.Add(currentCell);
                    //transforma o vizinho no nó atual
                    currentCell = currentNeighbour;
                    //avisa quem foi o nó anterior
                    if (lastCells.Count > 0) {
                        backingUp = lastCells.Count - 1;
                    }
                }
            } else {//aqui seleciona o nó inicial caso não exista
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }
        } 
        //aqui chama a função para a criação do grafo para menores caminhos
        Astar.CreateGrid();
        
    }
    //essa função serve para quebrar as paredes
    void BreakWall() {
        switch (wallToBreak) {
            case 1: 
                Destroy(cells[currentCell].north); 
                break;
            case 2:
                Destroy(cells[currentCell].east);
                break;
            case 3:
                Destroy(cells[currentCell].west);
                break;
            case 4:
                Destroy(cells[currentCell].south);
                break;
        }
    }
    //essa função da todos os vizinhos do nó e seleciona o proximo, caso tenha algum vizinho não visitado
    void GiveMeNeighbours() {
        int length = 0;
        int[] neighbors = new int[4];
        int[] conectedWalls = new int[4];
        int check = 0;
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;
        //adiciona os vizinhos cada nó tem no minimo 2 vizinhos e no maximo 4
        //west
        if (currentCell + 1 < totalCells && (currentCell + 1) != check) {
            if (cells[currentCell + 1].visited == false) {
                neighbors[length] = currentCell + 1;
                conectedWalls[length] = 3;
                length++;
            }
        }
        //east
        if (currentCell - 1 >= 0 && currentCell != check) {
            if (cells[currentCell - 1].visited == false) {
                neighbors[length] = currentCell - 1;
                conectedWalls[length] = 2;
                length++;
            }
        }
        //north
        if (currentCell + xSize < totalCells) {
            if (cells[currentCell + xSize].visited == false) {
                neighbors[length] = currentCell + xSize;
                conectedWalls[length] = 1;
                length++;
            }
        }
        //south
        if (currentCell - xSize >= 0) {
            if (cells[currentCell - xSize].visited == false) {
                neighbors[length] = currentCell - xSize;
                conectedWalls[length] = 4;
                length++;
            }

        }
        //seleciona o vizinho e diz qual é a parede que conecta entre eles para caso possa quebrar
        if (length != 0) {
            int theChosenOne = Random.Range(0, length);
            currentNeighbour = neighbors[theChosenOne];
            wallToBreak = conectedWalls[theChosenOne];
        } else {
            if (backingUp > 0) {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }
    // Update é chamada uma vez por quadro
    void Update()
    {
      
    }
}
