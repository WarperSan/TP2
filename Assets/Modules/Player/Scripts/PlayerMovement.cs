using ControllerModule.Controllers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController _characterController;

    [SerializeField]
    private Controller controller;

    [SerializeField]
    private float walkSpeed = 8;

    private float currentSpeed = 8;

    #region Movement

    public void ProcessMovement(Vector3 direction, float elapsed)
    {
        // Check for validity
        if (this._characterController == null)
            return;

        if (controller.Eyes == null)
            return;

        // Modify the direction
        Vector3 movement = (controller.Eyes.forward * direction.y) + (controller.Eyes.right * direction.x);
        movement.y = 0;
        movement.Normalize();

        // If not moving, skip
        if (direction.magnitude <= 0f)
            return;

        this._characterController.Move(elapsed * this.currentSpeed * movement);
    }

    #endregion

    #region Gravity

    private Vector3 gravityVelocity;
    private bool isGrounded;

    public void ProcessGravity(float elapsed)
    {
        // Check for validity
        if (this._characterController == null)
            return;

        if (this.isGrounded && this.gravityVelocity.y <= 0)
        {
            this.gravityVelocity.y = 0;
            this.hasJumped = false;
        }

        this.gravityVelocity += Physics.gravity * elapsed;
        this._characterController.Move(this.gravityVelocity * elapsed);

        // Update state
        this.isGrounded = this._characterController.isGrounded;
    }

    #endregion

    #region Jump

    [Header("Jump")]
    [SerializeField, Min(0)]
    private float jumpForce;
    private bool hasJumped;
    private bool isPressingJump;

    public void ProcessJump()
    {
        if (this.isPressingJump)
        {
            this.ProcessJumpStart();
        }
    }

    public void ProcessJumpStart()
    {
        this.isPressingJump = true;

        // Check for validity
        if (!this.isGrounded)
            return;


        this.hasJumped = true;

        // Override gravity
        this.gravityVelocity.y = this.jumpForce;

    }

    public void ProcessJumpRelease()
    {
        this.isPressingJump = false;

        // Check for validity
        if (!this.hasJumped)
            return;

        if (this.gravityVelocity.y <= 0)
            return;

        // Release jump
        this.gravityVelocity.y = 0;
        this.ProcessGravity(0);
    }

    #endregion

    #region Sprint

    [Header("Sprint")]
    [SerializeField]
    private Image slider;

    [SerializeField]
    private float sprintSpeed = 15;

    [SerializeField]
    private float energy = 3;

    [SerializeField]
    private float maxEnergy = 3;

    private bool isSprinting = false;
    public void StartSprint()
    {
        this.isSprinting = true;
    }

    public void EndSprint()
    {
        this.isSprinting = false;
    }

    public void ProcessSprint()
    {
        if (isSprinting && energy > 0)
        {
            this.currentSpeed = this.sprintSpeed;
            energy -= Time.deltaTime;
        }
        else
        {
            this.currentSpeed = this.walkSpeed;
            if (!this.isSprinting)
                energy += Time.deltaTime;
        }
        energy = Mathf.Clamp(energy, 0, maxEnergy);
        slider.fillAmount = energy / maxEnergy;
    }
    #endregion
}