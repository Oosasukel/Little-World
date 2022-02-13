using UnityEngine;

[CreateAssetMenu(fileName = "New World Config", menuName = "ScriptableObjects/WorldConfig", order = 0)]
public class WorldConfigSO : ScriptableObject
{
    [SerializeField]
    private bool[] activePolygons;

    public void SetPolygonsLength(int length)
    {
        activePolygons = new bool[length];
    }

    public void SetPolygonActive(int index, bool active)
    {
        if (activePolygons != null) activePolygons[index] = active;
    }

    public bool PolygonIsActive(int index)
    {
        return activePolygons[index];
    }

    public void OnValidate()
    {
        // Debug.Log(Time.time);
    }
}