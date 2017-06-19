using UnityEngine;
using System.Collections;
using System;

public class CaveGenerator : MonoBehaviour
{
	public GameObject groundTile;
	public GameObject wallTile;

	int randomFillPercent;

	string seed;
	int[,] map;

	GameObject player;

	public CaveGenerator(int randomFillPercent)
	{
		player = GameObject.Find("Player");
		this.randomFillPercent = randomFillPercent;
	}

	public GameObject[,] GenerateCave(int locationX, int locationY, int width, int height)
	{
		map = new int[width, height];
		RandomFillMap(width, height);

		for (int i = 0; i < 5; i++)
		{
			SmoothMap(width, height);
		}

		GameObject[,] tiles = CreateTiles(locationX, locationY, width, height);

		return tiles;
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

				if (neighbourWallTiles > 4)
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


	private GameObject[,] CreateTiles(int locationX, int locationY, int width, int height)
	{
		GameObject[,] tiles = new GameObject[width, height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				GameObject newTile;
				if (map[x, y] == 1)
					newTile = Instantiate(wallTile, new Vector3(x * 1f + locationX, y * 1f + locationY, 0), Quaternion.identity) as GameObject;
				else
					newTile = Instantiate(groundTile, new Vector3(x * 1f, y * 1f, 0), Quaternion.identity) as GameObject;

				if (newTile.tag == "Block")
				{
					WallTile wallScript = newTile.GetComponent<WallTile>();
					wallScript.player = player;
					wallScript.AdjustSpriteAndHitbox(tiles, new Vector2(x, y));
				}

				tiles[x, y] = newTile;
			}
		}

		return tiles;
	}
}