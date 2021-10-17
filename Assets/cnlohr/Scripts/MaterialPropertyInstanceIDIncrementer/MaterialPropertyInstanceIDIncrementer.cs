
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using BrokeredUpdates;


#if !COMPILER_UDONSHARP && UNITY_EDITOR
using VRC.SDKBase.Editor.BuildPipeline;
using UnityEditor;
using UdonSharpEditor;
using System.Collections.Immutable;
#endif

namespace BrokeredUpdates
{
	[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
	public class MaterialPropertyInstanceIDIncrementer : UdonSharpBehaviour
	{
		public BrokeredUpdateManager brokeredUpdateManager;

		void Start()
		{
			MaterialPropertyBlock block;
			MeshRenderer mr;

			int id = brokeredUpdateManager._GetIncrementingID();
			block = new MaterialPropertyBlock();
			mr = GetComponent<MeshRenderer>();
			//mr.GetPropertyBlock(block);
			block.SetVector( "_InstanceID", new Vector4( id, 0, 0, 0 ) );
			mr.SetPropertyBlock(block);
		}
	}
}



#if !COMPILER_UDONSHARP && UNITY_EDITOR
namespace UdonSharp
{
    public class UdonSharpBuildChecks_MaterialPropertyInstanceIDIncrementer : IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => -1;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
			if (requestedBuildType == VRCSDKRequestedBuildType.Avatar) return true;
			BrokeredUpdates.MaterialPropertyInstanceIDIncrementer [] bs = Resources.FindObjectsOfTypeAll( typeof( BrokeredUpdates.MaterialPropertyInstanceIDIncrementer ) ) as BrokeredUpdates.MaterialPropertyInstanceIDIncrementer[];
			foreach( BrokeredUpdates.MaterialPropertyInstanceIDIncrementer b in bs )
			{
				b.UpdateProxy();
				if( b.brokeredUpdateManager == null )
				{
					Debug.LogError($"[<color=#FF00FF>MaterialPropertyInstanceIDIncrementer</color>] Missing brokeredUpdateManager reference on {b.gameObject.name}");
					typeof(UnityEditor.SceneView).GetMethod("ShowNotification", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(null, new object[] { $"BrokeredSync missing brokeredUpdateManager reference on {b.gameObject.name}" });
					return false;				
				}
			}
			return true;
		}
	}
}

namespace BrokeredUpdates
{
    [CustomEditor(typeof(BrokeredUpdates.MaterialPropertyInstanceIDIncrementer))]
    public class BrokeredUpdatesMaterialPropertyInstanceIDIncrementerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            EditorGUILayout.Space();
			int ct = 0;
            if (GUILayout.Button(new GUIContent("Attach brokeredUpdateManager to all Brokered Sync objects.", "Automatically finds all Brokered Sync objects and attaches the manager.")))
			{
				MaterialPropertyInstanceIDIncrementer [] bs = Resources.FindObjectsOfTypeAll( typeof( MaterialPropertyInstanceIDIncrementer ) ) as MaterialPropertyInstanceIDIncrementer[];
				BrokeredUpdateManager [] managers = Resources.FindObjectsOfTypeAll( typeof( BrokeredUpdateManager ) ) as BrokeredUpdateManager[];
				if( managers.Length < 1 )
				{
					Debug.LogError($"[<color=#FF00FF>UdonSharp</color>] Could not find a BrokeredUpdateManager. Did you add the prefab to your scene?");
					return;
				}
				BrokeredUpdateManager manager = managers[0];
				foreach( MaterialPropertyInstanceIDIncrementer b in bs )
				{
					b.UpdateProxy();
					if( b.brokeredUpdateManager == null )
					{
						Debug.Log( $"Attaching to {b.gameObject.name}" );
						//https://github.com/MerlinVR/UdonSharp/wiki/Editor-Scripting#non-inspector-editor-scripts
						b.brokeredUpdateManager = manager;
						b.ApplyProxyModifications();
						ct++;
					}
				}
			}
			if( ct > 0 )
				Debug.Log( $"Attached {ct} manager references." );
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}
#endif
