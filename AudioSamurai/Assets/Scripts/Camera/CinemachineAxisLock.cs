using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's Z co-ordinate
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CinemachineAxisLock : CinemachineExtension
{

    [Tooltip("Lock the camera's X position to this value")]
    public float XPosition = 1;

    [Tooltip("Lock the camera's Y position to this value")]
    public float YPosition = 1;

    [Tooltip("Lock the camera's Z position to this value")]
    public float ZPosition = 1;

    public bool LockOnXAxis = false;
    public bool LockOnYAxis = false;
    public bool LockOnZAxis = false;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        if (stage == CinemachineCore.Stage.Body) {
            var pos = state.RawPosition;
            if (LockOnXAxis)
                pos.x = XPosition;
            if (LockOnYAxis)
                pos.y = YPosition;
            if (LockOnZAxis)
                pos.z = ZPosition;
            state.RawPosition = pos;
        }
    }
}