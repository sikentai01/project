Shader "Unlit/hallway1"
{
	
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MinPoint ("Min Point", Vector) = (0.0, 0.0, 0.0, 0.0) // 暗くする範囲の最小座標（ワールド座標）
        _MaxPoint ("Max Point", Vector) = (1.0, 1.0, 0.0, 0.0) // 暗くする範囲の最大座標（ワールド座標）
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
                float4 worldPos : TEXCOORD1; // ワールド座標を渡す
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MinPoint;
            float4 _MaxPoint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); // ワールド座標を計算
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // テクスチャの色を取得
                fixed4 col = tex2D(_MainTex, i.uv);

                // ピクセルのワールド座標が指定範囲内かチェック
                // `i.worldPos.xyz`が `_MinPoint.xyz`と`_MaxPoint.xyz`の間にあるか
                bool inRangeX = i.worldPos.x >= _MinPoint.x && i.worldPos.x <= _MaxPoint.x;
                bool inRangeY = i.worldPos.y >= _MinPoint.y && i.worldPos.y <= _MaxPoint.y;
                bool inRangeZ = i.worldPos.z >= _MinPoint.z && i.worldPos.z <= _MaxPoint.z;

                // 範囲内であれば色を暗くする
                if (inRangeX && inRangeY && inRangeZ)
                {
                    col.rgb *= 0.5; // 明るさを50%に下げる例
                }

                return col;
            }
            ENDCG
        }
    }
}
