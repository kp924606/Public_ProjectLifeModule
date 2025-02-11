using ILogger.Enum;
using ILogger.Interface;
using Judgment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILogger.AP
{
    /// <summary>
    /// 紀錄 Log 物件.
    /// </summary>
    public class LogInfo : ILogInfo
    {
        #region Implement Interfaces
        public ILogType Type { get; set; }

        public string? Name { get; set; }

        public string? Class { get; set; }
        public string? Method { get; set; }
        public string? Info { get; set; }
        public Exception? Error { get; set; }
        public object? obj { get; set; }
        public Code ResultCode { get; set; }
        #endregion

        /**/
        #region Constructor
        //public LogInfo(string cs, string mt, string info, ILogType type = ILogType.Info, Exception? error = null, object? obj = null)
        //{
        //    this.Class = cs;
        //    this.Method = mt;
        //    this.Info = info;
        //    this.Type = type;
        //    this.Error = error;
        //    this.obj = obj;
        //}

        public LogInfo(string na, string cs, string mt, string info, Code rc)
        {
            this.Name = na;
            this.Class = cs;
            this.Method = mt;
            this.Info = info;
            this.ResultCode = rc;
            this.Type = ILogType.Info;
            this.Error = null;
            this.obj = null;
        }

        //public LogInfo(string cs, string mt, string info, Code rc, ILogType type = ILogType.Info)
        //{
        //    this.Class = cs;
        //    this.Method = mt;
        //    this.Info = info;
        //    this.Type = type;
        //    this.ResultCode = rc;
        //    this.Error = null;
        //    this.obj = null;           
        //}

        public LogInfo(string na, string cs, string mt, string info, Code rc, ILogType type = ILogType.Info, Exception? error = null, object? obj = null)
        {
            this.Name = na;
            this.Class = cs;
            this.Method = mt;
            this.Info = info;
            this.Type = type;
            this.ResultCode = rc;
            this.Error = error;
            this.obj = obj;
        }
        #endregion
    }
}
