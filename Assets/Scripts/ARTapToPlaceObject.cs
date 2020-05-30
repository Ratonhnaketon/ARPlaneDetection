using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARTapToPlaceObject : MonoBehaviour
{ 
 	public GameObject placementIndicator, objectToPlace;
	private ARRaycastManager arRayCast;

	void Start()
	{
		arRayCast = GetComponent<ARRaycastManager>();
	}

	void Update()
	{
		var (placementPoseIsValid, placementPose) = updatePlacementPose();
		updatePlacementIndicator(placementPoseIsValid, placementPose);
		placeObject(placementPoseIsValid, placementPose);
	}

    private Tuple<bool, Pose> updatePlacementPose()
	{
		
		Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
		List<ARRaycastHit> hits = new List<ARRaycastHit>();
		arRayCast.Raycast(screenCenter, hits, TrackableType.Planes);

		Pose placementPose = new Pose();
		bool placementPoseIsValid = hits.Count > 0;

		if (placementPoseIsValid)
		{
			Vector3 cameraFoward = Camera.current.transform.forward;
			Vector3 cameraBearing = new Vector3(cameraFoward.x, 0, cameraFoward.z).normalized;
			
			placementPose = hits[0].pose;
			placementPose.rotation = Quaternion.LookRotation(cameraBearing);
		}

		return Tuple.Create(placementPoseIsValid, placementPose);
	}

	private void updatePlacementIndicator(bool placementPoseIsValid, Pose placementPose)
	{
		placementIndicator.SetActive(placementPoseIsValid);
		if (placementPoseIsValid)
		{
			placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
		}
	}

    private void placeObject(bool validPosition, Pose placement)
    {
		if (validPosition && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Instantiate(objectToPlace, placement.position, placement.rotation); 
		}
    }
}
