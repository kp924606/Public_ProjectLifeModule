using HolyGift;
using ILogger.AP;
using ILogger.Enum;
using Judgment;
using ProjectLifeModuleManagement.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ProjectLifeModuleManagement.Module
{
    public class MRightNow : BAtom
    {
        /**/
        #region Properties
        /// <summary>
        /// 生命或物件的動作做執行.
        /// </summary>
        private Action<object>? eventBehaviorContent { get; set; }

        /// <summary>
        /// 生命或物件的動作執行完成.
        /// </summary>
        private Action<object>? eventBehaviorFinish { get; set; }

        /// <summary>
        /// 重新複寫, 判斷是否正在運作
        /// </summary>
        protected override bool isRunning
        {
            set
            {
                if (this.threadBehavior == null)
                {
                    value = false;
                }
                else
                {
                    value = this.threadBehavior!.IsAlive;
                }
            }

            get
            {
                if (this.threadBehavior == null)
                {
                    return false;
                }
                else
                {
                    return this.threadBehavior!.IsAlive;
                }
            }
        }
        #endregion

        /**/
        #region Constructor
        public MRightNow(string name, Action<object> astart, Action<object> afinish)
            : base(name)
        {
            try
            {
                if (astart == null)
                {
                    throw new ExpectedInfo($@"Please check Action<object>, EventBehaviorContent is Null", Code.ODI_003);
                }

                if (afinish == null)
                {
                    throw new ExpectedInfo($@"Please check Action<object>, EventBehaviorFinish is Null", Code.ODI_003);
                }

                this.eventBehaviorContent = astart;
                this.eventBehaviorFinish = afinish;
            }
            catch (ExpectedInfo ex)
            {
                throw new ExpectedInfo($@"[{this.Name},{this.GetType().Name},{MethodBase.GetCurrentMethod()!.Name}]:{Key.ExpectedInfo}[{ex}]", ex.ReasonCode);
            }
            catch (Exception ex)
            {
                throw new ExpectedInfo($@"[{this.Name},{this.GetType().Name},{MethodBase.GetCurrentMethod()!.Name}]:{Key.Catch}[{ex}]", Code.FCT_002);
            }
            finally
            { 
            }
        }
        #endregion

        /**/
        #region override from Interfaces(BAtom)        
        public override void Start()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // 指定的動作及行為的執行緒還在執行
                    if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Thread Still Running", Code.IFO_000));
                    }
                    // 指定的動作設定為空
                    else if (this.eventBehaviorContent == null || this.eventBehaviorFinish == null)
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"EventBehavior is Null", Code.ODI_003, ILogType.Alarm, null, null));
                    }
                    else
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                        this.threadBehavior = new Thread(() =>
                        {
                            try
                            {
                                this.eventBehaviorContent(this.Name);
                                this.eventBehaviorFinish(this.Name);
                            }
                            catch (ExpectedInfo ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                            }
                            catch (Exception ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_007, ILogType.Catch, ex, null));
                            }
                            finally
                            {
                            }
                            //SpinWait.SpinUntil(() => false, 1);
                        });
                        this.threadBehavior.Start();
                    }
                }
                catch (ExpectedInfo ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                }
                catch (Exception ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_005, ILogType.Catch, ex, null));
                }
                finally
                {
                }
            });
        }

        public override void Interruption()
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                // 指定的動作及行為的執行緒還在執行
                if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                {
                    this.threadBehavior.Interrupt();
                    SpinWait.SpinUntil(() => false, 1);
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Finish", Code.IFO_000));
                }
                // 指定的動作及行為的執行緒未在執行
                else
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Thread Not Running, Don't do Interruption, Finish", Code.IFO_000));
                }
            }
            catch (ExpectedInfo ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
            }
            catch (Exception ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_002, ILogType.Catch, ex, null));
            }
            finally
            {
                
            }
        }

        public override void Annihilation()
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                // 指定的動作及行為的執行緒還在執行
                if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                {
                    this.threadBehavior.Interrupt();
                }
                this.eventBehaviorContent = null;
                this.eventBehaviorFinish = null;
                this.threadBehavior = null;
                
                SpinWait.SpinUntil(() => false, 1);
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Finish", Code.IFO_000));
            }
            catch (ExpectedInfo ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
            }
            catch (Exception ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_002, ILogType.Catch, ex, null));
            }
            finally
            {
            }
        }
        #endregion
    }
}
