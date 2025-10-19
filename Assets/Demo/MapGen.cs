using UnityEngine;

public class MapGen : MonoBehaviour {
    public float modRate = 1;
    public int placeLightEvery = 12;
    public int changeBiomeEvery = 12;
    public GameObject lightToPlace;
    public Transform lightParent;

    private GridPos prevLoc;
    private GridPos nextLoc;
    private float progress = 1;
    private int countdownPlaceLight = 1;
    private int countdownChangeBiome = 1;

    void Start() {
        prevLoc = GridPos.zero;
        nextLoc = GridPos.zero;
        CaveGrid.Biome.Next(GridPos.zero, 1);
        CaveGrid.I.SetPos(CaveGrid.Mod.Cave(nextLoc));
        countdownChangeBiome = changeBiomeEvery;
    }

    void Update() {
        transform.position = Vector3.Lerp(prevLoc.World, nextLoc.World, Maths.CubicInterpolate(Mathf.Clamp01(progress)));
        progress += Time.deltaTime / modRate;
        if (progress >= 1) {
            progress -= 1;
            prevLoc = nextLoc;
            nextLoc = nextLoc.RandomDeviation();
            CaveGrid.I.SetPos(CaveGrid.Mod.Cave(nextLoc));
            
            if (--countdownPlaceLight <= 0) {
                countdownPlaceLight = placeLightEvery;
                GameObject.Instantiate(lightToPlace, transform.position, Quaternion.identity, lightParent);
            }
            if (--countdownChangeBiome <= 0) {
                countdownChangeBiome = changeBiomeEvery;
                CaveGrid.Biome.Next(nextLoc, (CaveGrid.Biome.lastBiome + 1) % 13 + 1);
            }
        }
    }
}
