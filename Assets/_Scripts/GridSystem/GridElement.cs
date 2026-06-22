using UnityEngine;

public class GridElement : MonoBehaviour, ISandwichComponent
{
    [SerializeField] private SandwichComponentType _componentType;
    [SerializeField] private Vector2Int _position;

    public void Init(SandwichComponentType type, Vector2Int position)
    {
        _componentType = type;
        _position = position;

        transform.position = new Vector3(
            position.x * 3,
            0,
            position.y * 3
        );
    }

    public Vector2Int GetGridPosition()
    {
        return _position;
    }

    public SandwichComponentType GetSandwitchType()
    {
        return _componentType;
    }

    public void SetNewGridPos(Vector2Int gridPos)
    {
        _position = gridPos;
    }
}
