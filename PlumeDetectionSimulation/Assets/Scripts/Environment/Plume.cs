using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class Plume : MonoBehaviour
{
    static int gridSize = 300;
    static int plumeSize = 150;
    static int marginSize = (int)((gridSize - plumeSize) / 2f);
    public int plumeX1, plumeY1, plumeX2, plumeY2;
    public float[,] allValues = new float[gridSize, gridSize];
    public float[,] allValuesZeroToOne = new float[gridSize, gridSize];
    public float[,] plumeValues = new float[plumeSize, plumeSize];

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        ReadPlumeData();
        SpawnPlume();
        ZeroToOneScale();
    }

    public void Respawn()
    {
        ReadPlumeData();
        SpawnPlume();
        ZeroToOneScale();
    }

    private void ReadPlumeData()
    {
        String input = "";

        if (plumeSize == 150)
        {
            input = File.ReadAllText("Assets/PlumeValues150.txt");
        }
        else if (plumeSize == 200)
        {
            input = File.ReadAllText("Assets/PlumeValues200.txt");
        }

        // plume
        int i = 0, j = 0;
        foreach (var row in input.Split('\n'))
        {
            j = 0;
            foreach (var col in row.Trim().Split(' '))
            {
                plumeValues[i, j] = float.Parse(col.Trim(), CultureInfo.InvariantCulture.NumberFormat);
                j++;
            }
            i++;
        }

        // allValues
        for (int k = 0; k < gridSize; k++)
        {
            for (int s = 0; s < gridSize; s++)
            {
                // 6 is the average dissolved O2 in Indian River Lagoon
                allValues[k, s] = (float)random.NextDouble() + 6f;
            }
        }
    }

    private void SpawnPlume()
    {
        int randomX = 0;
        int randomY = 0;

        bool isValidSpawnPos = false;

        while (!isValidSpawnPos)
        {
            randomX = random.Next(0, gridSize);
            randomY = random.Next(0, gridSize);

            if ((randomX + plumeSize <= gridSize - marginSize && randomY + plumeSize <= gridSize - marginSize) ||
                (randomX + plumeSize <= gridSize - marginSize && randomY - plumeSize >= marginSize) ||
                (randomX - plumeSize >= marginSize && randomY + plumeSize <= gridSize - marginSize) ||
                (randomX - plumeSize >= marginSize && randomY - plumeSize >= marginSize))
            {
                isValidSpawnPos = true;
            }
        }

        if ((randomX <= (gridSize/2f) && randomY <= (gridSize / 2f)) ||
            (randomX > (gridSize / 2f) && randomY <= (gridSize / 2f)) ||
            (randomX <= (gridSize / 2f) && randomY > (gridSize / 2f)) ||
            (randomX > (gridSize / 2f) && randomY > (gridSize / 2f)))
        {

            // bottom left corner
            if (randomX + plumeSize <= gridSize - marginSize && randomY - plumeSize >= marginSize)
            {
                randomY -= plumeSize;
            }

            // top right corner
            else if (randomX - plumeSize >= marginSize && randomY + plumeSize <= gridSize - marginSize)
            {
                randomX -= plumeSize;
            }

            // bottom right corner
            else if (randomX - plumeSize >= marginSize && randomY - plumeSize >= marginSize)
            {
                randomX -= plumeSize;
                randomY -= plumeSize;
            }

            plumeX1 = randomX;
            plumeX2 = randomX + plumeSize;
            plumeY1 = randomY;
            plumeY2 = randomY + plumeSize;

            for (int i = 0; i < plumeSize; i++)
            {
                for (int j = 0; j < plumeSize; j++)
                {
                    allValues[randomX + i, randomY + j] = plumeValues[i, j];
                }
            }
        }
    }

    private void ZeroToOneScale()
    {
        float min = 13;
        float max = 0;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (allValues[i, j] < min)
                {
                    min = allValues[i, j];
                }

                if (allValues[i, j] > max)
                {
                    max = allValues[i, j];
                }
            }
        }

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                allValuesZeroToOne[i, j] = (allValues[i, j] - min) / (max - min) * 1.3f;
            }
        }

    }
}
