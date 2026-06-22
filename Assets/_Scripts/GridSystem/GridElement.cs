using UnityEngine;

public class GridElement : MonoBehaviour, ISandwichComponent
{
    [SerializeField] private SandwichComponentType _componentType;
    [SerializeField] private Vector2 _position;

    public void Init(SandwichComponentType type, Vector2 position)
    {
        _componentType = type;
        _position = position;

        transform.position = new Vector3(
            position.x * 3,
            0,
            position.y * 3
        );
    }

    SandwichComponentType ISandwichComponent.GetType()
    {
        return _componentType;
    }
}
