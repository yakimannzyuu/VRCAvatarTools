using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Yakimannzyuu
{
    // EFアバター改変用ツール
    // 2023年5月15日
    public class EFToolMenu : EditorWindow
    {
        public EFAvatarData AvatarData;
        [MenuItem("Tools/Yakimannzyuu/EFAvatarToolMenu %e")]
        static void OpenWindow()
        {
            GetWindow<EFToolMenu>("EFAvatarToolMenu");
        }
        
        public GameObject TargetAvatar;
        public GameObject ChildObject;
        public bool FixAnchorOverride;
        public bool FixPB;

        public const string EDITOR_ONLY = "EditorOnly";

        void OnGUI()
        {
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            // データファイル
            EditorGUILayout.LabelField("EFAvatarData");
            SerializedProperty property = so.FindProperty("AvatarData");
            EditorGUILayout.PropertyField(property, true); // True means show children
            so.ApplyModifiedProperties();

            if(AvatarData == null)
                return;
            // 変更前を選択
            property = null;
            property = so.FindProperty("TargetAvatar");
            EditorGUILayout.PropertyField(property, true);
            EFAvatarID TargetID = EFAvatarID.Unknown;
            if(TargetAvatar != null)
            {
                TargetID = AvatarData.FindID(TargetAvatar.name);
                EditorGUILayout.LabelField($"データ：{AvatarData.Content(TargetID).ViewName}");
            }
            so.ApplyModifiedProperties();
            if(TargetAvatar == null)
                return;

            property = null;
            property = so.FindProperty("ChildObject");
            EditorGUILayout.PropertyField(property, true);
            EFAvatarID ChildID = EFAvatarID.Unknown;
            if(ChildObject != null)
            {
                ChildID = AvatarData.FindID(ChildObject.name);
                EditorGUILayout.LabelField($"データ：{AvatarData.Content(ChildID).ViewName}");

            }
            so.ApplyModifiedProperties();

            if(ChildObject == null)
                return;
            // 状態を表示
            EditorGUILayout.LabelField($"衣装 : {AvatarData.Content(ChildID).ViewName}  > アバター : {AvatarData.Content(TargetID).ViewName}");
            EditorGUILayout.LabelField($"Size.y : {AvatarData.Content(TargetID).Scale.y} ÷ {AvatarData.Content(ChildID).Scale.y} = {AvatarData.ChangeSize(TargetID, ChildID).y}");
            EditorGUILayout.LabelField($"Position.y : {AvatarData.Content(TargetID).Position.y} - {AvatarData.Content(ChildID).Position.y} ÷ Size = {AvatarData.ChangePosition(TargetID, ChildID).y}");
            
            EditorGUILayout.LabelField($"AnchorOverrideを統合します。");
            property = so.FindProperty("FixAnchorOverride");
            EditorGUILayout.PropertyField(property, true);
            EditorGUILayout.LabelField($"胸PB、尻PBが貫通しないように、EditorOnly化とignore追加をします。");
            property = so.FindProperty("FixPB");
            EditorGUILayout.PropertyField(property, true);



            if( GUI.Button ( new Rect( 0.0f, 300.0f, 120.0f, 20.0f), "コピーを生成する。") )
                GenerateCopyAvatar();

            GUI_BoneTransport();
            so.ApplyModifiedProperties();
        }

        void GenerateCopyAvatar()
        {
            if(TargetAvatar == null || ChildObject == null)
                return;

            GameObject NewAvatar = Instantiate(TargetAvatar);
            GameObject NewChildObject = Instantiate(ChildObject, NewAvatar.transform);

            NewAvatar.name = TargetAvatar.name;
            NewChildObject.transform.localScale = AvatarData.ChangeSize(AvatarData.FindID(NewAvatar.name), AvatarData.FindID(ChildObject.name));
            NewChildObject.transform.localPosition = AvatarData.ChangePosition(AvatarData.FindID(NewAvatar.name), AvatarData.FindID(ChildObject.name));
            NewChildObject.name = ChildObject.name;

            if(FixAnchorOverride)
                ChangeAnchorOverride(NewAvatar.transform, NewChildObject.transform);
            if(FixPB)
            {
                AddIgnorePB(NewAvatar, NewChildObject);
                UnenablePB(NewChildObject.transform);
            }

            TargetAvatar.SetActive(false);
            NewAvatar.SetActive(true);
            TargetAvatar = NewAvatar;
            ChildObject.SetActive(false);
            NewChildObject.SetActive(true);
            ChildObject = NewChildObject;
        }

        void ChangeAnchorOverride(Transform target, Transform child, string name = "AnchorOverride")
        {
            var targetTransform = target.Find(name);
            var childTransform = child.Find(name);
            if(targetTransform == null)
            {
                Log("AnchorOverrideが見つかりませんでした。");
                return;
            }

            var renderers = child.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach(SkinnedMeshRenderer renderer in renderers)
                renderer.probeAnchor = targetTransform;
            
            childTransform.SetParent(targetTransform);
            ActiveObject(childTransform.gameObject);
        }

        void UnenablePB(Transform child, string name = "PhysBone")
        {
            foreach(Transform tra in FindContainsObject(child, name))
            {
                for(int i = 0; i < tra.childCount; i++)
                {
                    var PBs = tra.GetChild(i);
                    foreach(string str in AvatarData.UnenablePB)
                    {
                        if(PBs.name == str)
                        {
                            ActiveObject(PBs.gameObject);
                            break;
                        }
                    }
                }
            }
        }

        void AddIgnorePB(GameObject target, GameObject child)
        {
            var targetPBs = PhysBoneTool.GetAllPhysBones(target);
            var childPBs = PhysBoneTool.GetAllPhysBones(child);
            foreach(var targetPB in targetPBs)
            {
                foreach(var childPB in childPBs)
                {
                    Log($"{targetPB.name} == {childPB.name} : {targetPB.name == childPB.name}");
                    if(targetPB.name == childPB.name)
                    {
                        PhysBoneTool.AllChildrenIgnorePB(targetPB, childPB.rootTransform);
                    }
                }
            }
        }

        //
        void GUI_BoneTransport()
        {
            if(ChildObject.transform.parent == TargetAvatar.transform)
            {
                Transform avatarArmature = TargetAvatar.transform.Find("Armature");
                Transform childArmature = ChildObject.transform.Find("Armature");
                if(avatarArmature && childArmature)
                    if( GUI.Button ( new Rect( 0.0f, 340.0f, 120.0f, 20.0f), "ボーンを転送する。") )
                        BoneTransport(avatarArmature, childArmature, $"_{ChildObject.name}");
                
                if(!childArmature)
                    EditorGUILayout.LabelField($"{ChildObject.name}にArmatureが見つかりません。");
                if(!avatarArmature)
                    EditorGUILayout.LabelField($"{TargetAvatar.name}にArmatureが見つかりません。");
            }
        }

        void BoneTransport(Transform avatar, Transform child, string addName)
        {
            new BoneTransporter().BoneTransport(avatar, child, addName);
            ChildObject = null;
        }

        List<Transform> FindContainsObject(Transform obj, string str)
        {
            List<Transform> list = new List<Transform>();
            for(int i = 0; i < obj.childCount; i++)
            {
                if(obj.GetChild(i).name.Contains(str))
                    list.Add(obj.GetChild(i));
            }
            return list;
        }

        void ActiveObject(GameObject obj, bool active = false)
        {
            obj.SetActive(active);
            if(!active)
                obj.tag = EDITOR_ONLY;
        }

        public static void Log(string str, string color = "blue")
        {
            Debug.Log($"<color={color}>{str}</color>");
        }
    }
}
