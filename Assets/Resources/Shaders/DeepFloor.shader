// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DeepFloor"
{
	Properties
	{
		_Distance("Distance", Float) = 0
		_Foam("Foam", Range( 0 , 0.5)) = 0.1095562
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
		};

		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Distance;
		uniform float _Foam;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 color28 = IsGammaSpace() ? float4(0,0.4657798,0.8392157,0) : float4(0,0.1837232,0.6724432,0);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth29 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth29 = abs( ( screenDepth29 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Distance ) );
			float4 lerpResult26 = lerp( float4( 0,0,0,0 ) , color28 , saturate( distanceDepth29 ));
			float4 color36 = IsGammaSpace() ? float4(0.002580996,0.5471698,0.384861,0) : float4(0.0001997675,0.2603273,0.1225043,0);
			float4 lerpResult35 = lerp( lerpResult26 , color36 , saturate( ( 1.0 - ( distanceDepth29 / _Foam ) ) ));
			o.Emission = lerpResult35.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
0;421;1117;266;984.576;706.8389;2.059662;True;False
Node;AmplifyShaderEditor.RangedFloatNode;30;-718.6857,-389.5046;Inherit;False;Property;_Distance;Distance;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;29;-550.3193,-403.8772;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-404.6681,-270.2031;Inherit;False;Property;_Foam;Foam;1;0;Create;True;0;0;0;False;0;False;0.1095562;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;-175.1365,-280.1956;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;28;-439.2392,-679.2473;Inherit;False;Constant;_Color2;Color 2;4;0;Create;True;0;0;0;False;0;False;0,0.4657798,0.8392157,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;27;-298.5936,-447.5054;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;-58.73248,-272.238;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;26;-198.7068,-568.0712;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;33;91.85131,-260.0285;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;-9.663633,-468.9956;Inherit;False;Constant;_Color0;Color 0;2;0;Create;True;0;0;0;False;0;False;0.002580996,0.5471698,0.384861,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;35;304.0128,-495.6997;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;519.8757,-610.1925;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;DeepFloor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;30;0
WireConnection;31;0;29;0
WireConnection;31;1;34;0
WireConnection;27;0;29;0
WireConnection;32;0;31;0
WireConnection;26;1;28;0
WireConnection;26;2;27;0
WireConnection;33;0;32;0
WireConnection;35;0;26;0
WireConnection;35;1;36;0
WireConnection;35;2;33;0
WireConnection;0;2;35;0
ASEEND*/
//CHKSM=6F9541B5366932BE244006A78356A3EABD3F5CCC