using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPiece : MonoBehaviour {
    public static int ALL_OPEN = 511; // 2 ^ 9 - 1

    public TriPos pos;
    public int data;

    public TriPos Pos {
        set {
            pos = value;
            gameObject.name = pos.ToString();
        }
    }

    // public void Refresh() {
    //     foreach (Transform child in transform) {
    //         GameObject.Destroy(child.gameObject);
    //     }
    //     allData.Clear();
    //     IEnumerable<int> levels =
    //         from hex in pos.Corners
    //         where CaveGrid.I.inside[hex]
    //         select CaveGrid.I.elevation[hex];
    //     if (levels.Count() == 0) return;
    //     int min = levels.Min();
    //     int max = levels.Max();

    //     IEnumerable<int> lakeHeights =
    //         from hex in pos.Corners
    //         where CaveGrid.I.stats.GetLakeHeight(hex) != null
    //         select (int)CaveGrid.I.stats.GetLakeHeight(hex);
    //     if (lakeHeights.Count() > 0) {
    //         int lakeMax = lakeHeights.Max();
    //         if (max < lakeMax) max = lakeMax;
    //     }

    //     for (int i = min; i <= max + 1; i++) {
    //         FillLevel(i);
    //     }
    // }

    public void Set() => Refresh();

    public void Refresh() {
        for (int i = transform.childCount - 1; i >= 0; i--)
            GameObject.Destroy(transform.GetChild(i).gameObject);
        // Sample current level, below, and above
        int level = pos.w;
        Grid<bool> grid = CaveGrid.I.grid;
        GridPos[] hc = pos.HorizCorners;
        int[] data = new int[3];
        for (int i = 0; i < 3; i++) {
            int value = 0;
            if (!grid[hc[i] - GridPos.up]) value += 1;
            if (!grid[hc[i]]) value += 2;
            if (!grid[hc[i] + GridPos.up]) value += 4;
            data[i] = value;
        }
        this.data = data[0] * 100 + data[1] * 10 + data[2];

        // Open Rule 
        if ((data[0] & 2) + (data[1] & 2) + (data[2] & 2) == 0) return;
        // Floor/Ground Rule
        if ((data[0] & 2) * (data[1] & 2) * (data[2] & 2) == 8) {
            if ((data[0] & 4) + (data[1] & 4) + (data[2] & 4) == 0) {
                Create(CaveGrid.I.smooth.floor, level, Random.Range(0, 3), Randoms.CoinFlip, false, Four(3, 4, 5));
            }
            if ((data[0] & 1) + (data[1] & 1) + (data[2] & 1) == 0) {
                Create(CaveGrid.I.smooth.floor, level, Random.Range(0, 3), Randoms.CoinFlip, true, Four(3, 4, 5));
            }
            return;
        }

        // Slope Rule
        if(FindSlope(data,
                out int lowerBase1, out int lowerBase2, out int upperBase1, out int upperBase2,
                out int top1, out int top2, out int extraCornerValue)) {
            if (extraCornerValue == -1) { // Something is symmetric
                if (top2 != -1) {
                    if (lowerBase1 != -1) {
                        Create(CaveGrid.I.smooth.upperSlope, level, lowerBase1, Randoms.CoinFlip, false, Four(3, 1, 5));
                    } else {
                        Create(CaveGrid.I.smooth.upperSlope, level, upperBase1, Randoms.CoinFlip, true, Four(3, 1, 5));
                    }
                } else if (lowerBase1 != -1 && upperBase1 != -1) {
                    bool reflect = upperBase1 == NextCorner(top1);
                    bool rotateOnZ = Randoms.CoinFlip;
                    Create(CaveGrid.I.smooth.tunnelSlopeDouble, level, top1, reflect ^ rotateOnZ, rotateOnZ, Four(6, 4, 2));
                } else {
                    Create(CaveGrid.I.smooth.lowerSlope, level, top1, Randoms.CoinFlip, upperBase2 != -1, Four(0, 4, 2));
                }
            } else {
                bool yFlip = upperBase1 != -1;
                int base1 = yFlip ? upperBase1 : lowerBase1;
                bool baseIsAfterTop = base1 == NextCorner(top1);
                if (yFlip) extraCornerValue = FlipDataY(extraCornerValue);
                bool topIsThin = data[top1] == 2; // otherwise it's 3
                // extraCornerValue cannot be 1, 2, or 3, because those are base/top values
                if (extraCornerValue == 0) {
                    Create(topIsThin ? CaveGrid.I.smooth.tunnelSlopeLedge : CaveGrid.I.smooth.upperCurve,
                        level, base1, !baseIsAfterTop, yFlip, Four(3, 1, 0));
                } else if (extraCornerValue == 4) {
                    Create(CaveGrid.I.smooth.tunnelCurve, level, base1, !baseIsAfterTop, yFlip, Four(3, 1, 8, 0));
                } else if (extraCornerValue == 5) {
                    Create(CaveGrid.I.smooth.tunnelSlope, level, base1, !baseIsAfterTop, yFlip, Four(3, 1, 8, 2));
                } else { // extraCornerValue == 6 || extraCornerValue == 7
                    Create(CaveGrid.I.smooth.lowerCurve, level, top1, baseIsAfterTop, yFlip, Four(0, 4, 5, 8));
                }
            }
            return;
        }

        // Halves Rule

        int[] dataAbove = (from d in data select d >> 1).ToArray();
        int[] dataBelow = (from d in data select ((d & 2) >> 1) | ((d & 1) << 1)).ToArray();

        RenderHalf(level, dataAbove, false);
        RenderHalf(level, dataBelow, true);
    }

    // each data value is truncated
    // now data[i], each in range [0, 3], represents {open, floor, ceiling, wall}
    // by now, we've ruled out some combinations by Open Rule (all 0 || 2) & Ground/Floor Rule (all 1 || 3)
    private void RenderHalf(int level, int[] data, bool yFlip) {
        int otherLoc = -1;
        int otherValue = -1;
        if (HasTwo(data, 0, ref otherLoc, ref otherValue)) {
            if (otherValue == 1) {
                Create(CaveGrid.I.smooth.revcornerGutter, level, otherLoc, Randoms.CoinFlip, yFlip, Four(4));
            } else { // if (otherValue == 3)
                Create(CaveGrid.I.smooth.revcorner, level, otherLoc, Randoms.CoinFlip, yFlip, Four(4, 7));
            }
        } else if (HasTwo(data, 1, ref otherLoc, ref otherValue)) {
            if (otherValue == 0) {
                Create(CaveGrid.I.smooth.cornerGutter, level, otherLoc, Randoms.CoinFlip, yFlip, Four(5, 3));
            } else { // if (otherValue == 2)
                Create(CaveGrid.I.smooth.cornerMoulding, level, otherLoc, Randoms.CoinFlip, yFlip, Four(3, 7, 5));
            }
        } else if (HasTwo(data, 2, ref otherLoc, ref otherValue)) {
            Create(CaveGrid.I.smooth.revcornerMoulding, level, otherLoc, Randoms.CoinFlip, yFlip, Four(6, 4, 8));
        } else if (HasTwo(data, 3, ref otherLoc, ref otherValue)) {
            if (otherValue == 0) {
                Create(CaveGrid.I.smooth.corner, level, otherLoc, Randoms.CoinFlip, yFlip, Four(5, 3, 8, 6));
            } else { // if (otherValue == 2)
                Create(CaveGrid.I.smooth.cornerMoulding, level, otherLoc, Randoms.CoinFlip, yFlip, Four(3, 7, 5));
            }
        } else { // data is 3 different numbers in range [0, 3]
            int sum = data[0] + data[1] + data[2]; // sum is in [3, 6]
            int pivotValue = sum == 6 ? 2 : sum - 3; // where the triangle gets split down the middle: 0, 1, 2, 2
            int pivot = data[0] == pivotValue ? 0 : data[1] == pivotValue ? 1 : 2;
            int xValue = (sum - 1) % 4; // the values at x = -1 in the .blend: 2, 3, 0, ?
            bool xFlip = sum != 6 && data[(pivot + 2) % 3] == xValue; // found it at x = 1
            if (sum == 3) {
                Create(CaveGrid.I.smooth.tunnelPillarSlant, level, pivot, xFlip, yFlip, Four(8, 3));
            } else if (sum == 4) {
                Create(CaveGrid.I.smooth.endGutter, level, pivot, xFlip, yFlip, Four(5, 4, 8));
            } else if (sum == 5) {
                Create(CaveGrid.I.smooth.endMoulding, level, pivot, xFlip, yFlip, Four(3, 7, 6));
            } else {
                Create(CaveGrid.I.smooth.cornerMoulding, level, pivot, Randoms.CoinFlip, yFlip, Four(3, 7, 5));
            }
        }
    }

    private int NextCorner(int corner) => (corner + 1) % 3;
    private int ThirdCorner(int corner1, int corner2) => 3 - corner1 - corner2;
    private int FlipDataY(int data) => (data & 1) << 2 | (data & 2) | (data & 4) >> 2;

    public bool HasTwo(int[] data, int what, ref int otherLoc, ref int otherValue) {
        bool result = data.Where(d => d == what).Count() == 2;
        if (!result) return false;
        otherLoc = data[0] != what ? 0 : data[1] != what ? 1 : 2;
        otherValue = data[otherLoc];
        return true;
    }

    private bool FindSlope(int[] data,
            out int lowerBase1, out int lowerBase2, out int upperBase1, out int upperBase2,
            out int top1, out int top2, out int extraCornerValue) {
        lowerBase1 = -1;
        lowerBase2 = -1;
        upperBase1 = -1;
        upperBase2 = -1;
        top1 = -1;
        top2 = -1;
        extraCornerValue = -1;
        // find bases
        for (int i = 0; i < 3; i++) {
            if (data[i] == 1) {
                if (lowerBase1 == -1) lowerBase1 = i;
                else lowerBase2 = i; // we already ruled out all 3 corners equalling 1 or 4
            } else if (data[i] == 4) {
                if (upperBase1 == -1) upperBase1 = i;
                else upperBase2 = i;
            }
        }
        // find tops
        if (lowerBase1 != -1 && upperBase1 != -1) {
            int thirdCorner = ThirdCorner(lowerBase1, upperBase1);
            int value = data[thirdCorner];
            if (value == 2) {
                top1 = thirdCorner;
                return true;
            } else if (value == 3) {
                top1 = thirdCorner;
                upperBase1 = -1;
                extraCornerValue = 4; // it was upperBase1
                return true;
            } else if (value == 6) {
                top1 = thirdCorner;
                lowerBase1 = -1;
                extraCornerValue = 1; // it was lowerBase1
                return true;
            } else {
                return false;
            }
        }
        if (lowerBase1 != -1) {
            for (int i = 1; i < 3; i++) {
                int corner = (lowerBase1 + i) % 3;
                if (data[corner] == 2 || data[corner] == 3) {
                    if (top1 == -1) top1 = corner;
                    else top2 = corner;
                }
            }
            if (lowerBase2 == -1 && top1 != -1 && top2 == -1) {
                extraCornerValue = data[ThirdCorner(lowerBase1, top1)];
            }
            return top1 != -1;
        }
        if (upperBase1 != -1) {
            for (int i = 1; i < 3; i++) {
                int corner = (upperBase1 + i) % 3;
                if (data[corner] == 2 || data[corner] == 6) {
                    if (top1 == -1) top1 = corner;
                    else top2 = corner;
                }
            }
            if (upperBase2 == -1 && top1 != -1 && top2 == -1) {
                extraCornerValue = data[ThirdCorner(upperBase1, top1)];
            }
            return top1 != -1;
        }
        // else lowerBase1 == upperBase1 == -1
        return false;
    }

    // origSources is int[4] of {R,G,B,W} vertex color locations in MODEL,
    // where each is in range [0, 8] or is -1 for unused (see int[] map below):
    // COLOR[4] -> MODEL[9]
    public Transform Create(GameObject prefab, int y, int pivot, bool xFlip, bool yFlip, int[] origSources) {
        int yRot = 30 - pivot * 120;
        int left = (pivot + (xFlip?1:2)) % 3; // location of x == 1 (.blend) in WORLD SPACE
        int right = (pivot + (xFlip?2:1)) % 3; // location of x == -1 (.blend) in WORLD SPACE
        int above = yFlip ? 0 : 6; // location of z == 1 (.blend) in WORLD SPACE
        int below = yFlip ? 6 : 0; // location of z == -1 (.blend) in WORLD SPACE
        // list of MODEL locations transformed to WORLD SPACE: MODEL[9] -> WORLD[9]
        int[] map = new int[] {-1, left + below, pivot + below, right + below,
                                   left + 3,     pivot + 3,     right + 3,
                                   left + above, pivot + above, right + above}; 
        // list of color locations in WORLD SPACE: COLOR[4] -> WORLD[9]
        int[] sources = new int[] {map[origSources[0] + 1], map[origSources[1] + 1], map[origSources[2] + 1], map[origSources[3] + 1]};
        Transform newPiece = GameObject.Instantiate(prefab, transform).transform;
        newPiece.localRotation = Quaternion.Euler(0, yRot, 0);
        newPiece.localScale = Vector3.Scale(new Vector3(xFlip ? -1 : 1, yFlip ? -0.5f : 0.5f, 1), CaveGrid.I.scale);
        SetMaterial(newPiece, sources);
        return newPiece;
    }

    private int[] Four(params int[] values) {
        int[] result = new int[4];
        int i = 0;
        for (; i < values.Length; i++) result[i] = values[i];
        for (; i < 3; i++) result[i] = -1;
        return result;
    }

    private bool SetSoftMaterial(Transform newPiece) {
        List<GridPos> softPos = new List<GridPos>();
        foreach (GridPos gridPos in pos.HorizCorners) {
            if (CaveGrid.I.soft[gridPos]) softPos.Add(gridPos);
            if (CaveGrid.I.soft[gridPos - GridPos.up]) softPos.Add(gridPos - GridPos.up);
        }
        if (softPos.Count >= 1) {
            foreach (MeshRenderer renderer in newPiece.GetComponentsInChildren<MeshRenderer>())
                renderer.material = CaveGrid.I.softMaterial;
            // Debug.Log("Set children to soft in " + gameObject);
            MeshCollider[] childColliders = GetComponentsInChildren<MeshCollider>();
            foreach (MeshCollider childCollider in childColliders) {
                // Debug.Log("Collider " + childCollider);
                if (childCollider.GetComponent<SimpleSoluble>() != null) {
                    SimpleSoluble foundSs = childCollider.GetComponent<SimpleSoluble>();
                    foundSs.positions = softPos;
                }
                SimpleSoluble ss = childCollider.gameObject.AddComponent<SimpleSoluble>();
                ss.positions = softPos;
            }
        }
        return softPos.Count >= 1;
    }

    private void SetMaterial(Transform newPiece, int[] src) {
        if (SetSoftMaterial(newPiece)) return;
        // Better not to make a new material — this is the current bottleneck.
        // 0.3 ms * 18 TriPoses * 2 Sets * 21 largest diamter = 200ms
        Material material = new Material(CaveGrid.I.defaultMaterial);
        material.SetColor("_Color1", GetFloorColor(src[0]));
        material.SetColor("_Color2", GetFloorColor(src[1]));
        material.SetColor("_Color3", GetFloorColor(src[2]));
        material.SetColor("_Color4", GetFloorColor(src[3]));
        material.SetColor("_Walls1", GetWallColor(src[0]));
        material.SetColor("_Walls2", GetWallColor(src[1]));
        material.SetColor("_Walls3", GetWallColor(src[2]));
        material.SetColor("_Walls4", GetWallColor(src[3]));
        foreach (MeshRenderer renderer in newPiece.GetComponentsInChildren<MeshRenderer>()) renderer.material = material;
    }

    // for sourceId [-1, 8] value origins see int[] map in GridPiece.Create()
    private Color GetFloorColor(int sourceId) {
        if (sourceId == -1) return CaveGrid.Biome.DefaultFloor;
        int corner = sourceId % 3;
        int yOffset = sourceId / 3 - 1;
        return CaveGrid.Biome.GetFloor(pos.HorizCorners[corner] + GridPos.up * yOffset);
    }
    private Color GetWallColor(int sourceId) {
        if (sourceId == -1) return CaveGrid.Biome.DefaultWall;
        int corner = sourceId % 3;
        int yOffset = sourceId / 3 - 1;
        return CaveGrid.Biome.GetWall(pos.HorizCorners[corner] + GridPos.up * yOffset);
    }
}
