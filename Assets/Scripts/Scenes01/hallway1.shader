Shader "Unlit/hallway1"
{
	
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MinPoint ("Min Point", Vector) = (0.0, 0.0, 0.0, 0.0) // �Â�����͈͂̍ŏ����W�i���[���h���W�j
        _MaxPoint ("Max Point", Vector) = (1.0, 1.0, 0.0, 0.0) // �Â�����͈͂̍ő���W�i���[���h���W�j
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
                float4 worldPos : TEXCOORD1; // ���[���h���W��n��
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
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); // ���[���h���W���v�Z
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // �e�N�X�`���̐F���擾
                fixed4 col = tex2D(_MainTex, i.uv);

                // �s�N�Z���̃��[���h���W���w��͈͓����`�F�b�N
                // `i.worldPos.xyz`�� `_MinPoint.xyz`��`_MaxPoint.xyz`�̊Ԃɂ��邩
                bool inRangeX = i.worldPos.x >= _MinPoint.x && i.worldPos.x <= _MaxPoint.x;
                bool inRangeY = i.worldPos.y >= _MinPoint.y && i.worldPos.y <= _MaxPoint.y;
                bool inRangeZ = i.worldPos.z >= _MinPoint.z && i.worldPos.z <= _MaxPoint.z;

                // �͈͓��ł���ΐF���Â�����
                if (inRangeX && inRangeY && inRangeZ)
                {
                    col.rgb *= 0.5; // ���邳��50%�ɉ������
                }

                return col;
            }
            ENDCG
        }
    }
}
