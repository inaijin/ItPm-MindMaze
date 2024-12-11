using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AgentInput : MonoBehaviour, IAgentInput
{
    private Camera mainCamera;
    private bool fireButtonDown = false;

    [field: SerializeField]
    public UnityEvent<Vector2> OnMovementKeyPressed { get; set; }

    [field: SerializeField]
    public UnityEvent<Vector2> OnPointerPositionChange { get; set; }

    [field: SerializeField]
    public UnityEvent OnFireButtonPressed { get; set; }

    [field: SerializeField]
    public UnityEvent OnFireButtonReleased { get; set; }

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        GetMovementInput();
        GetPointerInput();
        GetFireInput();
        GetSpaceInput(); // New method to handle spacebar input
    }

    private void GetSpaceInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnFireButtonPressed?.Invoke(); // Trigger shooting event
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnFireButtonReleased?.Invoke(); // Stop shooting event
        }
    }

    private void GetFireInput()
    {
        if (Input.GetAxisRaw("Fire1") > 0)
        {
            if (fireButtonDown == false)
            {
                fireButtonDown = true;
                OnFireButtonPressed?.Invoke();
            }

        }
        else
        {
            if (fireButtonDown == true)
            {
                fireButtonDown = false;
                OnFireButtonReleased?.Invoke();
            }

        }
    }

    private void GetPointerInput()
    {
        var mouseInWorldSpace = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        OnPointerPositionChange?.Invoke(mouseInWorldSpace);
    }

    private void GetMovementInput()
    {
        OnMovementKeyPressed?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }
}
