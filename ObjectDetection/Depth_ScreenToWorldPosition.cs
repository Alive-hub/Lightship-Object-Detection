using Niantic.Lightship.AR.Utilities;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Depth_ScreenToWorldPosition : MonoBehaviour
{
    [SerializeField]
    private AROcclusionManager _occlusionManager;
    
    [SerializeField]
    private ARCameraManager _arCameraManager;
    
    [SerializeField]
    private Camera _camera;
    
    private Matrix4x4 _displayMatrix;
    private XRCpuImage? _depthImage;
    
    private void OnEnable()
    {
        _arCameraManager.frameReceived += OnCameraFrameEventReceived;
    }
    
    private void OnDisable()
    {
        _arCameraManager.frameReceived -= OnCameraFrameEventReceived;
    }
    
    private void OnCameraFrameEventReceived(ARCameraFrameEventArgs args)
    {
        // Cache the screen to image transform
        if (args.displayMatrix.HasValue)
        {
#if UNITY_IOS
            _displayMatrix = args.displayMatrix.Value.transpose;
#else
            _displayMatrix = args.displayMatrix.Value;
#endif
            
            Debug.Log("Display Matrix received and cached.");
        }
    }
    
    void Update()
    {
        if (_occlusionManager == null || _occlusionManager.subsystem == null)
        {
            Debug.LogError("OcclusionManager or its subsystem is null.");
            return;
        }

        if (!_occlusionManager.subsystem.running)
        {
            Debug.Log("Occlusion Subsystem is NOT running");
            return;
        }

        if (_occlusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            // Dispose the old image
            _depthImage?.Dispose();
            _depthImage = image;
            
            Debug.Log("Depth image acquired successfully.");
        }
        else
        {
            Debug.LogWarning("Failed to acquire depth image.");
        }
    }
    
    public Vector3 ConvertScreenToWorldPosition(Vector2 rect)
    {
        if (_depthImage.HasValue)
        {
            Debug.Log("Depth image has value.");
            var screenPosition = new Vector2(rect.x, rect.y);
            // Sample eye depth
            var uv = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
            var eyeDepth = (float)_depthImage.Value.Sample<float>(uv, _displayMatrix);
                
            // Get world position
            var worldPosition = _camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, eyeDepth));

            return worldPosition;
        }
        else
        {
            Debug.LogWarning("No depth image available.");
            return Vector3.zero;
        }
    }
}
