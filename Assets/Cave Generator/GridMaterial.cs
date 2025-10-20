using System.Collections.Generic;
using UnityEngine;

public struct MaterialConfig {
    public MaterialShape shape;
    public BlockFlags adjoiningTypes;
    public MaterialConfig(MaterialShape shape, BlockFlags adjoiningTypes) {
        this.shape = shape;
        this.adjoiningTypes = adjoiningTypes;
    }
    public bool ShapeAt(GridPos pos) => ((1 << (int)WorldGrid.Grid[pos]) & (int)adjoiningTypes) != 0;
}

// UNSPECIFIED is used in Grid3D as a placeholder for the default,
// which can be specified dynamically (as it is in SurfaceGrid)
public enum Block {
    UNSPECIFIED,
    AIR,
    DIRT,
}

public enum BlockFlags {
    AIR = 1 << 1,
    DIRT = 1 << 2,
}

public class GridMaterial : MonoBehaviour {
    private static GridMaterial instance;
    public static GridMaterial I { get => instance; }
    GridMaterial(): base() {
        instance = this;
    }
    public Dictionary<Block, MaterialConfig> config;
    public MaterialConfig this[Block materialType] {
        get => config[materialType];
    }

    public MaterialShape smooth;

    void Start() {
        config = new Dictionary<Block, MaterialConfig>() {
            [Block.DIRT] = new MaterialConfig(smooth, BlockFlags.DIRT)
        };
    }
}