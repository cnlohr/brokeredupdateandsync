
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CreateGrabbables : UdonSharpBehaviour
{
	private int x = 0, y = 0, z = 0;
	private BrokeredUpdateManager brokeredUpdateManager;
	private GameObject templateCube;
    void Start()
    {
		brokeredUpdateManager = GameObject.Find( "BrokeredUpdateManager" ).GetComponent<BrokeredUpdateManager>();
		Debug.Log( "START A\n" );
		brokeredUpdateManager.RegisterSubscription( this );
        templateCube = GameObject.Find( "GrabbableCube" );
		Debug.Log( "START B\n" );
/*
		for( x = 0; x < 10; x++ )
		for( y = 0; y < 10; y++ )
		for( z = 0; z < 10; z++ )
		{
			GameObject n = VRCInstantiate( templateCube );
			n.transform.position = new Vector3( x*.6f + 1, y*2 + (((x&1)!=0)?(z/10.0f):(-z/10.0f)), z*.6f + 1 );
			n.name = $"{x}_{y}_{z}";
		}
*/
    }
	
	
	public void BrokeredUpdate()
	{
	}
}
