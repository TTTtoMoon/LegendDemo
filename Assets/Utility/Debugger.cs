using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RogueGods.Utility
{
    /// <summary>
    ///     封装日志输出
    /// </summary>
    public class Debugger
    {
        #region logException

        [Conditional("DEBUGGER")]
        public static void LogException(Exception exception)
        {
#if DEBUGGER
            Debug.LogException(exception);
#endif
        }

        #endregion

        #region log

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEBUGGER")]
        public static void Log(params object[] log)
        {
#if DEBUGGER
        if (log.Length > 1)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < log.Length; i++)
            {
                str.Append(log[i]);
            }
            Debug.Log(str);
        }
        else if (log.Length==1)
        {
            Debug.Log(log[0]);

        }
#endif
        }

        [Conditional("DEBUGGER")]
        public static void Log(UnityEngine.Object obj, params object[] log)
        {
#if DEBUGGER
        if (log.Length > 1)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < log.Length; i++)
            {
                str.Append(log[i]);
            }
            Debug.Log(str, obj);
        }
        else if (log.Length==1)
        {
            Debug.Log(log[0], obj);
        }
#endif
        }

        [Conditional("DEBUGGER")]
        public static void Log(Color color, params object[] log)
        {
#if DEBUGGER
        StringBuilder str = new StringBuilder();
        str.Append("<Color=#");
        str.Append(ColorUtility.ToHtmlStringRGBA(color));
        str.Append(">");
        for (int i = 0; i < log.Length; i++)
        {
            str.Append(log[i]);
        }
        str.Append("</Color>");
        Debug.Log(str);
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogFormat(string log, params object[] args)
        {
#if DEBUGGER
        Debug.LogFormat(log, args);
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogFormat(Color color, string log, params object[] args)
        {
#if DEBUGGER
        StringBuilder str = new StringBuilder();
        str.Append("<Color=#");
        str.Append(ColorUtility.ToHtmlStringRGBA(color));
        str.Append(">");
        str.Append(log);
        str.Append("</Color>");
        Debug.LogFormat(str.ToString(), args);
#endif
        }

        #endregion

        #region logWarning

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEBUGGER")]
        public static void LogWarning(params object[] log)
        {
#if DEBUGGER
        if (log.Length > 1)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < log.Length; i++)
            {
                str.Append(log[i]);
            }

            Debug.LogWarning(str);
        }
        else if (log.Length==1)
        {
            Debug.LogWarning(log[0]);
        }
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogWarning(UnityEngine.Object obj, params object[] log)
        {
#if DEBUGGER
        if (log.Length > 1)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < log.Length; i++)
            {
                str.Append(log[i]);
            }

            Debug.LogWarning(str, obj);
        }
        else if (log.Length==1)
        {
            Debug.LogWarning(log[0], obj);
        }
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogWarning(Color color, params object[] log)
        {
#if DEBUGGER
        StringBuilder str = new StringBuilder();
        str.Append("<Color=#");
        str.Append(ColorUtility.ToHtmlStringRGBA(color));
        str.Append(">");
        for (int i = 0; i < log.Length; i++)
        {
            str.Append(log[i]);
        }
        str.Append("</Color>");
        Debug.LogWarning(str);
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogWarningFormat(string log, params object[] args)
        {
#if DEBUGGER
        Debug.LogWarningFormat(log, args);
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogWarningFormat(Color color, string log, params object[] args)
        {
#if DEBUGGER
        StringBuilder str = new StringBuilder();
        str.Append("<Color=#");
        str.Append(ColorUtility.ToHtmlStringRGBA(color));
        str.Append(">");
        str.Append(log);
        str.Append("</Color>");
        Debug.LogWarningFormat(str.ToString(), args);
#endif
        }

        #endregion

        #region logError

        // ReSharper disable Unity.PerformanceAnalysis
        [Conditional("DEBUGGER")]
        public static void LogError(params object[] log)
        {
#if DEBUGGER
        if (log.Length > 1)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < log.Length; i++)
            {
                str.Append(log[i]);
            }

            Debug.LogError(str);
        }
        else if (log.Length==1)
        {
            Debug.LogError(log[0]);
        }
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogError(UnityEngine.Object obj, params object[] log)
        {
#if DEBUGGER
        if (log.Length > 1)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < log.Length; i++)
            {
                str.Append(log[i]);
            }

            Debug.LogError(str, obj);
        }
        else if (log.Length==1)
        {
            Debug.LogError(log[0], obj);
        }
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogError(Color color, params object[] log)
        {
#if DEBUGGER
        StringBuilder str = new StringBuilder();
        str.Append("<Color=#");
        str.Append(ColorUtility.ToHtmlStringRGBA(color));
        str.Append(">");
        for (int i = 0; i < log.Length; i++)
        {
            str .Append(log[i]);
        }
        str.Append("</Color>");
        Debug.LogError(str);
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogErrorFormat(string log, params object[] args)
        {
#if DEBUGGER
        Debug.LogErrorFormat(log, args);
#endif
        }

        [Conditional("DEBUGGER")]
        public static void LogErrorFormat(Color color, string log, params object[] args)
        {
#if DEBUGGER
        StringBuilder str = new StringBuilder();
        str.Append("<Color=#");
        str.Append(ColorUtility.ToHtmlStringRGBA(color));
        str.Append(">");
        str.Append(log);
        str.Append("</Color>");
        Debug.LogErrorFormat(str.ToString(), args);
#endif
        }

        #endregion
    }

}
