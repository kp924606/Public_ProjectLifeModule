using HolyGift;
using ILogger.AP;
using ILogger.Interface;
using Judgment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace ProjectLifeModuleManagement.Base
{
    public class BAtom : BHiggsBoson
    {
        #region Properties
        /// <summary>
        /// 名稱.
        /// </summary>
        private string name { get; set; }

        /// <summary>
        /// 名稱.
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// 指定的動作及行為是否正在執行.
        /// </summary>
        protected virtual bool isRunning { get; set; }

        /// <summary>
        /// 指定的動作及行為是否正在執行.
        /// </summary>
        public bool IsRunning { get { return this.isRunning; } }

        /// <summary>
        /// 指定的動作及行為的執行緒(執行排程工作的實體).
        /// </summary>
        protected Thread? threadBehavior { get; set; }              

        #endregion

        /**/
        #region Constructor
        public BAtom(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ExpectedInfo($@"Please check Name, Data is Null Or Empty", Code.ODI_001);
                }

                this.name = name;
                this.threadBehavior = null;                
            }
            catch (ExpectedInfo ex)
            {
                throw new ExpectedInfo($@"[{this.GetType().Name},{MethodBase.GetCurrentMethod()!.Name}]:{Key.ExpectedInfo}[{ex}]", ex.ReasonCode);
            }
            catch (Exception ex)
            {
                throw new ExpectedInfo($@"[{this.GetType().Name},{MethodBase.GetCurrentMethod()!.Name}]:{Key.Catch}[{ex}]", Code.FCT_002);
            }
            finally
            {
            }
        }
        #endregion

        /**/
        #region override from Interfaces(BHiggsBoson)
        public override void Annihilation()
        {
        }

        //public override void Init()
        //{
        //}

        public override void Interruption()
        {
        }

        public override void Start()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
