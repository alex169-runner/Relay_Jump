Shader "Custom/GridHighlighter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (0,0,1,1)
        _HighlightPositions ("Highlight Positions", Vector) = (0,0,0,0)
        _HighlightCount ("Highlight Count", Int) = 0
        _HighlightRadius ("Highlight Radius", Float) = 0.6
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
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _HighlightColor;
            float _HighlightRadius;

            float4 _HighlightPositions[10];
            int _HighlightCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 基础纹理颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 检查是否需要高亮
                for (int idx = 0; idx < _HighlightCount; idx++)
                {
                    float2 highlightPos = _HighlightPositions[idx].xy;
                    float dist = distance(i.worldPos.xy, highlightPos);
                    
                    if (dist < _HighlightRadius)
                    {
                        // 直接返回蓝色
                        return _HighlightColor;
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
}