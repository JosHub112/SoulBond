using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System;

public class Input : MonoBehaviour
{
    public static Vector2 Movement;

    private InputAction _moveAction;
    private PlayerInput _PlayerInput;

    private void Awake()
    {
        _PlayerInput = GetComponent<PlayerInput>();
        _moveAction = _PlayerInput.actions["Move"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
    }

}
