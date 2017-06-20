using UnityEngine;
using System.Collections;
using System;

public class CaveGenerator : MonoBehaviour
{
	public GameObject groundTile;
	public GameObject wallTile;

	int randomFillPercent;
	int iterations;

	string seed;
	int[,] map;

	GameObject player;

	public CaveGenerator(int randomFillPercent, int iterations, GameObject groundTile, GameObject wallTile)
	{
		this.groundTile = groundTile;
		this.wallTile = wallTile;
		player = GameObject.Find("Player");
		this.randomFillPercent = randomFillPercent;
		this.iterations = iterations;
	}

	public int[,] GenerateCave(int width, int height)
	{
		map = new int[width, height];
		RandomFillMap(width, height);

		for (int i = 0; i < iterations; i++)
		{
			SmoothMap(width, height);
		}


		return map;
	}

	private void RandomFillMap(int width, int height)
	{
		seed = Time.time.ToString();

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
				{
					map[x, y] = 1;
				}
				else
				{
					map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
				}
			}
		}
	}

	private void SmoothMap(int width, int height)
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int neighbourWallTiles = GetSurroundingWallCount(x, y, width, height);

				if (neighbourWallTiles > 5)
					map[x, y] = 1;
				else if (neighbourWallTiles < 4)
					map[x, y] = 0;

			}
		}
	}

	private int GetSurroundingWallCount(int gridX, int gridY, int width, int height)
	{
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
		{
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
			{
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
				{
					if (neighbourX != gridX || neighbourY != gridY)
					{
						wallCount += map[neighbourX, neighbourY];
					}
				}
				else
				{
					wallCount++;
				}
			}
		}
		return wallCount;
	}
}