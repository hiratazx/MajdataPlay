using MajdataPlay.Settings;
using MajdataPlay.Utils;
using UnityEngine;
using UnityEngine.UI;
#nullable enable
namespace MajdataPlay.Scenes.Game
{
    /// <summary>
    /// Manages auto-rotation for gameplay scenes on mobile.
    /// When the device is in landscape orientation, hides non-gameplay UI
    /// and centers/scales the gameplay circle to fill the screen height,
    /// similar to AstroDX's landscape mode.
    /// </summary>
    [DefaultExecutionOrder(200)]
    public class LandscapeGameplayManager : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_IOS
        bool _isLandscape = false;
        bool _isInitialized = false;
        bool _isEnabled = true;

        // References set by Init()
        Camera _mainCamera;
        RectTransform _mainDisplayer;

        // Saved state for restoration
        Rect _originalCameraRect;
        float _originalOrthographicSize;
        Vector3 _originalCameraPosition;

        // Canvas UI elements to hide in landscape
        GameObject? _subCover;
        GameObject? _subCoverBottom;
        GameObject? _subDisplay;
        GameObject? _bgInfoHeader;
        GameObject? _chartAnalyzerObj;
        CanvasScaler? _canvasScaler;

        // ScreenPos movers to disable in landscape
        ScreenPosCameraMover? _cameraMover;
        ScreenPosCanvasMover? _canvasMover;

        Vector2 _originalCanvasScalerRef;
        float _originalMatchWidthOrHeight;
        bool _subCoverWasActive;
        bool _subCoverBottomWasActive;
        bool _subDisplayWasActive;
        bool _bgInfoHeaderWasActive;
        bool _chartAnalyzerWasActive;

        public void Init(Camera mainCamera, RectTransform mainDisplayer)
        {
            _mainCamera = mainCamera;
            _mainDisplayer = mainDisplayer;

            // Check setting
            var displayOptions = MajInstances.Settings?.Display;
            _isEnabled = displayOptions?.AutoRotateLandscape ?? true;

            if (!_isEnabled)
            {
                // Lock to portrait if disabled
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                return;
            }

            // Allow landscape rotation during gameplay
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;

            // Save original state
            _originalCameraRect = _mainCamera.rect;
            _originalOrthographicSize = _mainCamera.orthographicSize;
            _originalCameraPosition = _mainCamera.transform.position;

            // Find UI elements to hide in landscape
            var canvasRoot = _mainDisplayer.parent;
            if (canvasRoot != null)
            {
                var subCoverTransform = canvasRoot.Find("Sub_Cover");
                if (subCoverTransform != null)
                    _subCover = subCoverTransform.gameObject;

                var subCoverBottomTransform = canvasRoot.Find("Sub_Cover_Bottom");
                if (subCoverBottomTransform != null)
                    _subCoverBottom = subCoverBottomTransform.gameObject;

                var subDisplayTransform = canvasRoot.Find("Sub_Display");
                if (subDisplayTransform != null)
                    _subDisplay = subDisplayTransform.gameObject;
            }

            // Find the BG info header
            var bgInfoHeader = GameObject.Find("BGInfoHeader");
            if (bgInfoHeader != null)
                _bgInfoHeader = bgInfoHeader;

            var chartAnalyzer = GameObject.Find("ChartAnalyzer");
            if (chartAnalyzer != null)
                _chartAnalyzerObj = chartAnalyzer;

            // Find ScreenPos movers to disable in landscape
            _cameraMover = _mainCamera.GetComponent<ScreenPosCameraMover>();
            _canvasMover = FindObjectOfType<ScreenPosCanvasMover>();

            // Find canvas scaler
            var rootCanvas = _mainDisplayer.GetComponentInParent<Canvas>()?.rootCanvas;
            if (rootCanvas != null)
            {
                _canvasScaler = rootCanvas.GetComponent<CanvasScaler>();
                if (_canvasScaler != null)
                {
                    _originalCanvasScalerRef = _canvasScaler.referenceResolution;
                    _originalMatchWidthOrHeight = _canvasScaler.matchWidthOrHeight;
                }
            }

            _isInitialized = true;

            // Check initial orientation
            CheckOrientation();
        }

