using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Biomes))]
public class MaterialGrid : MonoBehaviour {
    private static MaterialGrid instance;
    public static MaterialGrid I { get => instance; }
    MaterialGrid(): base() {
        instance = this;
    }
    public static Vector3 Scale { get => I.scale; }

    public GridPiece prefab;
    public Vector3 scale = new Vector3(1, 1, 1);
    public MaterialShape shape;
    public Material defaultMaterial;
    public ParticleSystem dustPrefab;
    public ParticleSystem undustPrefab;

    public Grid<bool> grid = new Grid<bool>();
    public static Grid<bool> Grid { get => instance.grid; }

    private Dictionary<TriPos, GridPiece> renderGrid = new Dictionary<TriPos, GridPiece>();

    private Biomes biome;
    public static Biomes Biome {
        get {
            if (I.biome == null) I.biome = I.GetComponent<Biomes>();
            return I.biome;
        }
    }

    public void UpdatePos(GridPos pos, int relMinNeedsUpdate, int relMaxNeedsUpdate) {
        for (int i = relMaxNeedsUpdate; i >= relMinNeedsUpdate; i--) {
            GridPos posToCheck = pos + GridPos.up * i;
            foreach (TriPos tri in posToCheck.Triangles) {
                GridPos[] horizCorners = tri.HorizCorners;
                bool childExists = renderGrid.TryGetValue(tri, out GridPiece child);
                if (!childExists) {
                    child = GameObject.Instantiate(prefab, tri.World,
                        tri.right ? Quaternion.identity : Quaternion.Euler(0, 180, 0), transform);
                    child.Pos = tri;
                    renderGrid[tri] = child;
                }
                child.Refresh();
            }
        }
    }

    public Transform GetPosParent(GridPos pos) {
        bool childExists = renderGrid.TryGetValue(pos.Triangles[0], out GridPiece parent);
        return childExists ? parent.transform : null;
    }

    public void SetPos(GridMod mod) {
        if (mod.open) SetPosNoDelay(mod);
        else {
            MakeDust(mod.pos, 0, mod.roof, false);
            this.Invoke(() => SetPosNoDelay(mod), undustPrefab.main.duration);
        }
    }

    private void SetPosNoDelay(GridMod mod) {
        GridPos pos = mod.pos;
        int roof = mod.roof;
        bool value = mod.open;

        int relMinUpdated = 0;
        int relMaxUpdated = roof - 1;

        ForceSetPos(pos, relMinUpdated, relMaxUpdated, value);
        if (value) MakeDust(pos, relMinUpdated, relMaxUpdated, true);
    }

    private void ForceSetPos(GridPos pos, int relMinUpdated, int relMaxUpdated, bool open) {
        if (!open) Biome.Next(pos, overrideOld: false);
        else for (int i = -1; i < 2; i++) foreach (GridPos adj in GridPos.ListAllWithMagnitude(1)) {
            Biome.Next(pos + adj + GridPos.up * i, overrideOld: false);
        }
        for (int i = relMinUpdated; i <= relMaxUpdated; i++)
            grid[pos + GridPos.up * i] = open;
        UpdatePos(pos, relMinUpdated - 1, relMaxUpdated + 1);
    }

    private void MakeDust(GridPos pos, int relMinUpdated, int relMaxUpdated, bool open) {
        Transform dust;
        if (open) dust = GameObject.Instantiate(dustPrefab).transform;
        else dust = GameObject.Instantiate(undustPrefab).transform;
        dust.position = pos.World + Vector3.up * MaterialGrid.Scale.y * relMinUpdated;
        dust.localScale = Vector3.Scale(MaterialGrid.Scale, new Vector3(1, relMaxUpdated - relMinUpdated, 1));
    }
}
