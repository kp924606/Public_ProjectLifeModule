using Dapper;
using HolyGift;
using ILogger.AP;
using ILogger.Enum;
using Judgment;
using Microsoft.Data.SqlClient;
using ProjectLifeModuleManagement.Base;
using ProjectLifeModuleManagement.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCTUtility.Function;
using static ProjectLifeModuleManagement.Module.MFileWatcher;

namespace ProjectLifeModuleCase.Case
{
    /// <summary>
    /// 測試良率分析
    /// </summary>
    public class TW_TC_Advan_BICS9QLC_TYieldAnalyze_1 : BMolecule
    {
        #region Static
        private const string regexFilePattern = @"(?<=^|\n)([A-Za-z0-9]+)=(.*?)(?=\n|$)";
        private const int saveDBCountLimit = 1;
        private const int saveDBIntervalLimit = 20;
        private const string SQLInsertOrUpdateReportInfoToDB = @"
DECLARE @GetSqlCount int
SELECT @GetSqlCount = COUNT([FilePath])
FROM [Test].[dbo].[PDReportInfo]
where [FilePath] = @FilePath

--完全沒資料，新增
IF(@GetSqlCount = 0)
	BEGIN
        BEGIN TRANSACTION
	    --insert
        INSERT INTO [Test].[dbo].[PDReportInfo] (
        [Machine]
        ,[LotID]
        ,[Step]
        ,[Status]
        ,[TestMode]
        ,[Package]
        ,[Device]
        ,[Program]
        ,[CustomerID]
        ,[SerialNumber]
        ,[Operator]
        ,[UseTime]
        ,[TestTime]
        ,[TestStatus]
        ,[Temperature]
        ,[StartTime]
        ,[EndTime]
        ,[FilePath]
        ,[FileDate]
        )        
        VALUES(@Machine,@LotID,@Step,@Status,@TestMode,@Package,@Device,@Program,@CustomerID,@SerialNumber,@Operator,@UseTime,@TestTime,@TestStatus,@Temperature,@StartTime,@EndTime,@FilePath,@FileDate)
        COMMIT
	END
ELSE
    BEGIN
        BEGIN TRANSACTION
            --update
            UPDATE [Test].[dbo].[PDReportInfo]
            SET [Machine] = @Machine
            ,[LotID] = @LotID
            ,[Step] = @Step
            ,[Status] = @Status
            ,[TestMode] = @TestMode
            ,[Package] = @Package
            ,[Device] = @Device
            ,[Program] = @Program
            ,[CustomerID] = @CustomerID
            ,[SerialNumber] = @SerialNumber
            ,[Operator] = @Operator
            ,[UseTime] = @UseTime
            ,[TestTime] = @TestTime
            ,[TestStatus] = @TestStatus
            ,[Temperature] = @Temperature
            ,[StartTime] = @StartTime
            ,[EndTime] = @EndTime
            ,[FilePath] = @FilePath
            ,[FileDate] = @FileDate
            ,[ModifiedOn] = getdate()           
            Where [FilePath] = @FilePath
        COMMIT
    END
";
        #endregion

        /**/
        #region Properties
        private string? productDataSourcePath { get; set; }
        private string? productDataBackUpPath { get; set; }
        private double scanFileIntervalS { get; set; }
        private string? fileFilter { get; set; }
        private double scanFileStartH { get; set; }
        private double scanFileEndH { get; set; }
        private string? eqpMachineType { get; set; }
        private string? eqpCusterID { get; set; }
        private string? cimAPPTWSDomain { get; set; }
        private string? cimAPPTWSUri { get; set; }
        private double cimAPPTWSTimeOut { get; set; }
        private string? alPDDBConnection { get; set; }

        /// <summary>
        /// 存放來源檔案
        /// </summary>
        private ConcurrentQueue<BaseSourceFile>? sourceFileQ { get; set; }
        private ConcurrentQueue<BaseReportInfo>? reportInfoQ { get; set; }
        private ConcurrentQueue<BaseBackupFile>? backupFileQ { get; set; }

        #endregion

        /**/
        #region Constructor

