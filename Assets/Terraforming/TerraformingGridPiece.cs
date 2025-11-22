using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerraformingGridPiece : MonoBehaviour {
    public GridPos pos;

    public GridPos Pos {
        set {
            pos = value;
            gameObject.name = pos.ToString();
        }
    }

    void Start() {
        Refresh();
    }

    public void Refresh() {
        for (int i = transform.childCount - 1; i >= 0; i--)
            GameObject.Destroy(transform.GetChild(i).gameObject);

        bool solid = IsSolid(pos);
        if (IsSolid(pos + GridPos.up) != solid) Create(TerraformingGrid.I.horiz, 90);
        if (IsSolid(pos + GridPos.Q) != solid) Create(TerraformingGrid.I.vert, 30);
        if (IsSolid(pos + GridPos.W) != solid) Create(TerraformingGrid.I.vert, 90);
        if (IsSolid(pos + GridPos.E) != solid) Create(TerraformingGrid.I.vert, 150);
    }

    private bool IsSolid(GridPos pos) {
        return WorldGrid.Grid[pos] != Block.AIR;
    }

    private Transform Create(GameObject prefab, float yRot) {
        Transform newPiece = GameObject.Instantiate(prefab, transform).transform;
        newPiece.localRotation = Quaternion.Euler(0, yRot, 0);
        newPiece.localScale = Vector3.Scale(new Vector3(1, 0.5f, 1), WorldGrid.I.scale);
        return newPiece;
    }

}
