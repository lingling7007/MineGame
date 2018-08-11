using UnityEditor;
using UnityEngine;

public class EditorToolsMenu
{
    [MenuItem("Tools/Test/TestInspector", false, 5)]
    public static void TestInspector()
    {
        EditorWindow.GetWindow<TestInspector>(false, "TestInspector").Show();
    }
    [MenuItem("Assets/CopyItemPath")]
    public static void CopyItemPath()
    {

        Object select = Selection.activeObject;
        if (select == null)
        {
            return;
        }
        var path = AssetDatabase.GetAssetPath(select);
        EditorTextCopy.CopyText(path);
        Debug.LogError(path);

    }



}
