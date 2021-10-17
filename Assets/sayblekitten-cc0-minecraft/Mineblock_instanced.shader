﻿Shader "Custom/Mineblock_instanced"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MineTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_OverrideBlockID ("Override Block ID", Range(-1,171) ) = -1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow 

        #pragma target 5.0

        sampler2D _MineTex;

        struct Input
        {
            float2 uv_MineTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _OverrideBlockID;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
			float4 _InstanceID;
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			int cursor0 = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceID).z;
			int cursor1 = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceID).w;
			float4 c;
			if( ( cursor0 <= 0 && cursor1 <= 0 ) || _OverrideBlockID >= 0 )
			{
				// Albedo comes from a texture tinted by color
				uint blockID;
				if( _OverrideBlockID >= 0 )
					blockID = _OverrideBlockID;
				else
					blockID = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceID).y;
				 
				float2 uv = frac( IN.uv_MineTex );
				float2 newuv = uv/16.0;
				newuv += float2( blockID % 16, blockID / 16 )/16.0;
				newuv.y = 1-newuv.y;
				c = tex2Dlod(_MineTex, float4( newuv, 0, 0 ));

				float2 uvbase = floor( newuv * 256 ) / 256.;
				float2 dlxy = 1./256;
				float2 clxy = frac( newuv * 256 );
				float4 cob1a = tex2Dlod(_MineTex, float4( uvbase+float2(0,0), 0, 0 )) * _Color;
				float4 cob1b = tex2Dlod(_MineTex, float4( uvbase+float2(dlxy.x,0), 0, 0 )) * _Color;
				float4 cob1c = tex2Dlod(_MineTex, float4( uvbase+float2(0,dlxy.y), 0, 0 )) * _Color;
				float4 cob1d = tex2Dlod(_MineTex, float4( uvbase+dlxy, 0, 0 )) * _Color;
				float4 cob1 = lerp( lerp( cob1a, cob1b, clxy.x ), lerp( cob1c, cob1d, clxy.x ), clxy.y );
				o.Normal = normalize( float3( 0, 0, 1 ) + cob1.xyz*.5 );
			}
			else
			{
				cursor0--;
				cursor1--;
				
				// Currently used block ID.
				uint blockID = UNITY_ACCESS_INSTANCED_PROP(Props, _InstanceID).y;

				int localblockid = floor( IN.uv_MineTex.x * 16 ) + floor( IN.uv_MineTex.y * 16 ) * 16;
				float2 localuv = frac( IN.uv_MineTex * 16 );
				float2 newuv = (localuv/16) + (float2( localblockid % 16, localblockid / 16 )/16.0);
				newuv.y = 1-newuv.y;
				c = tex2Dlod(_MineTex, float4( newuv, 0, 0 ));
				if( localblockid == cursor0 || localblockid == cursor1 )
				{
					c = frac( c + sin( _Time.y * 15. ) );
				}
				if( localblockid == blockID )
				{
					c = frac( c + sin( _Time.y * 2. ) );
				}
			}
			c *= _Color;
			//c = cob1;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}