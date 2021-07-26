
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class DumpSyncedObjects : UdonSharpBehaviour
{
	public GameObject rootOfTreeToDump;
    void Start()
    {
        
    }
	
	void Interact()
	{
		Debug.Log( "DUMPING OBJECTS ================" );
		Transform [] allChildren = rootOfTreeToDump.GetComponentsInChildren<Transform>();
		int i;
		for( i = 0; i < allChildren.Length; i++ )
		{
			UdonBehaviour b = (UdonBehaviour)allChildren[i].GetComponent( typeof(UdonBehaviour) );
			if( b != null )
				b.SendCustomEvent("LogBlockState");
		}
		Debug.Log( "DUMPED OBJECTS ================" );
	}
}
