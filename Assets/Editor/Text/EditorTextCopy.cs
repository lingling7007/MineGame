using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTextCopy
{

    public static void CopyText(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            TextEditor text = new TextEditor();
            text.text = str;
            text.OnFocus();
            text.Copy();
        }
    }
}
