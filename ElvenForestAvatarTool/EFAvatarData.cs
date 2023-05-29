using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yakimannzyuu
{
    // アバターサイズ調整データ
    // 2023年5月12日　作成
    [CreateAssetMenu(fileName = "EFAvatarData", menuName = "Yakimannzyuu/EFAvatarData")]
    public class EFAvatarData : ScriptableObject
    {
        public List<EFDataContent> Contents;

        public List<string> UnenablePB;

        public EFDataContent Content(EFAvatarID ID)
        {
            if(ID == EFAvatarID.Unknown)
                return 
                    new EFDataContent
                    {
                        ViewName = "Unknown",
                        Names = new List<string>
                        {
                            "Unknown",
                        },
                        Scale = Vector3.one,
                        Position = Vector3.zero,
                    };
            return Contents[(int)ID];
        }

        // プレファブ検索。見つからない場合は-1:Unknownを返す。
        public EFAvatarID FindID(string prefabName)
        {
            for(EFAvatarID id = 0; (int)id < Contents.Count; id++)
                foreach(string str in Contents[(int)id].Names)
                    if(prefabName.Contains(str))
                        return id;
            return EFAvatarID.Unknown;
        }

        // 初期値書き込み
        void Awake()
        {
            if(Contents != null && Contents.Count > 0)
                return;
            Contents = new List<EFDataContent>
            {
                new EFDataContent
                {
                    ViewName = "基準",
                    Names = new List<string>
                    {
                        "Default",
                    },
                    Scale = Vector3.one,
                    Position = Vector3.zero,
                },
                new EFDataContent
                {
                    ViewName = "セフィ",
                    Names = new List<string>
                    {
                        "Shepir",
                    },
                    Scale = Vector3.one,
                    Position = Vector3.up * -0.019768f,
                },
                new EFDataContent
                {
                    ViewName = "ケセド",
                    Names = new List<string>
                    {
                        "Chesed",
                    },
                    Scale = Vector3.one * 1.110f,
                    Position = Vector3.up * 0.027131f,
                },
                new EFDataContent
                {
                    ViewName = "マルクト人型",
                    Names = new List<string>
                    {
                        "Omake",
                    },
                    Scale = Vector3.one * 1.021f,
                    Position = Vector3.up * 0.027131f,
                },
                new EFDataContent
                {
                    ViewName = "マルクト",
                    Names = new List<string>
                    {
                        "Malkuth",
                    },
                    Scale = Vector3.one * 1.021f,
                    Position = Vector3.up * 0.151071f,
                },
                new EFDataContent
                {
                    ViewName = "ネツァク",
                    Names = new List<string>
                    {
                        "Netzach"
                    },
                    Scale = Vector3.one,
                    Position = Vector3.zero,
                },
                new EFDataContent
                {
                    ViewName = "ホド",
                    Names = new List<string>
                    {
                        "Hod",
                    },
                    Scale = Vector3.one * 0.92f,
                    Position = Vector3.zero,
                },
            };

            UnenablePB = new List<string>{
                "Breast",
                "Hip"
            };
            EFToolMenu.Log("SephirAvatarDataを初期化しました。");
        }

        public Vector3 ChangeSize(EFAvatarID avatar, EFAvatarID child)
        {
            return Divid(Content(avatar).Scale, Content(child).Scale);
        }

        public Vector3 ChangePosition(EFAvatarID avatar, EFAvatarID child)
        {
            return Content(avatar).Position - Divid(Content(child).Position, ChangeSize(child, avatar));
        }

        public static Vector3 Divid(Vector3 vec, Vector3 vec2)
        {
            Vector3 vec3 = Vector3.zero;
            for(int i = 0; i < 3; i++)
                vec3[i] = vec[i] / vec2[i];
            return vec3;
        }
    }

    public enum EFAvatarID
    {
        Unknown = -1,
        None = 0,
    }

    [System.Serializable]
    public struct EFDataContent
    {
        public string ViewName;
        [Header("プレファブ名を検索するときに使用します")]
        public List<string> Names;
        [Header("基準とのサイズの差(LocalScale)")]
        public Vector3 Scale;
        [Header("基準との位置の差(LocalPosition)")]
        public Vector3 Position;
    }
}
