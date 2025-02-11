using HolyGift;
using ILogger.AP;
using ILogger.Interface;
using Judgment;
using ProjectLifeModuleManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLifeModuleManagement.Base
{
    /// <summary>
    /// 世界萬物皆由正反, 陰陽組合, 取之最小, 最讓我們捉模不透的最底層所組成, 我們對祂既看不到, 也不懂, 只能以抽象等方式描述及定義
    /// </summary>
    public abstract class BHiggsBoson : IBehavior
    {
        /**/
        #region Properties
        #endregion

        /**/
        #region Constructor       
        #endregion

        /**/
        #region virtual from Interfaces(IBehavior)
        public abstract void Annihilation();
        //public abstract void Init();
        public abstract void Interruption();
        public abstract void Start();
        #endregion
    }
}
