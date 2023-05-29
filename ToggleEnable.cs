using UnityEngine;
using UnityEditor;

namespace Yakimannzyuu
{
    public class ToggleEnable : Editor
    {
        const string MenuNameChangeEnable = "Tools/Yakimannzyuu/ChangeEnable &a";
        const string MenuNameSetEditorOnly = "Tools/Yakimannzyuu/ChangeEditorOnly &e";
        const string Untagged = "Untagged";
        const string EditorOnly = "EditorOnly";
        // アクティブ/非アクティブ化のショートカットを追加します。
        // Shift + A:非アクティブが含まれている場合はすべてアクティブに
        // Shift + A:すべてアクティブならすべて非アクティブに。
        [MenuItem(MenuNameChangeEnable)]
        static void ChangeEnable()
        {
            bool toActive = false;
            GameObject[] selectionObj = Selection.gameObjects;
            foreach(GameObject obj in selectionObj)
            {
                if(!obj.activeSelf)
                {
                    toActive = true;
                    break;
                }
            }
            Undo.RecordObjects(selectionObj, "ChangeEnable");
            foreach(GameObject obj in selectionObj)
            {
                obj.SetActive(toActive);
            }
            Undo.RecordObjects(selectionObj, "ChangeEnable");
        }

        // EditorOnlyを切り替える
        [MenuItem(MenuNameSetEditorOnly)]
        static void SetEditorOnly()
        {
            bool toActive = false;
            GameObject[] selectionObj = Selection.gameObjects;
            foreach(GameObject obj in selectionObj)
            {
                if(!obj.activeSelf)
                {
                    toActive = true;
                    break;
                }
            }
            Undo.RecordObjects(selectionObj, "ChangeTagEditorOnly");
            foreach(GameObject obj in selectionObj)
            {
                obj.SetActive(toActive);
                obj.tag = toActive ? Untagged : EditorOnly;
            }
            Undo.RecordObjects(selectionObj, "ChangeTagEditorOnly");
        }
    }
}