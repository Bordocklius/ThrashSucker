using UnityEngine;
using UnityEngine.UI;

namespace TrashSucker.Presenters
{
    /// <summary>
    /// Keeps a Canvas (or any GameObject) facing the camera in LateUpdate.
    /// Attach to a GameObject with a `Canvas` (world-space or screen-space camera) or to any UI root.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasFaceCameraPresenter : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField]
        private Camera _targetCamera;

        [Header("Behavior")]
        [SerializeField]
        private bool _smooth = true;
        [SerializeField]
        private float _smoothSpeed = 10f;
        [SerializeField]
        private bool _lockYAxis = true; // only rotate around Y (common for HUDs above objects)
        [SerializeField]
        private bool _reverse = false; // rotate 180° if your canvas faces away from camera

        private Canvas _canvas;

        void Start()
        {
            _canvas = GetComponent<Canvas>();

            if (_targetCamera == null)
                _targetCamera = Camera.main;
        }

        // Use LateUpdate so camera motion is already applied for this frame.
        void LateUpdate()
        {
            if (_targetCamera == null)
                return;

            // ScreenSpace-Overlay canvases do not need world rotation.
            if (_canvas != null && _canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return;

            Quaternion targetRotation;

            if (_lockYAxis)
            {
                // Keep object's Y level, look horizontally toward camera
                Vector3 lookPos = _targetCamera.transform.position;
                lookPos.y = transform.position.y;
                Vector3 direction = lookPos - transform.position;
                if (direction.sqrMagnitude <= Mathf.Epsilon)
                    return;
                targetRotation = Quaternion.LookRotation(direction);
            }
            else
            {
                Vector3 direction = _targetCamera.transform.position - transform.position;
                if (direction.sqrMagnitude <= Mathf.Epsilon)
                    return;
                targetRotation = Quaternion.LookRotation(direction);
            }

            if (_reverse)
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);

            if (_smooth)
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Clamp01(_smoothSpeed * Time.deltaTime));
            else
                transform.rotation = targetRotation;
        }
    }
}
