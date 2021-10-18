
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DemoMapScript : UdonSharpBehaviour
{
	public Light maplight;
	public VRCStation s1;
	public VRCStation s2;
	private bool StationUsed1;
	private bool StationUsed2;
	public VRCPlayerApi sp1;
	public VRCPlayerApi sp2;

	[UdonSynced]
	private float timeofday;
	private float timebothbeds;
	private float timesincesync;
    void Start()
    {
		if( Networking.IsMaster )
		{
			timeofday = 160;
		}
		timesincesync = 0;
    }
	
	void Update()
	{
		timeofday += Time.deltaTime/5.0f;
		Quaternion myRotation = Quaternion.identity;
        myRotation.eulerAngles =  new Vector3( timeofday, 270, -180 );
		maplight.transform.localRotation = myRotation;

		if( timeofday>=360 )
		{
			timeofday-=360;
		}
		
		float dist_from_90 = timeofday - 90;
		if( dist_from_90 < 0 ) dist_from_90 = -dist_from_90;
		float inten = (95.0f - dist_from_90)/15.0f;
		if( inten < 0 ) inten = 0;
		if( inten > 1 ) inten = 1;
		maplight.intensity = inten;
		
		if( Networking.GetOwner( gameObject ) == Networking.LocalPlayer )
		{
			if( Utilities.IsValid( sp1 ) && Utilities.IsValid( sp2 ) )
			{
				timebothbeds+= Time.deltaTime;
				Debug.Log( $"Both beds occupied {timebothbeds} {timeofday}" );
				if( timebothbeds > 5 )
				{
					if( timeofday > 170 && timeofday < 350 ) timeofday = 0;
					else timeofday = 180;
					s1.ExitStation( sp1 );
					s2.ExitStation( sp2 );
					sp1 = sp2 = null;
					timebothbeds = 0;
					RequestSerialization();
				}
			}
			else
			{
				timebothbeds = 0;
			}

			timesincesync += Time.deltaTime;
			if( timesincesync > 5 )
			{
				timesincesync = 0;
				RequestSerialization();
			}
		}
		
		
	}
	
	public void _StationEnter( VRCStation s, VRCPlayerApi p )
	{
		Debug.Log( $"Station Enter {s == s1} {s == s2}" );
		if( s == s1 ) { sp1 = p; }
		if( s == s2 ) { sp2 = p; }
	}

	public void _StationExit( VRCStation s, VRCPlayerApi p )
	{
		if( s == s1 ) { sp1 = null; }
		if( s == s2 ) { sp2 = null; }
		Debug.Log( $"Station Exit {s == s1} {s == s2}" );
	}

	public override void OnPlayerRespawn ( VRCPlayerApi p )
	{
		if( Utilities.IsValid( p ) )
		{
			if( p.displayName == "half＆half_madam" )
			{
				s1.UseStation( p );
			}
			if( p.displayName == "Yewnyx" )
			{
				s2.UseStation( p );
			}
			Debug.Log( $"TEST RESPAWN {p.displayName}" );
		}
	}
}
