using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour {
    private static WorldGrid instance;
    public static WorldGrid I { get => instance; }
    WorldGrid(): base() {
        instance = this;
    }
    public static Vector3 Scale { get => I.scale; }

    public GridPiece prefab;
    public Vector3 scale = new Vector3(1, 1, 1);
    public Material defaultMaterial;
    public ParticleSystem dustPrefab;
    public ParticleSystem undustPrefab;

    public Action<GridPos, int> PosUpdated;

    public SurfaceGrid grid = new SurfaceGrid();
    public static SurfaceGrid Grid { get => instance.grid; }

    private Dictionary<TriPos, GridPiece> renderGrid = new Dictionary<TriPos, GridPiece>();
    public Transform GetRenderPos(GridPos pos) {
        bool childExists = renderGrid.TryGetValue(pos.Triangles[0], out GridPiece parent);
        return childExists ? parent.transform : null;
    }
    public void UpdateRenderPos(GridPos pos, int relMinNeedsUpdate, int relMaxNeedsUpdate) {
        for (int i = relMaxNeedsUpdate; i >= relMinNeedsUpdate - 1; i--) {
            GridPos posToCheck;
            if (i >= relMinNeedsUpdate) posToCheck = pos + GridPos.up * i;
            else posToCheck = grid.GetSurfacePos(pos) - GridPos.up;
            foreach (TriPos tri in posToCheck.Triangles) {
                bool childExists = renderGrid.TryGetValue(tri, out GridPiece child);
                if (!childExists) {
                    child = GameObject.Instantiate(WorldGrid.I.prefab, tri.World,
                        tri.right ? Quaternion.identity : Quaternion.Euler(0, 180, 0), transform);
                    child.Pos = tri;
                    renderGrid[tri] = child;
                } else child.Refresh();
            }
        }
    }

    public void Set(GridMod mod) {
        if (mod.IsUnnecessary) return;
        ForceSetPos(mod);
        if (mod.materialType == Block.AIR) MakeDust(mod.pos, 0, mod.height - 1, true);
    }

    private void ForceSetPos(GridMod mod) {
        for (int i = 0; i < mod.height; i++)
            grid[mod.pos + GridPos.up * i] = mod.materialType;
        UpdateRenderPos(mod.pos, -1, mod.height);
        if (PosUpdated != null) PosUpdated(mod.pos, mod.height);
    }

    private void MakeDust(GridPos pos, int relMinUpdated, int relMaxUpdated, bool open) {
        Transform dust;
        if (open) dust = GameObject.Instantiate(dustPrefab).transform;
        else dust = GameObject.Instantiate(undustPrefab).transform;
        dust.position = pos.World + Vector3.up * WorldGrid.Scale.y * relMinUpdated;
        dust.localScale = Vector3.Scale(WorldGrid.Scale, new Vector3(1, relMaxUpdated - relMinUpdated, 1));
    }
}
