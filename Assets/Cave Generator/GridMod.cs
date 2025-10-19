
public struct GridMod {
    public GridPos pos;
    public int roof;
    public bool open;

    public GridMod(GridPos pos, int roof, bool open) {
        this.pos = pos;
        this.roof = roof;
        this.open = open;
    }

    public static GridMod Cave(GridPos pos, int roof = 1) => new GridMod(pos, roof, true);
    public static GridMod Wall(GridPos pos, int roof = 1) => new GridMod(pos, roof, false);

    public GridMod Inverted { get => new GridMod(pos, roof, !open); }
    public bool IsUnnecessary {
        get {
            bool result = true;
            for (int i = 0; i <= roof; i++)
                if (MaterialGrid.I.grid[pos + i * GridPos.up] != open)
                    result = false;
            return result;
        }
    }

    public bool Overlaps {
        get {
            bool result = false;
            for (int i = 0; i <= roof; i++)
                if (MaterialGrid.I.grid[pos + i * GridPos.up] == open)
                    result = true;
            return result;
        }
    }

    public override string ToString() {
        return pos + " | " + roof + (open ? " air" : " wall");
    }
}