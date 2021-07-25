
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class CustomRaycastSystem : UdonSharpBehaviour
{
	public LayerMask rmask;
	public RaycastHit lastHit;
	public int currentHandID;
	public Component [] lasthits;
	
	private bool inVR;

	void Start()
	{
		inVR = false;
		VRCPlayerApi localPlayer = Networking.LocalPlayer;
		if( localPlayer != null && localPlayer.IsUserInVR() )
		{
			inVR = true;
		}
		lasthits = new Component[2];
	}
	
	void Update()
	{
		VRCPlayerApi localPlayer = Networking.LocalPlayer;

		for( currentHandID = 0; currentHandID < (inVR?2:1); currentHandID++ )
		{
			VRCPlayerApi.TrackingDataType hand;
			if( !inVR ) hand = VRCPlayerApi.TrackingDataType.Head;
			else if( currentHandID == 0 ) hand = VRCPlayerApi.TrackingDataType.LeftHand;
			else hand = VRCPlayerApi.TrackingDataType.RightHand;
			VRCPlayerApi.TrackingData xformHand = localPlayer.GetTrackingData( hand );
			Vector3 Pos = xformHand.position;
			Vector3 Dir = (xformHand.rotation * Quaternion.Euler(0.0f, 41.0f, 0.0f) ) * Vector3.forward;
			
			UdonBehaviour behavior;
			
			if (!Physics.Raycast( Pos, Dir, out lastHit, 3.0f, rmask.value ) || ( lastHit.transform == null ) )
			{
				behavior = null;
			}
			else
			{
				behavior = (UdonBehaviour)lastHit.transform.GetComponent( typeof(UdonBehaviour) );
			}
			
			if( behavior != lasthits[currentHandID] && lasthits[currentHandID] != null )
			{
				((UdonBehaviour)lasthits[currentHandID]).SendCustomEvent("RaycastIntersectionLeave");
				lasthits[currentHandID] = null;
			}
			if( behavior != null )
			{
				behavior.SendCustomEvent("RaycastIntersectedMotion");
				lasthits[currentHandID] = behavior;
			}
		}
	}
}
