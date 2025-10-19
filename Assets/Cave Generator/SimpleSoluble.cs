using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSoluble : MonoBehaviour {
    public List<GridPos> positions;

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            foreach (GridPos pos in positions) {
                MaterialGrid.I.SetPos(GridMod.Cave(pos));
            }
        }
    }
}
