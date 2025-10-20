using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPiece : MonoBehaviour {
    public TriPos pos;
    public List<GridMaterialPiece> materials = new List<GridMaterialPiece>();

    public TriPos Pos {
        set {
            pos = value;
            gameObject.name = pos.ToString();
        }
    }

    public void Start() {
        foreach (Block block in Enum.GetValues(typeof(Block))) {
            if (block == Block.UNSPECIFIED || block == Block.AIR) continue;
            materials.Add(new GridMaterialPiece(block, this));
        }
        Refresh();
    }

    public void Refresh() {
        for (int i = transform.childCount - 1; i >= 0; i--)
            GameObject.Destroy(transform.GetChild(i).gameObject);

        foreach (GridMaterialPiece material in materials)
            material.Render();
    }
}
