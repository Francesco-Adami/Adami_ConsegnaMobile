using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerTouch : MonoBehaviour
{
    // praticamente inutile
    [SerializeField] private float movementDeadzone = 0.2f;
    [SerializeField] private LayerMask gridElementLayer;

    private GridElement selectedGridElement = null;
    private Vector2 startPos;
    private Vector2 endPos;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];

            // Gestisco il tocco
            HasTouchedGridElement(touch);

            if (touch.phase == TouchPhase.Ended && selectedGridElement)
            {
                endPos = touch.screenPosition;
                //Debug.Log($"Touch ended. Start Position: {startPos}, End Position: {endPos}");

                GridManager.Instance.TryMove(selectedGridElement, GetDir());
            }
        }
        else if (selectedGridElement != null)
        {
            // resetto la selezione quando non sta più toccando lo schermo
            selectedGridElement = null;
        }
    }


    /// <summary>
    /// ritorna true se tocco un oggetto con il componente GridElement, altrimenti false
    /// </summary>
    private bool HasTouchedGridElement(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            // ray per vedere se ho toccato un oggetto con il componente GridElement
            Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gridElementLayer))
            {
                // se effettivamente l'oggetto ha il componente GridElement allora lo seleziono
                if (hit.transform.TryGetComponent(out GridElement gridElement))
                {
                    selectedGridElement = gridElement;
                    //Debug.Log($"Touched GridElement: {gridElement.name}");
                    startPos = touch.screenPosition;
                    return true;
                }
            }
        }

        return false;
    }

    private Direction GetDir()
    {
        Direction dir = Direction.None;

        Vector2 delta = endPos - startPos;

        if (delta.magnitude < movementDeadzone)
        {
            Debug.LogWarning("Movimento non abbastanza lungo");
            return dir;
        }

        // se |delta.x| è maggiore di |delta.y| allora il movimento è orizzontale, altrimenti verticale
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            // Movimento orizzontale
            if (delta.x > 0)
            {
                dir = Direction.Right;
            }
            else if (delta.x < 0)
            {
                dir = Direction.Left;
            }
        }
        else
        {
            // Movimento verticale
            if (delta.y > movementDeadzone)
            {
                dir = Direction.Up;
            }
            else if (delta.y < -movementDeadzone)
            {
                dir = Direction.Down;
            }
        }

        return dir;
    }
}
