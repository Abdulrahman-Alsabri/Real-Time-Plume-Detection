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
    static int marginSize = (int)((gridSize - plumeSize) / 3f);
    public int plumeX1, plumeY1, plumeX2, plumeY2;
    public float[,] allValues = new float[gridSize, gridSize];
    public float[,] allValuesZeroToOne = new float[gridSize, gridSize];
    public float[,] plumeValues;
    public float[,] plumeValues90Degrees;
    public float[,] plumeValues180Degrees;
    public float[,] plumeValues270Degrees;

    private List<float[,]> allPlumesRegular = new List<float[,]>();
    private List<float[,]> allPlumes90Degrees = new List<float[,]>();
    private List<float[,]> allPlumes180Degrees = new List<float[,]>();
    private List<float[,]> allPlumes270Degrees = new List<float[,]>();
    public int numberOfPlumes = 10;
    private int plumeCounter = 0;

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        InitializePlumes();
        SpawnPlume();
    }

    public void Respawn()
    {
        SpawnPlume();
    }

    private void InitializePlumes()
    {
        for (int counter = 0; counter < numberOfPlumes; counter++)
        {
            plumeValues = new float[plumeSize, plumeSize];
            String fileName = "Assets\\PlumeData\\PlumeValues" + counter.ToString() + ".txt";
            String input = File.ReadAllText(fileName);

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

            plumeValues90Degrees = RotateMatrix(plumeValues, plumeSize);
            plumeValues180Degrees = RotateMatrix(plumeValues90Degrees, plumeSize);
            plumeValues270Degrees = RotateMatrix(plumeValues180Degrees, plumeSize);

            // add plume to Arraylist
            allPlumesRegular.Add(plumeValues);
            allPlumes90Degrees.Add(plumeValues90Degrees);
            allPlumes180Degrees.Add(plumeValues180Degrees);
            allPlumes270Degrees.Add(plumeValues270Degrees);
        }
    }

    static float[,] RotateMatrix(float[,] matrix, int n)
    {
        float[,] ret = new float[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = matrix[n - j - 1, i];
            }
        }

        return ret;
    }

    public void SpawnPlume(bool isRandomPlume = true, bool isRandomSpawn = true, int plumeIndex = 0, int orientation = 0, int randomX = 150, int randomY = 150)
    {
        // allValues
        for (int k = 0; k < gridSize; k++)
        {
            for (int s = 0; s < gridSize; s++)
            {
                // 6 is the average dissolved O2 in Indian River Lagoon
                allValues[k, s] = (float)random.NextDouble() + 6f;
            }
        }

        if (isRandomPlume)
        {
            orientation = random.Next(0, 4);
            if (orientation == 0)
            {
                plumeValues = allPlumesRegular[plumeCounter];
            }
            else if (orientation == 1)
            {
                plumeValues = allPlumes90Degrees[plumeCounter];
            }
            else if (orientation == 2)
            {
                plumeValues = allPlumes180Degrees[plumeCounter];
            }
            else if (orientation == 3)
            {
                plumeValues = allPlumes270Degrees[plumeCounter];
            }

            plumeCounter++;
            if (plumeCounter >= numberOfPlumes)
            {
                plumeCounter = 0;
            }
        }
        else
        {
            if (orientation == 0)
            {
                plumeValues = allPlumesRegular[plumeIndex];
            }
            else if (orientation == 1)
            {
                plumeValues = allPlumes90Degrees[plumeIndex];
            }
            else if (orientation == 2)
            {
                plumeValues = allPlumes180Degrees[plumeIndex];
            }
            else if (orientation == 3)
            {
                plumeValues = allPlumes270Degrees[plumeIndex];
            }
        }


        bool isValidSpawnPos = false;
        if (isRandomSpawn)
        {
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
        }

        if (!isRandomSpawn || isValidSpawnPos)
        {

            if ((randomX <= (gridSize / 2f) && randomY <= (gridSize / 2f)) ||
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
                // Debug.Log("Original: " + plumeX1.ToString() + ", " + plumeY1.ToString() + " | " + isValidSpawnPos);

                for (int i = 0; i < plumeSize; i++)
                {
                    for (int j = 0; j < plumeSize; j++)
                    {
                        allValues[randomX + i, randomY + j] = plumeValues[i, j];
                    }
                }
            }

            ZeroToOneScale();
        }
    }

    public bool SpawnValidity(int randomX, int randomY)
    {
        if (randomX > 0 && randomY > 0 && randomX < gridSize && randomY < gridSize)
        {
            if ((randomX + plumeSize <= gridSize - marginSize && randomY + plumeSize <= gridSize - marginSize) ||
                    (randomX + plumeSize <= gridSize - marginSize && randomY - plumeSize >= marginSize) ||
                    (randomX - plumeSize >= marginSize && randomY + plumeSize <= gridSize - marginSize) ||
                    (randomX - plumeSize >= marginSize && randomY - plumeSize >= marginSize))
            {
                return true;
            }
        }

        return false;
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
