using UnityEngine;
using UnityEditor;

namespace Yakimannzyuu
{
    public class ToggleEnable : Editor
    {
        const string _MenuNameChangeEnable = "Tools/Yakimannzyuu/ChangeEnable #a";
        const string _MenuNameSetEditorOnly = "Tools/Yakimannzyuu/ChangeEditorOnly #e";
        const string _Untagged = "Untagged";
        const string _EditorOnly = "EditorOnly";
        // アクティブ/非アクティブ化のショートカットを追加します。
        // Shift + a : 非アクティブが含まれている場合はすべてアクティブに
        // Shift + a : すべてアクティブならすべて非アクティブに。
        [MenuItem(_MenuNameChangeEnable)]
        static void ChangeEnable()
        {
            GameObject[] selectionObj = Selection.gameObjects;

            Undo.RecordObjects(selectionObj, _MenuNameChangeEnable);
            SetActives(selectionObj, !FindUnActive(selectionObj));
            Undo.RecordObjects(selectionObj, $"{_MenuNameChangeEnable} end");
        }

        // Shift + e : EditorOnlyを切り替える
        [MenuItem(_MenuNameSetEditorOnly)]
        static void SetEditorOnly()
        {
            GameObject[] selectionObj = Selection.gameObjects;
            bool toActive = !FindUnActive(selectionObj);

            Undo.RecordObjects(selectionObj, _MenuNameSetEditorOnly);
            SetActives(selectionObj, toActive);
            foreach(GameObject obj in selectionObj)
                obj.tag = toActive ? _Untagged : _EditorOnly;
            Undo.RecordObjects(selectionObj, $"{_MenuNameSetEditorOnly} end");
        }

        // アクティブかどうかを取得します。falseが優先されます。
        public static bool FindUnActive(GameObject[] objects)
        {
            foreach(GameObject obj in objects)
                if(!obj.activeSelf)
                    return false;
            return true;
        }

        // オブジェクトのアクティブを設定します。
        public static void SetActives(GameObject[] objects, bool active)
        {
            foreach(GameObject obj in objects)
                if(obj != null)
                    obj.SetActive(active);
        }
    }
}