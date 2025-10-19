using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Biome {
    public GameObject[] decorFloor;
    public GameObject[] decorTallFloor;
    public GameObject[] decorAnywhere;
}

public class Biomes : MonoBehaviour {
    public Color[] floors; // 0 is default: ignore
    public Color[] walls; // 0 is default: ignore
    public Biome[] decor;

    private Grid<int> grid = new Grid<int>();

    public int lastBiome = 0;

    public void Next(GridPos pos, int biome = -1, bool overrideOld = true) {
        if (!overrideOld && grid[pos] > 0) return;
        lastBiome = biome >= 0 ? biome : lastBiome;
        grid[pos] = lastBiome;
    }

    public int this[GridPos pos] => grid[pos];

    public Color DefaultFloor { get => floors[0]; }
    public Color DefaultWall { get => floors[0]; }
    public Color GetFloor(GridPos pos) {
        Debug.Log(grid[pos]);
        return floors[grid[pos]];
    }
    public Color GetWall(GridPos pos) {
        return walls[grid[pos]];
    }
}
