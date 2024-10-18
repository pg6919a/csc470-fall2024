using UnityEngine;

public class CellScript : MonoBehaviour
{
    [Header("Renderer")]
    public Renderer cubeRenderer;

    [Header("Cell State")]
    public bool alive = false;
    public int aliveCount = 0;

    [Header("Grid Position")]
    public int xIndex = -1;
    public int yIndex = -1;

    [Header("Colors")]
    public Color aliveColor = Color.green;
    public Color deadColor = Color.black;
    public Color initialColor = Color.red;

    void Start()
    {
        SetColor();
    }

    void OnMouseDown()
    {
        alive = !alive;
        SetColor();
    }

    public void SetColor()
    {
        if (alive)
        {
            float t = Mathf.Clamp01((float)aliveCount / 10f);
            Color dynamicColor = Color.Lerp(initialColor, aliveColor, t);
            cubeRenderer.material.color = dynamicColor;
        }
        else
        {
            cubeRenderer.material.color = deadColor;
        }
    }
}
