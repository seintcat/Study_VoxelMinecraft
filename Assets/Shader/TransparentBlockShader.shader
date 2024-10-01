Shader "Minecraft/Transparent Blocks"
{
    Properties
    {
        _MainTex ("Block Texture Atlas", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        LOD 100
        Lighting off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
    //        // make fog work
    //        #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float GlobalLightLevel;
            float minGlobalLightLevel;
            float maxGlobalLightLevel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float shade = (maxGlobalLightLevel - minGlobalLightLevel) * GlobalLightLevel + minGlobalLightLevel;
                shade *= i.color.a;
                shade = clamp(1 - shade, minGlobalLightLevel, maxGlobalLightLevel);
                
                //float localLightLevel = clamp(GlobalLightLevel + i.color.a, 0, 1);

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                clip(col.a - 1);
                col = lerp(col, float4(0, 0, 0, 1), shade);
                return col;
            }
            ENDCG
        }
    }
}
