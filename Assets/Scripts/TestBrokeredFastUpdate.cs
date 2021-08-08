﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using BrokeredUpdates;

public class TestBrokeredFastUpdate : UdonSharpBehaviour
{
	private BrokeredUpdates.BrokeredUpdateManager brokeredUpdateManager;
	private bool bOnList = false;
	private Material m;

	void Start()
	{
		brokeredUpdateManager = GameObject.Find( "BrokeredUpdateManager" ).GetComponent<BrokeredUpdateManager>();
	}
	
	public void BrokeredUpdate()
	{
		Vector4 col = GetComponent<MeshRenderer>().material.GetVector( "_Color" );
		col.x = ( col.x + 0.01f ) % 1;
		col.y = ( col.y + 0.01f ) % 1;
		col.z = ( col.z + 0.01f ) % 1;
		GetComponent<MeshRenderer>().material.SetVector( "_Color", col );
	}
	
	public override void Interact()
	{
		if( bOnList )
		{
			bOnList = false;
			brokeredUpdateManager.UnregisterSubscription( this );			
		}
		else
		{
			bOnList = true;
			brokeredUpdateManager.RegisterSubscription( this );
		}
	}
}
