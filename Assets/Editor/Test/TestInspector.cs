using CommonTools;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestInspector : EditorWindow
{
    private void OnGUI()
    {

        GUILayout.Space(10);
        if (GUILayout.Button("XmlToClass"))
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            path = path.Replace('\\', '/');
            if (Path.GetExtension(path) == ".xml")
            {
                XmlToClass xml = new XmlToClass();
                xml.ToClass(path);
                Process.Start("explorer.exe", "E:");
            }
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Test For "))
        {

            

        }

    }
    private void ProcessCommand(string command, string argument = "")
    {
        ProcessStartInfo start = new ProcessStartInfo(command);
        if (!string.IsNullOrEmpty(argument))
        {
            start.Arguments = argument;
        }
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = true;

        if (start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        Process p = Process.Start(start);

        if (!start.UseShellExecute)
        {
            UnityEngine.Debug.Log(p.StandardOutput);
            UnityEngine.Debug.Log(p.StandardError);
        }

        p.WaitForExit();
        p.Close();
    }


    private string GetPathClient()
    {
        return Application.dataPath;

    }
    private string GetPathProduct()
    {
        int idx = Application.dataPath.IndexOf("/sl_client");
        string path = Application.dataPath.Remove(idx) + "/sl_product";
        return path;
    }
    void CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            return;
        }
        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }
        //UnityEngine.Debug.LogError(tarPath + " -----  " + srcPath);
        CopyFile(srcPath, tarPath);
        string[] directionName = Directory.GetDirectories(srcPath);
        foreach (string dirPath in directionName)
        {
            // "//"
            string directionPathTemp = tarPath + dirPath.Substring(srcPath.Length);
            CopyFolder(dirPath, directionPathTemp);
        }
    }
    void CopyFile(string srcPath, string tarPath)
    {
        bool canCopy = true;
        if (srcPath.Contains("effects") || srcPath.Contains("不导出xml"))
        {
            canCopy = false;
        }
        if (canCopy)
        {
            UnityEngine.Debug.Log(srcPath);
            string[] filesList = Directory.GetFiles(srcPath);
            foreach (string f in filesList)
            {
                string fTarPath = tarPath + "\\" + f.Substring(srcPath.Length);
                if (File.Exists(fTarPath))
                {
                    File.Copy(f, fTarPath, true);
                }
                else
                {
                    File.Copy(f, fTarPath);
                }
            }
            return;
        }
    }

}
