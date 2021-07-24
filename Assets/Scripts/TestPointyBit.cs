
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TestPointyBit : UdonSharpBehaviour
{
    void Start()
    {
        
    }
	
	//public override void OnEnterTrigger( Collider c )
	public override void Interact( )
	{
		Debug.Log( "OET\n" );
	}
}
