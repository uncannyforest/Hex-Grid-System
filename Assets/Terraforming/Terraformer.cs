using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraformer : MonoBehaviour {
    public float range = 6;
    public GameObject verticalHighlight;
    public GameObject horizontalHighlight;
    public float highlightOffset = 1/6f;

    private Collider currentCollider;
    private GameObject currentHighlight;
    private GridPos currentPos;

    private Block material = Block.AIR;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            new GridMod(currentPos, material).Commit();
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, range, LayerMask.GetMask("Terraforming"))) {
            if (hit.collider == currentCollider) return;
            currentCollider = hit.collider;
            if (currentHighlight != null) Destroy(currentHighlight);
            Vector3 offset = currentCollider.transform.position + highlightOffset * hit.normal;
            GameObject prefab = hit.normal == Vector3.up || hit.normal == Vector3.down ?
                horizontalHighlight : verticalHighlight;
            currentHighlight = Instantiate(prefab, offset, currentCollider.transform.rotation, currentCollider.transform);
            Vector3 worldPos = material == Block.AIR
                ? currentCollider.bounds.center - highlightOffset * hit.normal
                : currentCollider.bounds.center + highlightOffset * hit.normal;
            currentPos = GridPos.FromWorld(worldPos);
        }

        if (Input.GetKeyDown("0")) material = Block.AIR;
        if (Input.GetKeyDown("1")) material = Block.DIRT;
    }
}
