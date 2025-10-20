public class SurfaceGrid {
    public static Block DEFAULT_GROUND = Block.DIRT;
    private Grid2D<int> surfaceElevation = new Grid2D<int>(); // value is lowest aboveGround value
    private Grid3D<Block> aboveGround = new Grid3D<Block>();
    private Grid3D<Block> belowGround = new Grid3D<Block>();

    public Block this[GridPos pos] {
        get {
            bool isAboveGround = pos.w >= surfaceElevation[pos.Horizontal];
            Grid3D<Block> grid = isAboveGround ? aboveGround : belowGround;
            Block defaultType = isAboveGround ? Block.AIR : DEFAULT_GROUND;
            Block result = grid[pos];
            return result == Block.UNSPECIFIED ? defaultType : result;
        }
        set {
            bool isAboveGround = pos.w >= surfaceElevation[pos.Horizontal];
            Grid3D<Block> grid = isAboveGround ? aboveGround : belowGround;
            grid[pos] = value;
        }
    }

    public void SetSurfaceElevation(GridPos firstAboveGround) {
        surfaceElevation[firstAboveGround.Horizontal] = firstAboveGround.w;
    }
    public GridPos GetSurfacePos(GridPos pos) {
        pos.w = surfaceElevation[pos.Horizontal];
        return pos;
    }
}