        public TW_TC_Advan_BICS9QLC_TYieldAnalyze_1(string name, Dictionary<string, object> dic)
            : base(name)
        {
            try
            {
                this.productDataSourcePath = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.ProductDataSourcePath);
                this.productDataBackUpPath = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.ProductDataBackUpPath);
                this.scanFileIntervalS = FUtility.GetDoubleAndCheckNOrEFromDic(dic, Key.ScanFileIntervalS);                
                this.fileFilter = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.FileFilter);
                this.scanFileStartH = FUtility.GetDoubleAndCheckNOrEFromDic(dic, Key.ScanFileStartH);
                this.scanFileEndH = FUtility.GetDoubleAndCheckNOrEFromDic(dic, Key.ScanFileEndH);
                this.eqpMachineType = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.EQPMachineType);
                this.eqpCusterID = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.EQPCusterID);
                this.cimAPPTWSDomain = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.CIMAPPTWSDomain);
                this.cimAPPTWSUri = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.CIMAPPTWSUri);
                this.cimAPPTWSTimeOut = FUtility.GetDoubleAndCheckNOrEFromDic(dic, Key.CIMAPPTWSTimeOut);
                this.alPDDBConnection = FUtility.GetStringAndCheckNOrEFromDic(dic, Key.ALPDDBConnection);

                this.sourceFileQ = new ConcurrentQueue<BaseSourceFile>();
                this.reportInfoQ = new ConcurrentQueue<BaseReportInfo>();
                this.backupFileQ = new ConcurrentQueue<BaseBackupFile>();
#if DEBUG
                this.productDataSourcePath = @"D:\Development\test\temp\PD\TC\Advan\eMMC\Machine1\DeviceName1\";
                this.productDataBackUpPath = @"D:\Development\test\backup\PD\TC\Advan\eMMC\Machine1\DeviceName1\";
                this.scanFileStartH = 999;
                this.alPDDBConnection = @"Data Source=WINDOWS10\MSSQL2019;Initial Catalog=Test;Persist Security Info=True;User ID=test;password=test@LN;Connection Timeout=90;Integrated Security=True;TrustServerCertificate=True";
#else

