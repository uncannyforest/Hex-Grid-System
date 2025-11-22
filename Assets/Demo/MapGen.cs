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
    private float width = 1;
    private int height = 1;

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
            NextStep();

            if (--countdownPlaceLight <= 0) {
                countdownPlaceLight = placeLightEvery;
                GameObject.Instantiate(lightToPlace, transform.position, Quaternion.identity, lightParent);
            }
        }
    }

    private void NextStep() {
        prevLoc = nextLoc;
        float deltaWidth = Randoms.CoinFlip ? 0 : 1 / width - Random.value;
        int deltaHeight = Randoms.CoinFlip ? 0 : Random.value < 1f / height ? 1 : -1;
        width += deltaWidth;
        height += deltaHeight;
        // Debug.Log("height: " + height + " width: " + width );

        if (deltaHeight == 0) nextLoc = nextLoc.RandomDeviation(elevationChangeRate);
        else {
            nextLoc = nextLoc.RandomDeviation(0);
            if (Randoms.CoinFlip) nextLoc -= GridPos.up * deltaHeight;
        }

        Block materialType = wasAboveGround ? Block.DIRT : Block.AIR;

        for (int i = 0; i < width; i++)
            foreach (GridPos rel in GridPos.ListAllWithMagnitude(i)) 
                if (i < width - 1 || Random.value < width - i) {
            GridMod mod = new GridMod(nextLoc + rel, materialType, height);
            mod.Commit();
        }

        wasAboveGround = nextLoc.w + height >= 1;
    }
}
