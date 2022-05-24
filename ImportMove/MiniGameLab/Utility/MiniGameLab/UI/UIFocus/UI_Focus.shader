// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MiniGameLab/UI_Focus"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_Radius("Radius", Float) = 0
		_Hardness("Hardness", Float) = 0
		_IsRadial("IsRadial", Int) = 0
		_Center("Center", Vector) = (0,0,0,0)
		_RectArea("RectArea", Vector) = (0,0,0,0)
		_Alpha("Alpha", Float) = 0

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord2 : TEXCOORD2;
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform half _Alpha;
			uniform int _IsRadial;
			uniform half2 _Center;
			uniform half _Radius;
			uniform half _Hardness;
			uniform half2 _RectArea;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				float3 ase_worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
				OUT.ase_texcoord2.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				OUT.ase_texcoord2.w = 0;
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				half4 appendResult53 = (half4(0.0 , 0.0 , 0.0 , _Alpha));
				float3 ase_worldPos = IN.ase_texcoord2.xyz;
				half3 temp_output_5_0_g7 = ( ( ase_worldPos - half3( _Center ,  0.0 ) ) / _Radius );
				half dotResult8_g7 = dot( temp_output_5_0_g7 , temp_output_5_0_g7 );
				half4 lerpResult33 = lerp( float4( 0,0,0,0 ) , appendResult53 , ( (float)_IsRadial == 0.0 ? pow( saturate( dotResult8_g7 ) , _Hardness ) : saturate( ( distance( max( ( abs( ( ase_worldPos - half3( _Center ,  0.0 ) ) ) - ( half3( _RectArea ,  0.0 ) * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / _Hardness ) ) ));
				
				half4 color = lerpResult33;
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18912
817;225;1101;677;1912.319;-87.93668;1.393261;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;21;-1180.499,700.767;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;15;-1150.242,624.998;Inherit;False;Property;_Hardness;Hardness;1;0;Create;True;0;0;0;False;0;False;0;39.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;42;-1149.76,365.3371;Inherit;False;Property;_Center;Center;3;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;43;-1149.759,854.3984;Inherit;False;Property;_RectArea;RectArea;4;0;Create;True;0;0;0;False;0;False;0,0;200,200;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;14;-1161.738,513.4129;Inherit;False;Property;_Radius;Radius;0;0;Create;True;0;0;0;False;0;False;0;529.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;50;-789.1203,589.4601;Inherit;False;BoxMask;-1;;1;9dce4093ad5a42b4aa255f0153c4f209;0;4;1;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;10;FLOAT3;0,0,0;False;17;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;37;-496.4942,441.2796;Inherit;False;Property;_IsRadial;IsRadial;2;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;52;-509.1651,620.1131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-528.2708,278.8669;Inherit;False;Property;_Alpha;Alpha;5;0;Create;True;0;0;0;False;0;False;0;0.875;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;60;-791.4339,452.2419;Inherit;False;SphereMask;-1;;7;988803ee12caf5f4690caee3c8c4a5bb;0;3;15;FLOAT2;0,0;False;14;FLOAT;0;False;12;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;40;-352.0193,481.8695;Inherit;False;0;4;0;INT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-322.2708,188.8669;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;33;-150.3059,283.8252;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;8;31.67547,284.247;Half;False;True;-1;2;ASEMaterialInspector;0;6;MiniGameLab/UI_Focus;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;True;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;50;1;21;0
WireConnection;50;4;42;0
WireConnection;50;10;43;0
WireConnection;50;17;15;0
WireConnection;52;0;50;0
WireConnection;60;15;42;0
WireConnection;60;14;14;0
WireConnection;60;12;15;0
WireConnection;40;0;37;0
WireConnection;40;2;60;0
WireConnection;40;3;52;0
WireConnection;53;3;54;0
WireConnection;33;1;53;0
WireConnection;33;2;40;0
WireConnection;8;0;33;0
ASEEND*/
//CHKSM=5DA684766D63900DC5EE975680492961D8B2F86A