
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using BrokeredUpdates;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class WorldDestroyer : UdonSharpBehaviour
{
	public int DestroyMode;
	[UdonSynced]
	public bool DoDestroy;
	private bool WasDestroyed;
	public GameObject Container;
    void Start()
    {
        WasDestroyed = false;
    }
	
	public override void OnDeserialization()
	{
		if( DoDestroy && !WasDestroyed )
		{
			Debug.Log( $"World Destroy {DestroyMode}" );
			BrokeredBlockIDer [] bs = (BrokeredBlockIDer[])Container.transform.GetComponentsInChildren<BrokeredBlockIDer>();
			
			foreach( BrokeredBlockIDer b in bs )
			{
				b._SetModeInternal( DestroyMode );
				b._UpdateID();
				b._SnapNow();
			}
		}
		WasDestroyed = DoDestroy;
	}
	
	public override void Interact()
	{
		bool master = Networking.IsMaster;
		Debug.Log( $"Interact {master}" );
		if( master )
		{
			Networking.SetOwner( Networking.LocalPlayer, gameObject );
			DoDestroy = !DoDestroy;
			RequestSerialization();
			OnDeserialization();
		}
	}
}
