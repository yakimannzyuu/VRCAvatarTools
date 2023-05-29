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
            bool toActive = FindUnActive(selectionObj);

            Undo.RecordObjects(selectionObj, _MenuNameChangeEnable);
            SetActives(selectionObj, toActive);
            Undo.RecordObjects(selectionObj, $"{_MenuNameChangeEnable} end");
        }

        // Shift + e : EditorOnlyを切り替える
        [MenuItem(_MenuNameSetEditorOnly)]
        static void SetEditorOnly()
        {
            GameObject[] selectionObj = Selection.gameObjects;
            bool toActive = FindUnActive(selectionObj);

            Undo.RecordObjects(selectionObj, _MenuNameSetEditorOnly);
            SetActives(selectionObj, toActive);
            foreach(GameObject obj in selectionObj)
                obj.tag = toActive ? _Untagged : _EditorOnly;
            Undo.RecordObjects(selectionObj, $"{_MenuNameSetEditorOnly} end");

        }

        [MenuItem( _MenuNameChangeEnable, true )]
        [MenuItem( _MenuNameSetEditorOnly, true )]
        static bool CanChange()
        {
            GameObject[] selectionObj = Selection.gameObjects;
            bool toActive = selectionObj.Length > 0 ? FindUnActive(selectionObj) : true;
            Menu.SetChecked(_MenuNameChangeEnable, !toActive);
            Menu.SetChecked(_MenuNameSetEditorOnly, !toActive);

            var gameObjects = Selection.gameObjects;
            return gameObjects != null && 0 < gameObjects.Length;
        }

        // アクティブかどうかを取得します。falseが優先されます。
        public static bool FindUnActive(GameObject[] objects)
        {
            foreach(GameObject obj in objects)
                if(!obj.activeSelf)
                    return true;
            return false;
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