Shader "Custom/FixedGlossySprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Highlight)]
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _HighlightIntensity ("Highlight Intensity", Range(0, 5)) = 1
        _HighlightPosition ("Highlight Position", Range(-1, 2)) = 0.5
        _HighlightWidth ("Highlight Width", Range(0.01, 1)) = 0.2
        _HighlightAngle ("Highlight Angle", Range(-180, 180)) = 45
        _HighlightFalloff ("Highlight Falloff", Range(0.1, 5)) = 2
        
        [Header(Animation)]
        _AnimationSpeed ("Animation Speed", Float) = 1
        _UseTimeAnimation ("Use Time Animation", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "UnityCG.cginc"

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
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _HighlightColor;
            float _HighlightIntensity;
            float _HighlightPosition;
            float _HighlightWidth;
            float _HighlightAngle;
            float _HighlightFalloff;
            float _AnimationSpeed;
            float _UseTimeAnimation;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Sample base texture
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Calculate highlight
                float2 uv = IN.texcoord;
                
                // Animate position if enabled
                float animatedPos = _HighlightPosition;
                if (_UseTimeAnimation > 0.5)
                {
                    animatedPos += sin(_Time.y * _AnimationSpeed) * 0.5;
                }
                
                // Rotate UV for angled highlight
                float angleRad = radians(_HighlightAngle);
                float2 rotatedUV = uv - 0.5;
                float2x2 rotMatrix = float2x2(cos(angleRad), -sin(angleRad), sin(angleRad), cos(angleRad));
                rotatedUV = mul(rotMatrix, rotatedUV) + 0.5;
                
                // Create highlight mask
                float highlightMask = abs(rotatedUV.x - animatedPos);
                highlightMask = 1.0 - smoothstep(0, _HighlightWidth, highlightMask);
                highlightMask = pow(highlightMask, _HighlightFalloff);
                
                // Apply highlight
                fixed3 highlight = _HighlightColor.rgb * highlightMask * _HighlightIntensity;
                c.rgb += highlight * c.a; // Умножаем на альфу базовой текстуры
                
                c.rgb *= c.a; // Premultiply alpha
                return c;
            }
            ENDCG
        }
    }
}