#endif

                // 掃描指定路徑的檔案, 請動後就先執行, 後續每2小時掃一次
                this.AddProject(new MTimer(@"ScanFile_Timer.1", DoScanFileInFolder, DoScanFileInFolderFinish, this.scanFileIntervalS, true, true));

                // 分析檔案
                this.AddProject(new MInfiniteLoop(@"DoAnalyzeFile_InfiniteLoop.2-1", DoAnalyzeFile, DoAnalyzeFileFinish));
                // 分析檔案
                this.AddProject(new MInfiniteLoop(@"DoAnalyzeFile_InfiniteLoop.2-2", DoAnalyzeFile, DoAnalyzeFileFinish));

                // 將檔案內容存入資料庫
                this.AddProject(new MInfiniteLoop(@"DoSaveReportInfo_InfiniteLoop.3", DoSaveReportInfoToDB, DoSaveReportInfoToDBFinish));

                // 將檔案內轉移至備存資料夾
                this.AddProject(new MInfiniteLoop(@"DoBackupFile_InfiniteLoop.4", DoBackupFile, DoBackupFileFinish));

                //this.hhddList = new List<string>() { @"09:00", @"10:00"};
                //this.AddProject(new MClock(@"test", DoTest, DoTestFinish, new List<string>() { @"09:00", @"10:00", @"08:00", @"20:00", @"14:00", @"17:17" }, true, true));
                //this.AddProject(new MClock(@"test", DoTest, DoTestFinish, new List<string>() { @"09:00", @"10:00", @"08:00", @"18:16", @"18:17", @"18:18", @"18:19" }, true, true));

                this.AddProject(new MClock(@"test", DoTest, DoTestFinish, new List<string>() { @"09:00", @"10:00", @"08:00", @"18:27", @"18:28", @"18:29", @"18:29", @"18:29", @"18:29" }, true, false));

                //MClock(string name, Action<object> astart, Action<object> afinish, List<string> hml, bool isCheckTimeOut, bool isDoRightNow) 
                // 監控檔案
                this.AddProject(new MFileWatcher(@"MFileWatcher", DoFileWatcher, DoFileWatcherException, @"D:\Development\test\a", @"*.txt", new List<WatcherChangeTypes>() { WatcherChangeTypes.Created, WatcherChangeTypes.Changed, WatcherChangeTypes.Deleted, WatcherChangeTypes.Renamed }, true));

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
        /// 掃描指定的檔案
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoScanFileInFolder(object obj)
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Do", Code.IFO_000));
                
                var tpStartDate = DateTime.Parse(DateTime.Now.AddHours(-scanFileStartH).ToString(@"yyyy-MM-dd"));
                //{2025/2/2 上午 12:00:00}
                var tpEndDate = DateTime.Parse(DateTime.Now.AddHours(-scanFileEndH).ToString(@"yyyy-MM-dd HH:mm:ss.fff"));
                //{2025/2/2 下午 11:18:49}

                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"tpStartDate:[{tpStartDate}],tpEndDate:[{tpEndDate}]", Code.IFO_000));

                // 確認機台資料夾是否存在
                if (Directory.Exists(this.productDataSourcePath))
                {
                    // 搜尋檔案
                    var findMDlist = new DirectoryInfo(this.productDataSourcePath).GetFiles(this.fileFilter!, SearchOption.AllDirectories).Where(x => DateTime.Compare(x.LastWriteTime, tpStartDate) >= 0 && DateTime.Compare(x.LastWriteTime, tpEndDate) <= 0).ToList();
                    //{D:\Development\test\PD\TC\Advan\eMMC\Machine1\DeviceName1\20250222\ET_TCT2025020222460100M1_ABC456789-001_MT00.log}

                    if (findMDlist != null && findMDlist.Any() && findMDlist.Count > 0)
                    {
                        // 放入Queue
                        Parallel.ForEach(findMDlist, file =>
                        {
                            // 已經有加入
                            if (sourceFileQ != null && sourceFileQ.Any(sf => sf.FilePath == file.FullName))
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"already Data in sourceFileQ, File:[{file.FullName}]", Code.IFO_000));
                            }
                            // 已經有加入
                            else if (reportInfoQ != null && reportInfoQ.Any(sf => sf.FilePath == file.FullName))
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"already Data in reportInfoQ, File:[{file.FullName}]", Code.IFO_000));
                            }
                            else if (backupFileQ != null && backupFileQ.Any(sf => sf.FilePath == file.FullName))
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"already Data in backupFileQ, File:[{file.FullName}]", Code.IFO_000));
                            }
                            else
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Add File in sourceFileQ, File:[{file.FullName}]", Code.IFO_000));
                                this.sourceFileQ!.Enqueue(new BaseSourceFile { FilePath = file.FullName });
                            }
                        });
                    }
                    // 沒有檔案
                    else
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Not any File in Folder, Folder:[{this.productDataSourcePath}], Filter:[{this.fileFilter!}]", Code.IFO_000));
                    }
                }
                // 資料夾不存在
                else
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Please check Folder Path, Folder Path Don't Exist, Folder:[{this.productDataSourcePath}]", Code.ODI_008, ILogType.Alarm, null, null));
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

        /// <summary>
        /// 掃描指定的檔案完成
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoScanFileInFolderFinish(object obj)
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Do", Code.IFO_000));
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

        /// <summary>
        /// 分析檔案
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoAnalyzeFile(object obj)
        {
            try
            {                
                BaseSourceFile? tpBSF = null;
                // 有取到新的檔案資料
                if (this.sourceFileQ!.TryDequeue(out tpBSF))
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Do", Code.IFO_000));

                    this.GetBaseReportInfo(tpBSF);
                }
                // 沒有新的檔案資料
                else
                {
                    // 等待
                    SpinWait.SpinUntil(() => false, 1000);
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

        /// <summary>
        /// 分析檔案完成
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoAnalyzeFileFinish(object obj)
        {
            try
            {
                //BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Do", Code.IFO_000));
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

        /// <summary>
        /// 解析指定的檔案內容
        /// </summary>
        /// <param name="tpBSF"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void GetBaseReportInfo(BaseSourceFile? tpBSF)
        {
            var reportInfo = new BaseReportInfo();
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Do", Code.IFO_000));

                if (tpBSF == null)
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Please check BaseSourceFile is Null", Code.ODI_005, ILogType.Alarm, null, null));
                    return;
                }

                if (!File.Exists(tpBSF.FilePath!))
                {
                    BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Please check Dll File Path, File Don't Exist:[{tpBSF.FilePath!}]", Code.FDE_001, ILogType.Alarm, null, null));
                    return;                    
                }

                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"FilePath:[{tpBSF.FilePath!}]", Code.IFO_000));

                string fileContent = File.ReadAllText(tpBSF.FilePath!);

                
                //string pattern = @"(?<=^|\n)([A-Za-z0-9]+)=(.*?)(?=\n|$)";

                foreach (Match match in Regex.Matches(fileContent, regexFilePattern))
                {
                    string key = match.Groups[1].Value.Trim();
                    string value = match.Groups[2].Value.Trim();

                    if (PropertyActions.ContainsKey(key))
                    {
                        PropertyActions[key](reportInfo, value);
                    }
                }

                reportInfo.Status = @"1";
                reportInfo.TestStatus = @"MT";
                reportInfo.Package = @"Package";
                reportInfo.Device = @"Device";
                reportInfo.UseTime = @"0";
                reportInfo.TestStatus = @"TestStatus";
                reportInfo.FilePath = tpBSF.FilePath!;
                reportInfo.FileName = Path.GetFileName(tpBSF.FilePath!);
                //reportInfo.FileDate = File.GetLastWriteTime(reportInfo.FilePath).ToString(@"yyyy-MM-dd HH:mm:ss.fff");
                reportInfo.FileDate = File.GetLastWriteTime(reportInfo.FilePath);
                
            }
            catch (ExpectedInfo ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
                ///將有問題的檔案, 依照使用者需求處置
                ///務必將檔案轉移至其他位置, 避免不斷被掃到而重工
            }
            catch (Exception ex)
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_002, ILogType.Catch, ex, null));
                ///將有問題的檔案, 依照使用者需求處置
                ///務必將檔案轉移至其他位置, 避免不斷被掃到而重工
            }
            finally
            {
                reportInfoQ!.Enqueue(reportInfo);
            }
        }

        /// <summary>
        /// 定義對應的 Key Value
        /// </summary>
        private static readonly Dictionary<string, Action<BaseReportInfo, string>> PropertyActions = new()
        {
            { "CusID", (obj, value) => obj.CustomerID = value },
            { "LotID", (obj, value) => obj.LotID = value },
            { "Step", (obj, value) => obj.Step = value },
            { "Product", (obj, value) => obj.Product = value },
            { "PartNumber", (obj, value) => obj.PartNumber = value },
            { "SerialNumber", (obj, value) => obj.SerialNumber = value },
            { "Manufacturer", (obj, value) => obj.Manufacturer = value },
            { "StartDate", (obj, value) => obj.StartTime = value },
            { "EndDate", (obj, value) => obj.EndTime = value },
            { "Recipe", (obj, value) => obj.Program = value },
            { "OP", (obj, value) => obj.Operator = value },
            { "MachineName", (obj, value) => obj.Machine = value },
            { "MachineIP", (obj, value) => obj.MachineIP = value },
            { "MachineMAC", (obj, value) => obj.MachineMAC = value },
            { "TestTime", (obj, value) => obj.TestTime = value.Replace(@"/s", string.Empty).ToString() },
            { "Temperature", (obj, value) => obj.Temperature = value }

            //{ "CusID", (obj, value) => obj.CusID = value },
            //{ "LotID", (obj, value) => obj.LotID = value },
            //{ "Step", (obj, value) => obj.Step = value },
            //{ "Product", (obj, value) => obj.Product = value },
            //{ "PartNumber", (obj, value) => obj.PartNumber = value },
            //{ "SerialNumber", (obj, value) => obj.SerialNumber = value },
            //{ "Manufacturer", (obj, value) => obj.Manufacturer = value },
            //{ "StartDate", (obj, value) => obj.StartDate = value },
            //{ "EndDate", (obj, value) => obj.EndDate = value },
            //{ "Recipe", (obj, value) => obj.Recipe = value },
            //{ "OP", (obj, value) => obj.OP = value },
            //{ "MachineName", (obj, value) => obj.MachineName = value },
            //{ "MachineIP", (obj, value) => obj.MachineIP = value },
            //{ "MachineMAC", (obj, value) => obj.MachineMAC = value }

/* 檔案內容
[INFO]
CusID=TCT
LotID=ABC456789-001
Step=MT00
Product=BICS6_Gen3
PartNumber=F12345-64G-NAND
SerialNumber=TCT2025020222460100M1
Manufacturer=GOD
WaferID=WAFER-67890
FlashDieID=TOSH-112233
ControlDieID=Ph-8229DD
Temperature=85
TestTime=1/s
StartDate=2025-02-02 22:42:00.000
EndDate=2025-02-02 22:42:01.000
AP=20250222V10603
Recipe=TCTFNTV79AAPL001
OP=170
MachineName=Machine1
MachineIP=172.20.20.22
MachineMAC=AB:10:CD:00:07:XY
*/
        };

        /// <summary>
        /// 測試
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoTest(object obj)
        {
            try
            {
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"111111111 aaa HelloWorld:[{DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.fff")}]", Code.IFO_000));

                SpinWait.SpinUntil(() => false, 40000);
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"111111111 bbb HelloWorld:[{DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.fff")}]", Code.IFO_000));

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

        /// <summary>
        /// 測試
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoTest2(object obj)
        {
            try
            {
                SpinWait.SpinUntil(() => false, 20000);
                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"2222222222 HelloWorld:[{DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss.fff")}]", Code.IFO_000));

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

        /// <summary>
        /// 測試完成
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoTestFinish(object obj)
        {
            try
            {
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

        private void DoFileWatcherException(string name, Exception exx)
        {
            try
            {
               
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

        /// <summary>
        /// 觸發監控到的事件
        /// </summary>
        /// <param name="obj"></param>
        private void DoFileWatcher(WatcherFileData obj)
        {
            try
            {
                if (obj.Types == WatcherChangeTypes.Created)
                {
                }
                else if (obj.Types == WatcherChangeTypes.Changed)
                {
                }
                else if (obj.Types == WatcherChangeTypes.Deleted)
                {
                }
                else if (obj.Types == WatcherChangeTypes.Renamed)
                {
                }
                else
                { 
                
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

        /// <summary>
        /// 將檔案內容存入資料庫
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoSaveReportInfoToDB(object obj)
        {
            try
            {
                if (this.reportInfoQ!.Count > 0)
                {
                    // 先取得檔案內容並加入陣列內(卡陣列上限)
                    List<BaseReportInfo> briL = new List<BaseReportInfo>();
                    do
                    {
                        BaseReportInfo? tpbri = null;
                        // 有取到分析後的檔案內容
                        if (this.reportInfoQ!.TryDequeue(out tpbri))
                        {
                            briL.Add(tpbri);
                        }
                    }
                    while (this.reportInfoQ!.Count > 0 && briL.Count <= saveDBCountLimit);

                    try
                    {
                        using (var conn = new SqlConnection(this.alPDDBConnection))
                        {
                            if (conn.State == ConnectionState.Closed) conn.Open();

                            // 執行 SQL
                            var results = conn.Execute(SQLInsertOrUpdateReportInfoToDB, briL);

                            // 成功
                            if (results == briL.Count())
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Update or Insert Report Info To DB Finish:[{string.Join(";", briL.Select(z => $@"{z.FilePath}"))}]", Code.IFO_000));
                                
                                // 加入備份清單
                                Parallel.ForEach(briL, bri =>
                                {
                                    this.backupFileQ!.Enqueue(new BaseBackupFile() { FileName = bri.FileName, FilePath = bri.FilePath, FileDate = bri.FileDate!, Machine = bri.Machine, LotID = bri.LotID, Step = bri.Step });
                                });
                            }
                            //執行失敗或資料資料庫一致
                            else
                            {
                                BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Update or Insert Report Info To DB Fail:[{string.Join(";", briL.Select(z => $@"{z.FilePath}"))}]", Code.ODI_008, ILogType.Alarm, null, null));
                                
                                ///將有問題的檔案, 依照使用者需求處置
                                ///務必將檔案轉移至其他位置, 避免不斷被掃到而重工
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
                // 尚未有任何檔案內容
                else
                {
                    // 等待
                    SpinWait.SpinUntil(() => false, saveDBIntervalLimit);
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

        /// <summary>
        /// 將檔案內容存入資料庫完成
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoSaveReportInfoToDBFinish(object obj)
        {
            try
            {
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

        /// <summary>
        /// 將檔案內轉移至備存資料夾
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoBackupFile(object obj)
        {
            try
            {
                if (this.backupFileQ!.Count > 0)
                {
                    // 確認備份資料夾是否存在
                    if (Directory.Exists(this.productDataBackUpPath))
                    {
                        BaseBackupFile? tpbbf = null;
                        // 有取到分析後的檔案內容
                        if (this.backupFileQ!.TryDequeue(out tpbbf))
                        {
                            bool isMoveFinish = false;
                            try
                            {
                                var newFileBackupFolderPath = Path.Combine(this.productDataBackUpPath, tpbbf.LotID!, tpbbf.Step!, tpbbf.Machine!, tpbbf.FileDate.ToString(@"yyyy-MM-dd"));
                                //D:\Development\test\backup\PD\TC\Advan\eMMC\Machine1\DeviceName1\ABC456789-001\MT00\Machine1\2025-02-02

                                //確認備份資料夾子目錄不存在, 就建立
                                if (!Directory.Exists(newFileBackupFolderPath))
                                {
                                    Directory.CreateDirectory(newFileBackupFolderPath);
                                }
                                var newFileBackupFilePath = Path.Combine(newFileBackupFolderPath, tpbbf.FileName!);
                                File.Move(tpbbf.FilePath!, newFileBackupFilePath);
                                isMoveFinish = true;
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
                            //搬移失敗, 重新加回去柱列
                            if (!isMoveFinish)
                            {
                                this.backupFileQ.Enqueue(tpbbf);
                                SpinWait.SpinUntil(() => false, 1000);
                            }
                        }
                    }
                    // 資料夾不存在
                    else
                    {
                        BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"Please check BackupFolder Path, Folder Path Don't Exist, Folder:[{this.productDataBackUpPath}]", Code.ODI_008, ILogType.Alarm, null, null));
                    }                    
                }
                // 尚未有任何要備份的檔案
                else
                {
                    // 等待
                    SpinWait.SpinUntil(() => false, 1000);
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

        /// <summary>
        /// 將檔案內轉移至備存資料夾完成
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ExpectedInfo"></exception>
        private void DoBackupFileFinish(object obj)
        {
            try
            {
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


        #region Other Class

        /// <summary>
        /// 來源檔案
        /// </summary>
        public class BaseSourceFile
        {
            public string? FilePath { get; set; }
        }

        /// <summary>
        /// 檔案內容
        /// </summary>
        public class BaseReportInfo
        {
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public string? CustomerID { get; set; }
            public string? LotID { get; set; }
            public string? Step { get; set; }
            public string? Product { get; set; }
            public string? PartNumber { get; set; }
            public string? SerialNumber { get; set; }
            public string? Manufacturer { get; set; }
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public string? Program { get; set; }
            public string? Operator { get; set; }
            public string? Machine { get; set; }
            public string? MachineIP { get; set; }
            public string? MachineMAC { get; set; }
            public string? Status { get; set; }
            public string? TestMode { get; set; }
            public string? Package { get; set; }
            public string? Device { get; set; }
            public string? UseTime { get; set; }
            public string? TestTime { get; set; }
            public string? TestStatus { get; set; }
            public string? Temperature { get; set; }
            public DateTime FileDate { get; set; }            
        }

        /// <summary>
        /// 備份的檔案
        /// </summary>
        public class BaseBackupFile
        {
            public string? FileName { get; set; }
            public string? FilePath { get; set; }
            public DateTime FileDate { get; set; }
            public string? Machine { get; set; }
            public string? LotID { get; set; }
            public string? Step { get; set; }
        }
        #endregion
    }
}
