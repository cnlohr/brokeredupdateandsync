Shader "Unlit/CustomRaycaster"
{
	Properties
	{
		[HideInInspector] _TargetUV0("Target UV0", Vector) = (0, 0, 0, 0 )
		[HideInInspector] _TargetUV1("Target UV1", Vector) = (0, 0, 0, 0 )
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float4 _TargetUV0;
			float4 _TargetUV1;

			float4 frag (v2f i) : SV_Target
			{
				int thispanel = floor( i.uv.x );
				float4 col = 0.;
				if( thispanel == floor( _TargetUV0.x ) )
				{
					col.x = _TargetUV0.w-5*length( i.uv - _TargetUV0 );
				}
				if( thispanel == floor( _TargetUV1.x ) )
				{
					col.y = _TargetUV1.w-5*length( i.uv - _TargetUV1 );
				}


				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
