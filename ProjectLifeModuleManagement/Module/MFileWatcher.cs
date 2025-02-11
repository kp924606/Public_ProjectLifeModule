using HolyGift;
using ILogger.AP;
using ILogger.Enum;
using Judgment;
using ProjectLifeModuleManagement.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLifeModuleManagement.Module
{
    /// <summary>
    /// 監控檔案，指定資料夾路徑及檔案名稱
    /// </summary>
    public class MFileWatcher : BAtom
    {
        /**/
        #region Properties

        /// <summary>
        /// 監控檔案物件
        /// FileName	    1	監視檔案名稱變更（檔案重新命名時觸發）
        /// DirectoryName	2	監視目錄名稱變更（目錄重新命名時觸發）
        /// Attributes	    4	監視檔案或目錄屬性變更（如唯讀、隱藏、系統屬性變更）
        /// Size	        8	監視檔案大小變更（當檔案內容變更，導致大小改變時觸發）
        /// LastWrite	   16	監視最後寫入時間變更（檔案內容改變後保存時觸發）
        /// LastAccess	   32	監視最後存取時間變更（檔案被讀取時觸發）
        /// CreationTime   64	監視檔案或目錄的建立時間變更
        /// Security	  256	監視安全性設定變更（如權限修改）
        /// </summary>        
        private FileSystemWatcher? fSWatcher { get; set; }


        /// <summary>
        /// 監控到指定的檔案事件所觸發之函式
        /// </summary>
        private Action<WatcherFileData>? eventBehaviorContent { get; set; }

        /// <summary>
        /// 監控崩潰所觸發之函式
        /// </summary>
        private Action<string?, Exception?> eventBehaviorWatchException { get; set; }

        /// <summary>
        /// 重新複寫, 取得監控物件是否正在運作
        /// </summary>
        protected override bool isRunning
        {
            set
            {
                if (this.fSWatcher == null)
                {
                    value = false;
                }
                else
                {
                    value = this.fSWatcher!.EnableRaisingEvents;
                }
            }

            get
            {
                if (this.fSWatcher == null)
                {
                    return false;
                }
                else
                {
                    return this.fSWatcher!.EnableRaisingEvents;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// 監控檔案，指定資料夾路徑及檔案名稱.
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="fswf">監控到指定的檔案事件所觸發之函式</param>
        /// <param name="fswe">監控崩潰所觸發之函式</param>
        /// <param name="dirpath">監控之資料夾</param>
        /// <param name="filter">監控檔案名稱</param>
        /// <param name="fswnl">監控類型</param>
        /// <param name="incloudsubfolders">監控路徑是否包含子目錄</param>        
        public MFileWatcher(string name, Action<WatcherFileData> fswf, Action<string, Exception> fswe, string dirpath, string filter, List<WatcherChangeTypes> fswnl, bool incloudsubfolders)
            : base(name)
        {
            try
            {
                if (fswf == null)
                {
                    throw new ExpectedInfo($@"Please check Action<WatcherFileData>, EventBehaviorContent is Null", Code.ODI_003);
                }

                if (fswe == null)
                {
                    throw new ExpectedInfo($@"Please check Action<string, Exception>, EventBehaviorWatchException is Null", Code.ODI_003);
                }

                if (string.IsNullOrEmpty(dirpath))
                {
                    throw new ExpectedInfo($@"Please check FSWatcher Folder Path, Data is Null Or Empty", Code.ODI_001);
                }

                if (!Directory.Exists(dirpath))
                {
                    throw new ExpectedInfo($@"Please check FSWatcher Folder Path, Folder Path Don't Exist, Folder:[{dirpath}]", Code.ODI_008);
                }

                if (string.IsNullOrEmpty(filter))
                {
                    throw new ExpectedInfo($@"Please check FSWatcher Filter, Data is Null Or Empty", Code.ODI_001);
                }

                if (fswnl == null)
                {
                    throw new ExpectedInfo($@"Please check Action<WatcherFileData, Exception>, EventBehaviorWatchException is Null", Code.ODI_004);
                }

                eventBehaviorContent = fswf;
                eventBehaviorWatchException = fswe!;
                fSWatcher = new FileSystemWatcher(dirpath);
                fSWatcher.Filter = filter;
                fSWatcher.IncludeSubdirectories = incloudsubfolders;
                fSWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                foreach (var n in fswnl)
                {
                    switch (n)
                    {
                        case WatcherChangeTypes.Deleted:
                            this.fSWatcher.Deleted += Watcher_Event;
                            break;
                        case WatcherChangeTypes.Created:
                            this.fSWatcher.Created += Watcher_Event;
                            break;
                        case WatcherChangeTypes.Changed:
                            this.fSWatcher.Changed += Watcher_Event;
                            break;
                        case WatcherChangeTypes.Renamed:
                            this.fSWatcher.Renamed += Watcher_Event;
                            break;                        
                        default:
                            throw new ExpectedInfo($@"Please check FileSystemWatcherNotify Type Unlnow:[{n.ToString()}]", Code.FCT_004);
                    }
                }
                this.fSWatcher.Error += Watcher_Error;
            }
            catch (ExpectedInfo ex)
            {
                throw new ExpectedInfo($@"[{Name},{GetType().Name},{MethodBase.GetCurrentMethod()!.Name}]:{Key.ExpectedInfo}[{ex}]", ex.ReasonCode);
            }
            catch (Exception ex)
            {
                throw new ExpectedInfo($@"[{Name},{GetType().Name},{MethodBase.GetCurrentMethod()!.Name}]:{Key.Catch}[{ex}]", Code.FCT_002);
            }
            finally
            {
            }
        }
        #endregion


        #region Meth

        /// <summary>
        /// 監控到的動作事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Event(object sender, FileSystemEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var wf = new WatcherFileData();
                    wf.Name = this.Name;
                    wf.FilePath = e.FullPath;
                    wf.Types = e.ChangeType;
                    this.eventBehaviorContent!(wf);
                }
                catch (ExpectedInfo ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                }
                catch (Exception ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_005, ILogType.Catch, ex, null));
                }
                finally
                {
                }
            });
        }

        /// <summary>
        /// 監控發生錯誤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.eventBehaviorWatchException!(this.Name, e.GetException());
                }
                catch (ExpectedInfo ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                }
                catch (Exception ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_005, ILogType.Catch, ex, null));
                }
                finally
                {
                }
            });
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
                    // 監控不為空且還在運作
                    if (this.fSWatcher != null && this.fSWatcher.EnableRaisingEvents)
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"FileSystemWatcher Still Running", Code.IFO_000));
                    }
                    // 監控不為空且尚未運作
                    else if (this.fSWatcher != null && !this.fSWatcher.EnableRaisingEvents)
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));
                        this.fSWatcher.EnableRaisingEvents = true;
                        BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Finish", Code.IFO_000));
                    }
                    else
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"FileSystemWatcher is Null, Not Run", Code.ODI_005, ILogType.Alarm, null, null));
                    }
                }
                catch (ExpectedInfo ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                }
                catch (Exception ex)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_005, ILogType.Catch, ex, null));
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
                // 監控不為空且還在運作
                if (this.fSWatcher != null && this.fSWatcher.EnableRaisingEvents)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));
                    this.fSWatcher.EnableRaisingEvents = false;
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Finish", Code.IFO_000));

                }
                // 監控不為空且尚未運作
                else if (this.fSWatcher != null && !this.fSWatcher.EnableRaisingEvents)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"FileSystemWatcher Not Running, Don't do Interruption, Finish", Code.IFO_000));
                }
                // 監控為空
                else
                {
                    BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"FileSystemWatcher is Null, Don't do Interruption, Finish", Code.IFO_000));
                }
            }
            catch (ExpectedInfo ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
            }
            catch (Exception ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_002, ILogType.Catch, ex, null));
            }
            finally
            {
            }
        }

        public override void Annihilation()
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"...", Code.IFO_000));

                // 監控不為空且還在運作
                if (this.fSWatcher != null && this.fSWatcher.EnableRaisingEvents)
                {
                    this.fSWatcher.EnableRaisingEvents = false;
                }

                this.fSWatcher = null;
                this.eventBehaviorContent = null;
                this.eventBehaviorWatchException = null!;

                SpinWait.SpinUntil(() => false, 1);
                BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Finish", Code.IFO_000));
            }
            catch (ExpectedInfo ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
            }
            catch (Exception ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(Name, GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_002, ILogType.Catch, ex, null));
            }
            finally
            {
            }
        }

        #endregion

        #region Other Class
        public class WatcherFileData
        {
            public string? Name { set; get; }
            public string? FilePath { set; get; }
            public WatcherChangeTypes Types { set; get; }
            public object? obj { set; get; }
        }
        #endregion

    }
}
