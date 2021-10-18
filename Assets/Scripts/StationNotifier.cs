
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class StationNotifier : UdonSharpBehaviour
{
	public DemoMapScript demo;
	
	public override void OnStationEntered( VRCPlayerApi p )
	{
		Debug.Log( "OnStationEntered" );
		demo._StationEnter( (VRCStation)gameObject.GetComponent(typeof(VRCStation)), p );
	}

	public override void OnStationExited( VRCPlayerApi p )
	{
		Debug.Log( "OnStationExited" );
		demo._StationExit( (VRCStation)gameObject.GetComponent(typeof(VRCStation)), p );
	}
	
	public override void Interact()
    {
        Networking.LocalPlayer.UseAttachedStation();
    }

    void Start()
    {
        
    }
}
