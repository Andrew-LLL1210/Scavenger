using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // Clears our list gridPositions and prepares it to generate a new board.
    private void InitialiseList()
    {
        // Clear our list gridPositions.
        gridPositions.Clear();

        // Loop through x axis (columns).
        for (int x = 1; x < columns - 1; x++)
        {
            // Within each column, loop through y axis (rows).
            for (int y = 1; y < rows - 1; y++)
            {
                // At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    private void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length - 1)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    private Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    private void LayoutObjectAtRandom(GameObject[] tiles, int minmum, int maximum)
    {
        int objectCount = Random.Range(minmum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPos = RandomPosition();
            GameObject tileChoice = tiles[Random.Range(0, tiles.Length)];
            Instantiate(tileChoice, randomPos, Quaternion.identity);
        }
    }

    // return value indicates if there is a next level
    public bool SetupScene(int level)
    {
        string resource_name = String.Format("day{0}", level);
        TextAsset level_asset = Resources.Load(resource_name) as TextAsset;
        if (level_asset == null) return false;

        string[] lines = level_asset.text.Split('\n');
        for (int i = 0; i < lines.Length; i++) for (int j = 0; j < lines[i].Length; j++) {
            char c = lines[i][j];
            Vector3 pos = new Vector3(j - 1, rows - i, 0f);
            if (c == 'X') {
                GameObject wall = randomElement(wallTiles);
                Instantiate(wall, pos, Quaternion.identity);
            } else if (c == 'F') {
                GameObject food = randomElement(foodTiles);
                Instantiate(food, pos, Quaternion.identity);
            } else if (c == 'E') {
                GameObject enemy = randomElement(enemyTiles);
                Instantiate(enemy, pos, Quaternion.identity);
            } else if (c == 'T') {
                Instantiate(exit, pos, Quaternion.identity);
            }
            GameObject floor = randomElement(floorTiles);
            Instantiate(floor, pos, Quaternion.identity);
        }
        return true;
    }

    public T randomElement<T>(T[] list) {
        return list[Random.Range(0, list.Length)];
    }
}
