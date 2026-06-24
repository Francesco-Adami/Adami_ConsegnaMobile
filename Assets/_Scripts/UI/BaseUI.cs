using System.Collections;
using UnityEngine;

public interface IGameUI
{
    public GameUI GetUIType();
}

public class BaseUI : MonoBehaviour, IGameUI
{
    [Header("BaseUI")]
    [SerializeField] private GameUI gameUI;

    public GameUI GetUIType()
    {
        return gameUI;
    }
}