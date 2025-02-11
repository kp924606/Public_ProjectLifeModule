using Judgment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILogger.AP
{
    public class ExpectedInfo : Exception
    {
        #region Property
        public Code ReasonCode { get; set; }

        #endregion

        #region Constructor
        public ExpectedInfo(string? message, Code RC)
            : base(message)
        {
            this.ReasonCode = RC;
        }

        #endregion
    }
}
