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
    /// <summary>
    /// Timer 模組, 可指定要定時觸發的動作
    /// </summary>
    public class MTimer : BAtom
    {
        /**/
        #region Properties

        /// <summary>
        /// 確認超時指定的動作及行為.
        /// </summary>
        protected System.Timers.Timer? timerOut { get; set; }

        /// <summary>
        /// 生命或物件的動作做執行.
        /// </summary>
        private Action<object>? eventBehaviorContent { get; set; }

        /// <summary>
        /// 生命或物件的動作執行完成.
        /// </summary>
        private Action<object>? eventBehaviorFinish { get; set; }

        /// <summary>
        /// Timer 的間隔時間, 每輪流一次動作之間的時間差.
        /// </summary>
        private double interval { get; set; }

        /// <summary>
        /// 判斷確認動作執行是否超時.
        /// </summary>
        private bool isCheckTimeOut { get; set; }

        /// <summary>
        /// 是否馬上執行指定動作
        /// </summary>
        private bool isDoRightNow { get; set; }

        /// <summary>
        /// 重新複寫, 判斷是否正在運作
        /// </summary>
        protected override bool isRunning
        {
            set
            {
                if (this.timerOut == null)
                {
                    value = false;
                }
                else
                {
                    value = this.timerOut!.Enabled;
                }
            }

            get
            {
                if (this.timerOut == null)
                {
                    return false;
                }
                else
                {
                    return this.timerOut!.Enabled;
                }
            }
        }
        #endregion

        /**/
        #region Constructor

        /// <summary>
        /// Timer 模組初始化
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="astart">動作做執行</param>
        /// <param name="afinish">動作執行完成</param>
        /// <param name="interval">間隔時間(s/秒)</param>
        /// <param name="isCheckTimeOut">判斷確認動作執行是否超時</param>
        /// <param name="isDoRightNow">是否馬上執行指定動作</param>
        /// <exception cref="ExpectedInfo"></exception>
        public MTimer(string name, Action<object> astart, Action<object> afinish, double interval, bool isCheckTimeOut, bool isDoRightNow) 
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

                if (interval <= 0)
                {
                    throw new ExpectedInfo($@"Please check Interval, Value <= 0", Code.VDE_003);
                }

                this.eventBehaviorContent = astart;
                this.eventBehaviorFinish = afinish;
                this.interval = interval*1000;
                this.isCheckTimeOut = isCheckTimeOut;
                this.isDoRightNow = isDoRightNow;

                // 啟動計時執行工作
                this.timerOut = new System.Timers.Timer();
                //執行間隔時間,單位為毫秒;
                this.timerOut.Interval = this.interval;
                // 無限輪迴
                this.timerOut.AutoReset = true;
                // Timer 動作
                this.timerOut.Elapsed += new ElapsedEventHandler(this.TimerWork);
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
        #region Method
        /// <summary>
        /// Timer 動作內容
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void TimerWork(object? source, ElapsedEventArgs? e)
        {
            try
            {
                // 動作還在執行
                if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                {
                    // 確認有卡 timeout 超時, 重新開始動作
                    if (this.isCheckTimeOut)
                    {
                        //終止動作
                        this.threadBehavior.Interrupt();

                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer Check Thread is TimeOut, ReTrigger...", Code.IFO_000));

                        // 實作動作, 觸發客製化作法
                        this.threadBehavior = new Thread(() =>
                        {
                            try
                            {
                                this.eventBehaviorContent!(this.Name);
                                this.eventBehaviorFinish!(this.Name);
                                SpinWait.SpinUntil(() => false, 1);
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
                        });
                        this.threadBehavior.Start();
                    }
                    // 持續動作, 等待
                    else
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer Check Thread Still IsAlive...", Code.IFO_000));
                    }
                }
                // 未執行動作, 啟動動作
                else if (this.threadBehavior != null && !this.threadBehavior.IsAlive)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer Check Thread Do Trigger...", Code.IFO_000));

                    // 實作動作, 觸發客製化作法
                    this.threadBehavior = new Thread(() =>
                    {
                        try
                        {
                            this.eventBehaviorContent!(this.Name);
                            this.eventBehaviorFinish!(this.Name);
                            SpinWait.SpinUntil(() => false, 1);
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
                    });

                    // 執行緒啟動
                    this.threadBehavior.Start();
                }
                // 執行緒為空
                else
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer Check Thread init and do Trigger...", Code.IFO_000));

                    // 實作動作, 觸發客製化作法
                    this.threadBehavior = new Thread(() =>
                    {
                        try
                        {
                            this.eventBehaviorContent!(this.Name);
                            this.eventBehaviorFinish!(this.Name);
                            SpinWait.SpinUntil(() => false, 1);
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
                    });
                    // 執行緒啟動
                    this.threadBehavior.Start();
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
        #endregion

        /**/
        #region override from Interfaces(BAtom)
        public override void Annihilation()
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                // 停止Timer
                if (this.timerOut != null)
                {
                    this.timerOut.Stop();
                    this.timerOut.Close();
                    this.timerOut.Dispose();
                }

                // 執行續還在執行, 將停止
                if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                {
                    this.threadBehavior.Interrupt();
                }
                this.eventBehaviorContent = null;
                this.eventBehaviorFinish = null;
                this.threadBehavior = null;
                this.timerOut = null;

                this.interval = 0;
                this.isCheckTimeOut = false;
                this.isDoRightNow = false;

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

        public override void Interruption()
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                // 停止 Timer
                if (this.timerOut != null)
                {
                    this.timerOut.Stop();

                    // 執行續還在執行, 將停止
                    if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                    {
                        this.threadBehavior.Interrupt();

                        SpinWait.SpinUntil(() => false, 1);
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer & Thread do Stop, Interruption_1 Finish", Code.IFO_000));
                    }
                    else
                    {
                        SpinWait.SpinUntil(() => false, 1);
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer do Stop, Thread Not Run, Interruption_2 Finish", Code.IFO_000));
                    }
                }
                else
                {
                    // 執行續還在執行, 將停止
                    if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                    {
                        this.threadBehavior.Interrupt();
                        SpinWait.SpinUntil(() => false, 1);
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer Not Running, Thread do Interrupt, Interruption_3 Finish", Code.IFO_000));
                    }
                    else
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Timer & Thread Not Running, Interruption_4 Finish", Code.IFO_000));
                    }
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

        public override void Start()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // 執行緒還在執行
                    if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Thread Still Running", Code.IFO_000));
                    }
                    // 動作設定等為空
                    else if (this.eventBehaviorContent == null || this.eventBehaviorFinish == null)
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"EventBehavior is Null", Code.ODI_003, ILogType.Alarm, null, null));
                    }
                    else
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Start...", Code.IFO_000));

                        if (this.isDoRightNow)
                        {
                            this.TimerWork(null, null);
                        }
                        this.timerOut!.Start();
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
        #endregion
    }
}
