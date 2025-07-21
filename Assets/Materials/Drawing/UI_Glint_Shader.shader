Shader "Custom/UI/IconSparkle_OnlyLine"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_HighlightColor("Highlight Color", Color) = (1,1,1,1)
		_HighlightWidth("Highlight Width", Float) = 0.1
		_Speed("Speed", Range(0,5)) = 1
		_StartTime("Start Time", Float) = 0
		_Duration("Duration", Float) = 1
	}

		SubShader
		{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Lighting Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _HighlightColor;
				float _HighlightWidth;
				float _Speed;
				float _StartTime;
				float _Duration;

				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 texCol = tex2D(_MainTex, i.uv);

					float2 dir = normalize(float2(1, -1));
					float diag = dot(i.uv, dir);

					// 변경된 타이밍 처리
					float elapsed = _Time.y - _StartTime;
					float progress = saturate(elapsed / _Duration); // 0~1 범위

					float sparkleLine = smoothstep(progress - _HighlightWidth, progress, diag) *
										(1.0 - smoothstep(progress, progress + _HighlightWidth, diag));

					float mask = texCol.a;

					fixed4 finalCol;
					finalCol.rgb = _HighlightColor.rgb * sparkleLine * mask;
					finalCol.a = sparkleLine * mask;

					return finalCol;
				}
			ENDCG
		}
		}
}
