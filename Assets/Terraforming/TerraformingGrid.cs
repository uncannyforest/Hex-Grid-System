using System.Collections.Generic;
using UnityEngine;

public class TerraformingGrid : MonoBehaviour {
    private static TerraformingGrid instance;
    public static TerraformingGrid I { get => instance; }
    TerraformingGrid(): base() {
        instance = this;
    }
    public TerraformingGridPiece prefab;
    public GameObject horiz;
    public GameObject vert;

    private Dictionary<GridPos, TerraformingGridPiece> grid = new Dictionary<GridPos, TerraformingGridPiece>();

    void Awake() {
        WorldGrid.I.PosUpdated += OnPosUpdated;
    }

    private void OnPosUpdated(GridPos pos, int height) {
        for (int i = -1; i < height; i++) {
            GridPos relPos = pos + GridPos.up * i;
            UpdateGridPiece(relPos);
            UpdateGridPiece(relPos + GridPos.A);
            UpdateGridPiece(relPos + GridPos.S);
            UpdateGridPiece(relPos + GridPos.D);
        }
    }

    private void UpdateGridPiece(GridPos pos) {
        bool childExists = grid.TryGetValue(pos, out TerraformingGridPiece child);
        if (!childExists) {
            child = GameObject.Instantiate(prefab, pos.World, Quaternion.identity, transform);
            child.Pos = pos;
            grid[pos] = child;
        } else child.Refresh();
    }
}