        void Update()
        {
            if (!_isInitialized || !_isEnabled)
                return;

            CheckOrientation();
        }

        void CheckOrientation()
        {
            bool isNowLandscape = Screen.width > Screen.height;

            if (isNowLandscape && !_isLandscape)
            {
                EnterLandscape();
            }
            else if (!isNowLandscape && _isLandscape)
            {
                ExitLandscape();
            }
        }

        void EnterLandscape()
        {
            _isLandscape = true;
            MajDebug.LogDebug("[LandscapeGameplayManager] Entering landscape mode");

            // Save current active states before hiding
            _subCoverWasActive = _subCover != null && _subCover.activeSelf;
            _subCoverBottomWasActive = _subCoverBottom != null && _subCoverBottom.activeSelf;
            _subDisplayWasActive = _subDisplay != null && _subDisplay.activeSelf;
            _bgInfoHeaderWasActive = _bgInfoHeader != null && _bgInfoHeader.activeSelf;
            _chartAnalyzerWasActive = _chartAnalyzerObj != null && _chartAnalyzerObj.activeSelf;

            // Hide non-gameplay UI
            if (_subCover != null) _subCover.SetActive(false);
            if (_subCoverBottom != null) _subCoverBottom.SetActive(false);
            if (_subDisplay != null) _subDisplay.SetActive(false);
            if (_bgInfoHeader != null) _bgInfoHeader.SetActive(false);
            if (_chartAnalyzerObj != null) _chartAnalyzerObj.SetActive(false);

            // Disable ScreenPos movers so they don't conflict with our layout
            if (_cameraMover != null) _cameraMover.enabled = false;
            if (_canvasMover != null) _canvasMover.enabled = false;

            // Reset camera rect to full screen
            _mainCamera.rect = new Rect(0, 0, 1, 1);

            // Adjust camera for landscape - show just the gameplay circle
            // The gameplay circle has a radius of ~5.4 world units
            // In landscape, we want the circle to fill the screen height
            if (_mainCamera.orthographic)
            {
                _mainCamera.orthographicSize = 5.4f;
            }

            // Center the camera on the gameplay area (circle center is at y=0 in world space)
            _mainCamera.transform.position = new Vector3(0, 0, -10);

            // Adjust canvas scaler for landscape orientation
            if (_canvasScaler != null)
            {
                // In landscape, swap the reference resolution so the canvas scales properly
                _canvasScaler.referenceResolution = new Vector2(1920, 1080);
                _canvasScaler.matchWidthOrHeight = 1f; // Match height to ensure circle fits
            }
        }

        void ExitLandscape()
        {
            _isLandscape = false;
            MajDebug.LogDebug("[LandscapeGameplayManager] Exiting landscape mode");

            // Restore camera settings
            _mainCamera.rect = _originalCameraRect;
            if (_mainCamera.orthographic)
            {
                _mainCamera.orthographicSize = _originalOrthographicSize;
            }
            _mainCamera.transform.position = _originalCameraPosition;

            // Restore canvas scaler
            if (_canvasScaler != null)
            {
                _canvasScaler.referenceResolution = _originalCanvasScalerRef;
                _canvasScaler.matchWidthOrHeight = _originalMatchWidthOrHeight;
            }

            // Re-enable ScreenPos movers so portrait layout works correctly
            if (_cameraMover != null) _cameraMover.enabled = true;
            if (_canvasMover != null) _canvasMover.enabled = true;

            // Restore UI elements to their previous active state
            if (_subCover != null) _subCover.SetActive(_subCoverWasActive);
            if (_subCoverBottom != null) _subCoverBottom.SetActive(_subCoverBottomWasActive);
            if (_subDisplay != null) _subDisplay.SetActive(_subDisplayWasActive);
            if (_bgInfoHeader != null) _bgInfoHeader.SetActive(_bgInfoHeaderWasActive);
            if (_chartAnalyzerObj != null) _chartAnalyzerObj.SetActive(_chartAnalyzerWasActive);
        }

        void OnDestroy()
        {
            // Restore to portrait-only when leaving gameplay
            if (_isLandscape)
            {
                ExitLandscape();
            }

            // Lock back to portrait for non-gameplay scenes
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
        }
#endif
    }
}
