using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

 struct Coords
    {

        public int X;
        public int Y;
        public Coords(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }


public class Simulation : MonoBehaviour
{

   
    // Start is called before the first frame update
    public int height = 100;
    public int width = 100;
    public int grainPositionX1 = 25;
    public int grainPositionY1 = 25;
    public int grainPositionX2 = 70;
    public int grainPositionY2 = 70;
    public int maxValue = 4;
    public int numberIterations = 0;
    public int changeOfSeason = 70;
    public bool summer = false;
    public float cellSize = 2.0f;
    public int[,] gridArray;
    public bool[,] usedArrayPlant;
    public GameObject plant;
    public GameObject[,] gridArrayPlants;
    private float simulateInSeconds = 1.0f;
    private float simulationTimer = 0;
    private int simulationTimer2 = 0;

    void Start()
    {
        gridArray = new int[width, height];
        gridArrayPlants = new GameObject[width, height];
        usedArrayPlant = new bool[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                gridArray[i,j] = 0;
                usedArrayPlant[i,j] = false;
            }
        }

        
    }

    Vector3 getWorldPosition(int x, int y) {
        return new Vector3(x,0,y)*cellSize;
    }

    void sandSimulation() {
        if (this.numberIterations >= this.changeOfSeason) {
            this.summer = !this.summer;
            this.numberIterations = 0;
        }
        this.numberIterations += 1;
        var events = new Stack<Coords>();
        var originalX = this.grainPositionX1;
        var originalY = this.grainPositionY1;
        
        if (this.summer) {
            var newEvent = new Coords(this.grainPositionX1, this.grainPositionY1);
            events.Push(newEvent);
        } else {
            var newEvent = new Coords(this.grainPositionX2, this.grainPositionY2);
            events.Push(newEvent);
            originalX = this.grainPositionX2;
            originalY = this.grainPositionY2;
        }
       

        while (events.Count > 0) {
            Coords newEvent = events.Pop();
            var posX = newEvent.X;
            var posY = newEvent.Y;
            this.gridArray[posX,posY] += 1;
            if (this.gridArray[posX,posY] >= this.maxValue) {
                this.gridArray[posX,posY] -= 4;
                if (Math.Abs(originalX - ((posX + 1) % width)) <= 25) {
                    events.Push( new Coords((posX + 1) % width, posY));
                }
                if (Math.Abs(originalX - ((posX - 1) % width)) <= 25) {
                    events.Push(new Coords((posX - 1) % width, posY));
                }
                if (Math.Abs(originalY - ((posY + 1) % height)) <= 25) {
                    events.Push(new Coords(posX, (posY + 1) % height));
                }
                if (Math.Abs(originalY - ((posY - 1) % height)) <= 25) {
                    events.Push(new Coords(posX, (posY - 1) % height));
                }
            }
        }

        if (this.summer) {
            originalX = this.grainPositionX2;
            originalY = this.grainPositionY2;
        } else {
            originalX = this.grainPositionX1;
            originalY = this.grainPositionY1;
        }
       

        for(int i = 0;i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (Math.Abs(originalX - i) <= 25 && Math.Abs(originalY - j) <= 25) {
                    this.gridArray[i,j] -= 1;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(simulationTimer >= simulateInSeconds)
        {
            //Debug.Log(simulationTimer2);
            this.sandSimulation();

            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (gridArray[i,j] >= 1) {
                        if (!usedArrayPlant[i,j]) {
                            GameObject newPlant = Instantiate(plant);
                            newPlant.transform.position = getWorldPosition(i, j);
                            gridArrayPlants[i,j] =  newPlant;
                            usedArrayPlant[i,j] = true;

                        }
                        this.gridArrayPlants[i,j].GetComponent<LSystemExecutor>().derivations = gridArray[i,j];
                        int childs = this.gridArrayPlants[i,j].transform.childCount;
                        for (int k = childs - 1; k > 0; k--)
                        {
                            GameObject.Destroy(this.gridArrayPlants[i,j].transform.GetChild(k).gameObject);
                        }
                        this.gridArrayPlants[i,j].GetComponent<LSystemExecutor>().update(simulationTimer2);
                    }
                }
            }
            simulationTimer = 0;
            simulationTimer2 += 1;
        }
        simulationTimer += Time.deltaTime * 1.0f;
    }
}
