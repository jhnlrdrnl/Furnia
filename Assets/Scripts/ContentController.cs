using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ContentController : ARBaseGestureInteractable
{
    public BundleWebLoader api;
    private GameObject furniture;
    private Pose pose;
    private readonly List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();

    [SerializeField] private GameObject loading, _crosshair;
    [SerializeField] private Camera _arCamera;
    [SerializeField] private ARRaycastManager _arRaycastManager;
    [SerializeField] private TextMeshProUGUI syncText;
    
    private void FixedUpdate()
    {
        if (_crosshair.activeSelf)
            CrosshairCalculation();
    }

    public void OnFinishPlacement()
    {
        _crosshair.SetActive(false);
    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.targetObject == null || gesture.targetObject.layer == 9)
            return true;
        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.isCanceled || !_crosshair.activeSelf)
            return;

        if (gesture.targetObject != null && gesture.targetObject.layer != 9)
            return;

        if (IsPointerOverUI(gesture))
            return;

        if (GestureTransformationUtility.Raycast(gesture.startPosition, _raycastHits, TrackableType.PlaneWithinPolygon))
        {
            var hit = _raycastHits[0];

            if (Vector3.Dot(Camera.main.transform.position - hit.pose.position, hit.pose.rotation * Vector3.up) < 0)
                return;

            GameObject placedObj = Instantiate(furniture, pose.position, pose.rotation);

            var anchorObject = new GameObject("PlacementAnchor");
            anchorObject.transform.SetPositionAndRotation(pose.position, pose.rotation);

            placedObj.transform.parent = anchorObject.transform;
        }
    }

    void CrosshairCalculation()
    {
        Vector3 origin = _arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

        if (GestureTransformationUtility.Raycast(origin, _raycastHits, TrackableType.PlaneWithinPolygon))
        {
            pose = _raycastHits[0].pose;
            var cameraForward = _arCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            pose.rotation = Quaternion.LookRotation(cameraBearing);
            _crosshair.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
    }

    bool IsPointerOverUI(TapGesture touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(touch.startPosition.x, touch.startPosition.y)
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    public void LoadContent(string name)
    {
        ShowLoading();
        api.GetObjectBundle(name, OnContentLoaded);
    }

    void OnContentLoaded(GameObject content)
    {
        HideLoading();
        furniture = content;
        syncText.text = "Asset retrieved. " + content.name + " is ready for instantiating";
        Debug.Log("Asset retrieved. " + content.name + " is ready for instantiating");
    }

    void ShowLoading() => loading.SetActive(true);
    void HideLoading() => loading.SetActive(false);
}
