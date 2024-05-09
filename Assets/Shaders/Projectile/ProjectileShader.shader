Shader "Custom/BackgroundOverlayScrollingAndScaling"
{
    Properties
    {
        _BackgroundTex("Background Texture", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", Range(0.0, 10.0)) = 1.0
        _Scale("Scale", Range(0.0, 0.01)) = 0.001
        _Transparency("Transparency", Range(0.0, 1.0)) = 0.5
    }

        SubShader
        {
            Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
            LOD 200

            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                float _ScrollSpeed;
                float _Scale;

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _BackgroundTex;
                float _Transparency;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv.y = (o.vertex.y / o.vertex.w + 1) * -0.5 * _Scale * _ScreenParams.xy + _Time.y * _ScrollSpeed;
                    o.uv.x = (o.vertex.x / o.vertex.w + 1) * 0.5 * _Scale * _ScreenParams.xy;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Sample from the background texture
                    fixed4 bgCol = tex2D(_BackgroundTex, i.uv);

                // Apply transparency
                bgCol.a *= _Transparency;

                return bgCol;
            }
            ENDCG
        }
        }
}
