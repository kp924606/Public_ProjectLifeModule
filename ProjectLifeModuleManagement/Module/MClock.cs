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
using TCTUtility.Function;

namespace ProjectLifeModuleManagement.Module
{
    /// <summary>
    /// Timer 模組, 可指定要定時觸發的動作
    /// </summary>
    public class MClock : BAtom
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


        private List<string>? hhddList { get; set; }

        /// <summary>
        /// 判斷確認動作執行是否超時.
        /// </summary>
        private bool isCheckTimeOut { get; set; }

        /// <summary>
        /// 是否馬上執行指定動作
        /// </summary>
        private bool isDoRightNow { get; set; }

        /// <summary>
        /// 紀錄已完成過馬上執行
        /// </summary>
        private bool isDoRightNowFinish { get; set; }

        /// <summary>
        /// 指定終止
        /// </summary>
        private bool isInterruption { get; set; }

        /// <summary>
        /// 任務取消標記來源
        /// </summary>
        private CancellationTokenSource? cts { get; set; }

        /// <summary>
        /// 任務, 等待
        /// </summary>
        private Task? delayTask { get; set; }

        /// <summary>
        /// 重新複寫, 判斷是否正在運作
        /// </summary>
        protected override bool isRunning
        {
            set
            {
                if (this.isInterruption)
                {
                    value = false;
                }
                else
                {
                    value = true;
                }
            }

            get
            {
                if (this.isInterruption)
                {
                    return false;
                }
                else
                {
                    return true;
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
        /// <param name="hml">@"08:00",時間清單(HH:mm)</param>
        /// <param name="isCheckTimeOut">判斷確認動作執行是否超時</param>
        /// <param name="isDoRightNow">是否馬上執行指定動作</param>
        /// <exception cref="ExpectedInfo"></exception>
        public MClock(string name, Action<object> astart, Action<object> afinish, List<string> hml, bool isCheckTimeOut, bool isDoRightNow) 
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

                if (hml == null || hml.Count <= 0)
                {
                    throw new ExpectedInfo($@"Please check List<string>, hhddList is Null or hhddList Count <= 0 ", Code.ODI_004);
                }

                string hmlM = string.Empty;
                if (!FUtility.ValidateTimeFormatForHHmm(hml, out hmlM))
                {
                    throw new ExpectedInfo(hmlM, Code.ODI_010);
                }
                //移除重複元素
                hml = hml.Distinct().ToList();

                this.eventBehaviorContent = astart;
                this.eventBehaviorFinish = afinish;

                this.hhddList = hml;
                //排序, 越早的越前面
                this.hhddList!.Sort((a, b) => TimeSpan.Parse(a).CompareTo(TimeSpan.Parse(b)));

                this.isCheckTimeOut = isCheckTimeOut;
                this.isDoRightNow = isDoRightNow;
                this.cts = null;
                this.delayTask = null;
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
        
        #endregion

        /**/
        #region override from Interfaces(BAtom)
        public override void Annihilation()
        {
            try
            {
                //123
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                this.isInterruption = true;
                // 任務標記, 做終止
                if (this.cts != null)
                {
                    this.cts.Cancel();
                    this.cts = null;
                    this.delayTask = null;
                }

                // 執行續還在執行, 將停止
                if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                {
                    this.threadBehavior.Interrupt();
                }
                this.eventBehaviorContent = null;
                this.eventBehaviorFinish = null;
                this.threadBehavior = null;
               
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
                //123
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                this.isInterruption = true;

                // 任務標記, 做終止
                if (this.cts != null)
                {
                    this.cts.Cancel();
                }
                // 執行續還在執行, 將停止
                if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                {
                    this.threadBehavior.Interrupt();

                    SpinWait.SpinUntil(() => false, 1);
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Thread do Stop, Interruption_1 Finish", Code.IFO_000));
                }
                else
                {
                    SpinWait.SpinUntil(() => false, 1);
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Thread Not Run, Interruption_2 Finish", Code.IFO_000));
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

        //public override void Start()
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        try
        //        {
        //            // 執行緒還在執行
        //            if (this.threadBehavior != null && this.threadBehavior.IsAlive)
        //            {
        //                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Thread Still Running", Code.IFO_000));
        //            }
        //            // 動作設定等為空
        //            else if (this.eventBehaviorContent == null || this.eventBehaviorFinish == null)
        //            {
        //                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"EventBehavior is Null", Code.ODI_003, ILogType.Alarm, null, null));
        //            }
        //            else
        //            {
        //                this.isInterruption = false;

        //                this.threadBehavior = new Thread(() =>
        //                {
        //                    while (!this.isInterruption)
        //                    {
        //                        try
        //                        {
        //                            // 馬上執行
        //                            if (this.isDoRightNow && !this.isDoRightNowFinish)
        //                            {
        //                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"..., DoRightNow", Code.IFO_000));
        //                                this.isDoRightNowFinish = true;
        //                            }
        //                            // 等待指定的下一輪時間
        //                            else
        //                            {
        //                                DateTime now = DateTime.Now;
        //                                // 指定觸發時間 @"08:00", @"09:00", @"10:00", @"14:00", @"20:00"
        //                                // Now:{2025/2/5 下午 05:11:32}
        //                                // 最近目標時間:{2025/2/5 下午 08:00:00}
        //                                // 
        //                                //DateTime? nextTrigger = this.hhddList!.Select(time => DateTime.Today.Add(TimeSpan.Parse(time)))
        //                                //.Where(t => t > now) // 過濾掉已經過去的時間
        //                                //.OrderBy(t => t) // 找出最近的時間點
        //                                //.FirstOrDefault();

        //                                DateTime? nextTrigger = null;
        //                                var nextTriggerL = this.hhddList!.Select(time => DateTime.Today.Add(TimeSpan.Parse(time)))
        //                                .Where(t => t > now) // 過濾掉已經過去的時間
        //                                .OrderBy(t => t) // 找出最近的時間點
        //                                .ToList();

        //                                if (nextTriggerL == null || nextTriggerL.Count <= 0)
        //                                {
        //                                    nextTrigger = DateTime.Today.AddDays(1).Add(TimeSpan.Parse(this.hhddList!.FirstOrDefault()!));
        //                                }
        //                                else
        //                                {
        //                                    nextTrigger = nextTriggerL.FirstOrDefault();
        //                                }
        //                                TimeSpan delay = nextTrigger.Value - now;
        //                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"..., Wait:[{delay.TotalSeconds}]", Code.IFO_000));

        //                                // 建立 CancellationTokenSource
        //                                this.cts = new CancellationTokenSource();
        //                                // 等待到指定時間
        //                                this.delayTask = Task.Delay(delay, cts.Token);
        //                                this.delayTask.Wait();
        //                            }

        //                            this.eventBehaviorContent!(this.Name);
        //                            this.eventBehaviorFinish!(this.Name);
        //                            SpinWait.SpinUntil(() => false, 1);
        //                        }
        //                        catch (TaskCanceledException ex)
        //                        {
        //                            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, Code.FCT_010, ILogType.Error, ex, null));
        //                        }
        //                        catch (ExpectedInfo ex)
        //                        {
        //                            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_006, ILogType.Catch, ex, null));
        //                        }
        //                        finally
        //                        {
        //                        }
        //                    }
        //                });
        //                this.threadBehavior.Start();
        //            }
        //        }
        //        catch (ExpectedInfo ex)
        //        {
        //            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
        //        }
        //        catch (Exception ex)
        //        {
        //            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_005, ILogType.Catch, ex, null));                    
        //        }
        //        finally
        //        {
        //        }
        //    });
        //}

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
                        this.isInterruption = false;
                        while (!this.isInterruption)
                        {
                            try
                            {
                                // 馬上執行
                                if (this.isDoRightNow && !this.isDoRightNowFinish)
                                {
                                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"..., DoRightNow", Code.IFO_000));
                                    this.isDoRightNowFinish = true;
                                }
                                // 等待指定的下一輪時間
                                else
                                {
                                    DateTime now = DateTime.Now;
                                    // 指定觸發時間 @"08:00", @"09:00", @"10:00", @"14:00", @"20:00"
                                    // Now:{2025/2/5 下午 05:11:32}
                                    // 最近目標時間:{2025/2/5 下午 08:00:00}
                                    
                                    DateTime? nextTrigger = null;
                                    var nextTriggerL = this.hhddList!.Select(time => DateTime.Today.Add(TimeSpan.Parse(time)))
                                    .Where(t => t > now) // 過濾掉已經過去的時間
                                    .OrderBy(t => t) // 找出最近的時間點
                                    .ToList();

                                    if (nextTriggerL == null || nextTriggerL.Count <= 0)
                                    {
                                        nextTrigger = DateTime.Today.AddDays(1).Add(TimeSpan.Parse(this.hhddList!.FirstOrDefault()!));
                                    }
                                    else
                                    {
                                        nextTrigger = nextTriggerL.FirstOrDefault();
                                    }
                                    TimeSpan delay = nextTrigger.Value - now;
                                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"..., Wait:[{delay.TotalSeconds}]/s", Code.IFO_000));

                                    // 建立 CancellationTokenSource
                                    this.cts = new CancellationTokenSource();
                                    // 等待到指定時間
                                    this.delayTask = Task.Delay(delay, cts.Token);
                                    this.delayTask.Wait();
                                }

                                if (this.isCheckTimeOut)
                                {
                                    // 執行緒還在執行
                                    if (this.threadBehavior != null && this.threadBehavior.IsAlive)
                                    {
                                        this.threadBehavior.Interrupt();
                                    }

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
                                // 進行下一個時程
                                else
                                { 
                                
                                }                                                              
                            }
                            catch (AggregateException ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, Code.FCT_010, ILogType.Error, ex, null));
                            }                            
                            catch (ExpectedInfo ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                            }
                            catch (Exception ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_006, ILogType.Catch, ex, null));
                            }
                            finally
                            {
                            }
                        }
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
