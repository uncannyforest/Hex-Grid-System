using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D<T> {
    private List<List<T>>[] quads = new List<List<T>>[] {
        new List<List<T>>(), // x y >= 0
        new List<List<T>>(), // y
        new List<List<T>>(), // 
        new List<List<T>>()}; // x

    private T get2D(List<List<T>> list2d, int x, int y) {
        if (list2d.Count <= y) return default;
        List<T> list = list2d[y];
        if (list.Count <= x) return default;
        return list[x];
    }

    private void set2D(List<List<T>> list2d, int x, int y, T value) {
        while (list2d.Count <= y) list2d.Add(new List<T>());
        List<T> list = list2d[y];
        while (list.Count <= x) list.Add(default);
        list[x] = value;
    }

    private void ThrowIfLarge(GridPos pos) {
        if (1000_000 <= pos.x || 1000_000 <= pos.y
                || 1000_000 <= -pos.x || 1000_000 <= -pos.y)
            throw new ArgumentOutOfRangeException("Huge GridPos " + pos);
    }

    private void ThrowIf3D(GridPos pos) {
        if (pos.w != 0)
            throw new ArgumentOutOfRangeException("GridPos.w is not 0" + pos);
    }

    public T this[GridPos pos] {
        get {
            ThrowIf3D(pos);
            ThrowIfLarge(pos);
            int xIndex = pos.x >= 0 ? pos.x : -1 - pos.x;
            int yIndex = pos.y >= 0 ? pos.y : -1 - pos.y;
            int quadIndex = pos.x >= 0 ? 1 : 0;
            if (pos.y >= 0) quadIndex = 3 - quadIndex;
            return get2D(quads[quadIndex], xIndex, yIndex);
        }
        set {
            ThrowIf3D(pos);
            ThrowIfLarge(pos);
            int xIndex = pos.x >= 0 ? pos.x : -1 - pos.x;
            int yIndex = pos.y >= 0 ? pos.y : -1 - pos.y;
            if (xIndex > xMax) xMax = xIndex;
            if (yIndex > yMax) yMax = yIndex;
            if (xIndex < xMin) xMin = xIndex;
            if (yIndex < yMin) yMin = yIndex;
            int quadIndex = pos.x >= 0 ? 1 : 0;
            if (pos.y >= 0) quadIndex = 3 - quadIndex;
            set2D(quads[quadIndex], xIndex, yIndex, value);
        }
    }

    private int xMax = -1;
    private int yMax = -1;
    private int xMin = 0;
    private int yMin = 0;

    public BoundsInt Bounds { get => new BoundsInt(xMin, yMin, 0, xMax, yMax, 0); }
}
