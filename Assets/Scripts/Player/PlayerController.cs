using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] float playerSpeed;
    [Range(10, 50)][SerializeField] float gravityValue;
    [SerializeField] float pushBackResolve;

    private Vector3 move;
    private Vector3 playerVelocity;
    public Vector3 pushBack;

    private Vector2 movementInput = Vector2.zero;


    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }
    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        move = (transform.right * movementInput.x) + (transform.forward * movementInput.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move((playerVelocity + pushBack) * Time.deltaTime);

        pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushBackResolve);
    }
}
