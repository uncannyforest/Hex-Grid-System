
public struct GridMod {
    public GridPos pos;
    public int height;
    public Block materialType;

    public GridMod(GridPos pos, Block materialType, int roof = 1) {
        this.pos = pos;
        this.materialType = materialType;
        this.height = roof;
    }

    public void Commit() => WorldGrid.I.Set(this);

    public bool IsUnnecessary {
        get {
            bool result = true;
            for (int i = 0; i <= height; i++)
                if (WorldGrid.I.grid[pos + i * GridPos.up] != materialType)
                    result = false;
            return result;
        }
    }

    public bool Overlaps {
        get {
            bool result = false;
            for (int i = 0; i <= height; i++)
                if (WorldGrid.I.grid[pos + i * GridPos.up] == materialType)
                    result = true;
            return result;
        }
    }

    public override string ToString() {
        return pos + " | " + height + " | " + materialType;
    }
}