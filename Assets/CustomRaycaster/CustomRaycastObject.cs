using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;


namespace BrokeredUpdates
{
	public class CustomRaycastObject : UdonSharpBehaviour
	{
		private CustomRaycastSystem customRaycastSystem;
		private MeshRenderer mr;
		private MaterialPropertyBlock block;
		private int [] useFlags;
		private Vector2 [] lastUV;
		void Start()
		{
			useFlags = new int[2];
			lastUV = new Vector2[2];
			block = new MaterialPropertyBlock();
			lastUV[0] = new Vector2( -1000, -1000 );
			lastUV[1] = new Vector2( -1000, -1000 );
			mr = GetComponent<MeshRenderer>();
			customRaycastSystem = GameObject.Find( "CustomRaycastSystem" ).GetComponent<CustomRaycastSystem>();
		}

		public void InputGrab( bool valu, VRC.Udon.Common.UdonInputEventArgs args )
		{
			int hand = ( args.handType == HandType.LEFT )?0:1;
			if( args.boolValue )
				useFlags[hand] |= 1;
			else
				useFlags[hand] &= ~1;
		}

		public void InputUse( bool valu, VRC.Udon.Common.UdonInputEventArgs args )
		{
			int hand = ( args.handType == HandType.LEFT )?0:1;
			if( args.boolValue )
				useFlags[hand] |= 2;
			else
				useFlags[hand] &= ~2;
			UpdateMatProps( hand );
		}
		
		public void UpdateMatProps( int hand )
		{
			mr.GetPropertyBlock(block);

			float triggerQty = 
				Mathf.Max(Input.GetAxisRaw((hand==0)?"Oculus_CrossPlatform_PrimaryIndexTrigger":"Oculus_CrossPlatform_SecondaryIndexTrigger"),
				Input.GetMouseButton(0) ? 1 : 0);

			block.SetVector( (hand==0)?"_TargetUV0":"_TargetUV1",
				new Vector4( 
					lastUV[hand].x,
					lastUV[hand].y,
					useFlags[hand], triggerQty ) );
			mr.SetPropertyBlock(block);
		}
		
		public void RaycastIntersectionLeave()
		{
			int hid = customRaycastSystem.currentHandID;
			lastUV[hid] = new Vector2( -1000, -1000 );
			UpdateMatProps( hid );
		}
		
		public void RaycastIntersectedMotion()
		{
			int hid = customRaycastSystem.currentHandID;
			lastUV[hid] = customRaycastSystem.lastHit.textureCoord;
			UpdateMatProps( hid );
		}
	}
}
