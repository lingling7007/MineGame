
using SimpleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonTools
{

    public class StringTools
    {
        /// <summary>
        ///格式化失败，返回原字符串
        /// </summary>
        public static string FormatFailReturnOrigin(string str, params object[] param)
        {
            if (CheckStringFormatParam(str, param))
            {
                for (int cnt = 0; cnt < param.Length; cnt++)
                {
                    str = str.Replace("{" + cnt + "}", param[cnt].ToString());
                }
            }
            else
            {
                LogCtrl.Log(" FormatFailReturnOrigin " + str);
            }
            return str;
        }
        /// <summary>
        /// 代替 string.Format
        /// </summary>
        public static string Format(string str, params object[] param)
        {
            string content = str;
            if (CheckStringFormatParam(content, param))
            {
                for (int cnt = 0; cnt < param.Length; cnt++)
                {

                    content = content.Replace("{" + cnt + "}", param[cnt].ToString());
                }
                return content;
            }
            Debug.LogError("  Format Error   " + content + "   Param Length: " + param.Length);
            return str;
        }

        public static bool CheckStringFormatParam(string str, params object[] param)
        {
            if (param.Length == 0)
            {
                return false;
            }
            for (int cnt = 0; cnt < param.Length; cnt++)
            {
                if (param[cnt] == null)
                {
                    return false;
                }
            }
            int idx = 0;
            for (int cnt = 0; cnt < str.Length; cnt++)
            {
                if (str[cnt].ToString() == "{")
                {
                    if (cnt + 2 < str.Length)
                    {
                        if (str[cnt + 1].ToString() == idx.ToString() && str[cnt + 2].ToString() == "}")
                        {
                            idx++;
                            cnt += 2;
                        }
                    }
                }
            }
            if (idx != param.Length)
            {
                return false;
            }
            return true;
        }
    }

}
