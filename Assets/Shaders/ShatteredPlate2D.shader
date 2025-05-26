Shader "Custom/ShatteredPlate2D"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Параметры эффекта разбивания
        _ShatterProgress ("Shatter Progress", Range(0, 1)) = 0
        _CrackWidth ("Crack Width", Range(0, 0.1)) = 0.01
        _CrackColor ("Crack Color", Color) = (0.2, 0.2, 0.2, 1)
        _ShatterCells ("Shatter Cells", Range(5, 50)) = 15
        _RandomSeed ("Random Seed", Float) = 42
        _ExplosionForce ("Explosion Force", Range(0, 2)) = 0.5
        _GravityEffect ("Gravity Effect", Range(0, 1)) = 0.3
        _RotationSpeed ("Rotation Speed", Range(0, 5)) = 2
        _FadeOut ("Fade Out", Range(0, 1)) = 0.5
        
        // Unity sprite renderer properties
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
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
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _RendererColor;
            fixed4 _Flip;
            
            float _ShatterProgress;
            float _CrackWidth;
            fixed4 _CrackColor;
            float _ShatterCells;
            float _RandomSeed;
            float _ExplosionForce;
            float _GravityEffect;
            float _RotationSpeed;
            float _FadeOut;
            
            // Функция для генерации псевдослучайного числа
            float2 random2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                          dot(p, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }
            
            // Функция для создания диаграммы Вороного
            float3 voronoi(float2 uv, float time)
            {
                float2 n = floor(uv);
                float2 f = frac(uv);
                
                float minDist = 1e9;
                float2 minPoint = float2(0, 0);
                float2 minCell = float2(0, 0);
                
                // Проверяем соседние ячейки
                for (int j = -1; j <= 1; j++)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        float2 neighbor = float2(i, j);
                        float2 cellPos = n + neighbor;
                        float2 randPoint = random2(cellPos + _RandomSeed);
                        
                        // Анимация точек для эффекта дрожания
                        randPoint = 0.5 + 0.5 * sin(time * 0.5 + 6.2831 * randPoint);
                        
                        float2 point_x = neighbor + randPoint;
                        float dist = length(point_x - f);
                        
                        if (dist < minDist)
                        {
                            minDist = dist;
                            minPoint = point_x + n;
                            minCell = cellPos;
                        }
                    }
                }
                
                return float3(minDist, minCell.xy);
            }
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                
                // Базовая трансформация вершин
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.color = IN.color * _Color * _RendererColor;
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xy;
                
                // Применяем flip для спрайтов
                OUT.texcoord.x = (OUT.texcoord.x - 0.5) * _Flip.x + 0.5;
                OUT.texcoord.y = (OUT.texcoord.y - 0.5) * _Flip.y + 0.5;
                
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // Получаем цвет текстуры
                fixed4 texColor = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Вычисляем Voronoi паттерн
                float2 scaledUV = IN.texcoord * _ShatterCells;
                float3 voronoiData = voronoi(scaledUV, _Time.y);
                
                // Расстояние до края ячейки
                float edgeDist = voronoiData.x;
                float2 cellID = voronoiData.yz;
                
                // Создаем маску трещин
                float crackMask = smoothstep(_CrackWidth * 0.5, _CrackWidth, edgeDist);
                
                // Если прогресс > 0, начинаем анимацию разлетания
                if (_ShatterProgress > 0)
                {
                    // Уникальное смещение для каждой ячейки
                    float2 cellRandom = random2(cellID + _RandomSeed * 2.0);
                    float2 cellDirection = normalize(cellRandom);
                    
                    // Добавляем взрывную силу от центра
                    float2 fromCenter = IN.texcoord - 0.5;
                    cellDirection = lerp(cellDirection, normalize(fromCenter), 0.7);
                    
                    // Вычисляем смещение осколка
                    float2 displacement = cellDirection * _ShatterProgress * _ExplosionForce;
                    
                    // Добавляем гравитацию
                    displacement.y -= _ShatterProgress * _ShatterProgress * _GravityEffect;
                    
                    // Смещаем UV координаты для эффекта разлетания
                    float2 displacedUV = IN.texcoord - displacement;
                    
                    // Проверяем, принадлежит ли текущий пиксель смещенной ячейке
                    float3 displacedVoronoi = voronoi(displacedUV * _ShatterCells, _Time.y);
                    
                    if (abs(displacedVoronoi.y - cellID.x) < 0.1 && abs(displacedVoronoi.z - cellID.y) < 0.1)
                    {
                        // Добавляем вращение осколков
                        float rotation = _ShatterProgress * _RotationSpeed * (cellRandom.x * 2.0 - 1.0);
                        float2 rotatedUV = displacedUV - 0.5;
                        float cosRot = cos(rotation);
                        float sinRot = sin(rotation);
                        float2 finalUV = float2(
                            rotatedUV.x * cosRot - rotatedUV.y * sinRot,
                            rotatedUV.x * sinRot + rotatedUV.y * cosRot
                        ) + 0.5;
                        
                        // Получаем цвет с новых координат
                        texColor = tex2D(_MainTex, finalUV) * IN.color;
                        
                        // Применяем затухание
                        float fadeFactor = 1.0 - _ShatterProgress * _FadeOut;
                        texColor.a *= fadeFactor;
                    }
                    else
                    {
                        // Пиксель не принадлежит ячейке - делаем прозрачным
                        texColor.a = 0;
                    }
                }
                else
                {
                    // Без анимации просто показываем трещины
                    texColor = lerp(_CrackColor, texColor, crackMask);
                }
                
                return texColor;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}