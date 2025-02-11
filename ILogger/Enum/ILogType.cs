using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILogger.Enum
{
    /// <summary>
    /// 區分 Log 類型.
    /// Distinguish Log type.
    /// </summary>
    public enum ILogType
    {
        /// <summary>
        /// 資訊.
        /// Information
        /// </summary>
        Info,

        /// <summary>
        /// 警報.
        /// Alarm.
        /// </summary>
        Alarm, 

        /// <summary>
        /// 錯誤.
        /// Error.
        /// </summary>
        Error,

        /// <summary>
        /// 失敗.
        /// Fail.
        /// </summary>
        Fail,

        /// <summary>
        /// 崩潰.
        /// Catch
        /// </summary>
        Catch,

        /// <summary>
        /// 成功.
        /// Catch
        /// </summary>
        Pass,
    }
}
