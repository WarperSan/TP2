using ControllerModule.Controllers;
using UnityEngine;

public class PlayerController : Controller
{
    [Header("Movement")]
    [SerializeField]
    private PlayerMovement playerMovement;

    private Vector2 movementDirection;

    /// <inheritdoc/>
    protected override void OnStart() => ControllerManager.SwitchTo(this);

    /// <inheritdoc/>
    protected override void OnSwitchIn() => SetCursorLock(true);

    /// <inheritdoc/>
    public override void OnMove(Vector2 dir) => this.movementDirection = dir;

    /// <inheritdoc/>
    public override void OnJumpStart() => this.playerMovement.ProcessJumpStart();

    /// <inheritdoc/>
    public override void OnJumpEnd() => this.playerMovement.ProcessJumpRelease();

    /// <inheritdoc/>
    public override void OnSprintStart() => this.playerMovement.StartSprint();

    /// <inheritdoc/>
    public override void OnSprintEnd() => this.playerMovement.EndSprint();

    [Header("Interaction")]
    [SerializeField]
    private PlayerInteraction playerInteraction;

    /// <inheritdoc/>
    public override void OnFireStart() => this.playerInteraction.Interact();

    /// <inheritdoc/>
    protected override void OnUpdate(float elapsed)
    {
        this.playerMovement.ProcessMovement(this.movementDirection, elapsed);
        this.playerMovement.ProcessGravity(elapsed);
        this.playerMovement.ProcessJump();
        this.playerMovement.ProcessSprint();
        this.playerInteraction.UpdateCursor();
    }
}