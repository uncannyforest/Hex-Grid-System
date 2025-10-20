using UnityEngine;

public class MapGen : MonoBehaviour {
    public float modRate = 1;
    public int placeLightEvery = 12;
    public int changeBiomeEvery = 12;
    public GameObject lightToPlace;
    public Transform lightParent;
    public float elevationChangeRate = .5f;

    private GridPos prevLoc;
    private GridPos nextLoc;
    private float progress = 0;
    private int countdownPlaceLight = 1;

    private bool wasAboveGround = false;

    void Start() {
        prevLoc = -GridPos.up;
        nextLoc = -GridPos.up;
        new GridMod(nextLoc, Block.AIR).Commit();
    }

    void Update() {
        transform.position = Vector3.Lerp(prevLoc.World, nextLoc.World, Maths.CubicInterpolate(Mathf.Clamp01(progress)));
        progress += Time.deltaTime / modRate;
        if (progress >= 1) {
            progress -= 1;
            prevLoc = nextLoc;
            nextLoc = nextLoc.RandomDeviation(elevationChangeRate);
            Block materialType = wasAboveGround ? Block.DIRT : Block.AIR;
            GridMod mod = nextLoc.w == prevLoc.w ? new GridMod(nextLoc, materialType)
                : nextLoc.w < prevLoc.w ? new GridMod(nextLoc, materialType, 2)
                : new GridMod(nextLoc - GridPos.up, materialType, 2);
            mod.Commit();
            wasAboveGround = nextLoc.w >= 0;
            
            if (--countdownPlaceLight <= 0) {
                countdownPlaceLight = placeLightEvery;
                GameObject.Instantiate(lightToPlace, transform.position, Quaternion.identity, lightParent);
            }
        }
    }
}
