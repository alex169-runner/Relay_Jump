using UnityEngine;

public class Action
{
    public int starId;
    public int planetId;

    public Vector2Int planetPosOld;
    public Vector2Int planetPosNew;

    public Action(int starId,
              int planetId,
              Vector2Int planetPosOld,
              Vector2Int planetPosNew)
    {
        this.starId = starId;
        this.planetId = planetId;
        this.planetPosOld = planetPosOld;
        this.planetPosNew = planetPosNew;
    }
}
