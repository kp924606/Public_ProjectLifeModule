using HolyGift;
using ILogger.AP;
using ILogger.Enum;
using ILogger.Interface;
using Judgment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLifeModuleManagement.Base
{
    public class BMolecule : BAtom
    {
        /**/
        #region Properties
        /// <summary>
        /// 指定的動作及行為集合
        /// </summary>
        private List<BAtom>? _atomList { get; set; }
        /// <summary>
        /// 指定的動作及行為集合
        /// </summary>
        public List<BAtom> AtomList { get { return this._atomList!; } }
        #endregion

        /**/
        #region Static
        /// <summary>
        /// 生命, 個體的溝通方式
        /// </summary>
        public static Action<ILogInfo>? Communication { get; set; }
        #endregion

        /**/
        #region Constructor
        public BMolecule(string name)
            : base(name)
        {

        }
        #endregion

        /**/
        #region override from Interfaces(BAtom)
        public override void Annihilation()
        {
            try
            {
                if (this._atomList != null)
                {
                    Parallel.ForEach(this._atomList!, ba =>
                    {
                        try
                        {
                            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Annihilation:[{ba.Name}]...", Code.IFO_000));
                            ba.Annihilation();
                        }
                        catch (ExpectedInfo ex)
                        {
                            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                        }
                        catch (Exception ex)
                        {
                            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_001, ILogType.Catch, ex, null));
                        }
                        finally
                        {
                            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Annihilation:[{ba.Name}] Finish", Code.IFO_000));
                        }
                    });
                    this.isRunning = false;
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Annihilation All Finish", Code.IFO_000));
                }
                else
                {
                    throw new ExpectedInfo($@"Please check AtomList, List is Null", Code.ODI_004);
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

        //public override void Init()
        //{
        //}

        public override void Interruption()
        {
            try
            {
                if (this._atomList != null)
                {
                    lock (this._atomList)
                    {
                        Parallel.ForEach(this._atomList!, ba =>
                        {
                            try
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Interruption:[{ba.Name}]...", Code.IFO_000));
                                ba.Interruption();
                            }
                            catch (ExpectedInfo ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                            }
                            catch (Exception ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_001, ILogType.Catch, ex, null));
                            }
                            finally
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Interruption:[{ba.Name}] Finish", Code.IFO_000));
                            }
                        });
                        this.isRunning = false;
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Interruption All Finish", Code.IFO_000));
                    }
                }
                else
                {
                    throw new ExpectedInfo($@"Please check AtomList, List is Null", Code.ODI_004);
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
            try
            {
                if (this._atomList != null)
                {
                    lock (this._atomList)
                    {
                        Parallel.ForEach(this._atomList!, ba =>
                        {
                            try
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Start:[{ba.Name}]...", Code.IFO_000));
                                ba.Start();
                            }
                            catch (ExpectedInfo ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                            }
                            catch (Exception ex)
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_001, ILogType.Catch, ex, null));
                            }
                            finally
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Start:[{ba.Name}] Finish", Code.IFO_000));
                            }
                        });
                        this.isRunning = true;
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Start All Finish", Code.IFO_000));
                    }
                }
                else
                {
                    throw new ExpectedInfo($@"Please check AtomList, List is Null", Code.ODI_004);
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
        #region Method
        /// <summary>
        /// 加入專案, 任務
        /// </summary>
        /// <param name="ij"></param>
        protected void AddProject(BAtom ba)
        {
            try
            {
                if (this._atomList == null)
                {
                    this._atomList = new List<BAtom>();
                }

                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Add:[{ba.Name}]...", Code.IFO_000));
                lock (this._atomList)
                {
                    this._atomList.Add(ba);
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
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Add:[{ba.Name}] Finish", Code.IFO_000));
            }
        }

        #endregion
    }
}
