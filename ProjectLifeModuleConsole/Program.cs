/// 【LICENSE】
///  Copyright 2025 TCT, located in the Galaxy Oasis Arm, Solar System, Earth, Asia, Kaohsiung City, Taiwan. All rights reserved.
///  Everyone is permitted to copy and distribute verbatim copies of this license document, but changing it is not allowed.
///
/// 【開發者聲明】
/// 本專案由 [蔡承廷/TCT] 開發，遵循開源原則，免費提供給所有有興趣的人員使用、修改和分發.
///
/// 【引用方式】
/// 使用本專案之 DLL 時，請確保包含此聲明，並遵守相關的開源許可證條款.
///
/// 【ACKNOWLEDGEMENTS】
/// 本發行版包含 MicroSoft 開發環境提供的套件.
/// 本發行版包應用於 MicroSoft Windows 系統的軟體.
///
/// 允許使用、複製、修改、分發和銷售本軟體及其文件，無需支付任何費用，前提是上述版權聲明出現在所有副本中，並且該版權聲明和本許可聲明均出現在支持文件中.
/// 上述版權聲明和本許可聲明應包含在所有副本或軟體的重要部分中.
///
/// 本軟體按提供，無任何類型的保證，無論是明示或暗示，包括但不限於對適銷性、特定用途的適用性和不侵權的保證.在任何情況下，TCT 對於因使用本軟體或與本軟體的使用或其他交易相關的任何索賠、損害或其他責任不承擔任何責任，無論是基於合同、侵權或其他原因.
///
/// 除非在本聲明中包含，否則不得在廣告中或其他方式使用 TCT 的名稱來促進本軟體的銷售、使用或其他交易，除非事先獲得 TCT 的書面授權.
///
/// 版權所有 2025 TCT，位於宇宙銀河綠洲臂太陽系地球亞洲臺灣高雄市.保留所有權利.
/// 允許使用、複製、修改和分發本軟體及其文件，無需支付任何費用，前提是上述版權聲明出現在所有副本中，並且該版權聲明和本許可聲明均出現在支持文件中，且不得在有關軟體分發的廣告或宣傳中使用 TCT 的名稱，除非事先獲得具體的書面許可.
///
/// TCT 對於本軟體不承擔任何保證，包括所有隱含的適銷性和適用性保證，在任何情況下，TCT 對於任何特殊、間接或後果性損害或因使用、本資料或利潤損失而產生的任何損害不承擔任何責任，無論是基於合同、過失或其他侵權行為，均不承擔責任.
///
/// 【軟體免責聲明】
/// 本軟體按，不提供任何明示或暗示的擔保，包括但不限於對適銷性、特定用途適用性及非侵權的擔保.
/// 在適用法律允許的最大範圍內，對因使用或無法使用本軟體所產生的損害及風險，包括但不限於直接或間接的個人損害、商業利潤的喪失、貿易中斷、商業信息的丟失或任何其他經濟損失，開發者不承擔任何責任.
///
/// 【Developer Declaration】
/// This project is developed by [Tsai Cheng-Ting/TCT] and is freely available for use, modification, and distribution by all interested parties, adhering to open-source principles.
///
/// 【Usage Instructions】
/// When using the DLL of this project, please ensure that this declaration is included and that you comply with the relevant open-source license terms.
///
/// 【ACKNOWLEDGEMENTS】
/// This distribution includes packages provided by the Microsoft development environment.
/// This distribution contains software applicable to the Microsoft Windows system. Copyright 2025 TCT.
///
/// Permission is granted to use, copy, modify, distribute, and sell this software and its documentation without any fee, provided that the above copyright notice appears in all copies and that both the copyright notice and this permission notice are included in supporting documentation.
/// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
///
/// This software is provided without any type of warranty, express or implied, including but not limited to warranties of merchantability, fitness for a particular purpose, and non-infringement. In no event shall TCT be liable for any claims, damages, or other liabilities arising from the use of this software or in connection with the use or other dealings in this software, whether based on contract, tort, or other reasons.
///
/// Except as contained in this notice, the name of TCT shall not be used in advertising or otherwise to promote the sale, use, or other dealings in this software without prior written authorization from TCT.
///
/// Copyright 2025 TCT, located in the Galaxy Oasis Arm, Solar System, Earth, Asia, Kaohsiung City, Taiwan. All rights reserved.
/// Permission is granted to use, copy, modify, and distribute this software and its documentation without any fee, provided that the above copyright notice appears in all copies and that both the copyright notice and this permission notice are included in supporting documentation, and that the name of TCT shall not be used in advertising or publicity pertaining to the distribution of the software without specific, written prior permission.
///
/// TCT disclaims all warranties with regard to this software, including all implied warranties of merchantability and fitness. In no event shall TCT be liable for any special, indirect, or consequential damages or any damages whatsoever resulting from loss of use, data, or profits, whether in an action of contract, negligence, or other tortious action, arising out of or in connection with the use or performance of this software.
///
/// 【Software Disclaimer】
/// This software is provided without any express or implied warranties, including but not limited to the implied warranties of merchantability, fitness for a particular purpose, and non-infringement.
/// To the maximum extent permitted by applicable law, the developer shall not be liable for any damages or risks arising from the use or inability to use this software, including but not limited to direct or indirect personal injury, loss of commercial profits, business interruption, loss of business information, or any other economic loss.


using HolyGift;
using ILogger.AP;
using ILogger.Enum;
using ILogger.Interface;
using Judgment;
using OLogger.AP;
using ProjectLifeModuleManagement.Base;
using ProjectLifeModuleManagement.Interface;
using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using TCTUtility.Function;

/**/
#region Close Exit Button in Wondow
const int MF_BYCOMMAND = 0x00000000;
const int SC_CLOSE = 0xF060;
[DllImport("user32.dll")]
static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
[DllImport("user32.dll")]
static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
[DllImport("kernel32.dll", ExactSpelling = true)]
static extern IntPtr GetConsoleWindow();
#endregion

#region Property
//宣告輸出用的 Log 資訊物件
OLogInfo? ologinfo = null;
SemaphoreSlim semaphoreLogInfo = new SemaphoreSlim(1); // 限制最多執行緒同時執行

#endregion

try
{
    // Disable 關閉視窗
    DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);

    // 顯示視窗 Title
    ShowConsoleTitle(string.Empty);

    // 取 Config ModuleName 設置    
    string strMN = ConfigurationManager.AppSettings[Key.ModuleName]!;
    if (string.IsNullOrEmpty(strMN))
    {
        throw new ExpectedInfo($@"Please check ModuleName in Connfig, Get ModuleName is Null Or Empty", Code.ODI_001);
    }

    // 取得 OLogInfo 資料 from Config
    var section = (ConfigurationManager.GetSection(typeof(OLogInfo).Name) as Hashtable)!.Cast<DictionaryEntry>().ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
    //{[MaxSaveLimit, 7777]}
    //{[MaxSaveSize, 3]}
    //{[MaxSaveDay, 30]}

    int tpMSD = 0;
    if (!Int32.TryParse(section[Key.MaxSaveDay], out tpMSD))
    {
        throw new ExpectedInfo($@"Please check MaxSaveDay in Connfig, Format is Not Int", Code.VDE_001);
    }

    int tpMSS = 0;
    if (!Int32.TryParse(section[Key.MaxSaveSize], out tpMSS))
    {
        throw new ExpectedInfo($@"Please check MaxSaveSize in Connfig, Format is Not Int", Code.VDE_001);
    }

    int tpMSL = 0;
    if (!Int32.TryParse(section[Key.MaxSaveLimit], out tpMSL))
    {
        throw new ExpectedInfo($@"Please check MaxSaveLimit in Connfig, Format is Not Int", Code.VDE_001);
    }

    // 產出 Log 物件設置 
    ologinfo = new OLogInfo(strMN, tpMSD, tpMSS, tpMSL);

    Communication(new LogInfo(Key.System, MethodBase.GetCurrentMethod()!.DeclaringType!.ToString(), MethodBase.GetCurrentMethod()!.Name, $@"
        /***********************************************
                          _ooOoo_
                         o8888888o
                         88"" . ""88
                         (| -_- |)
                         O\  =  /O
                      ____/`---'\____
                    .'  \\|     |//  `.
                   /  \\|||  :  |||//  \
                  /  _||||| -:- |||||-  \
                  |   | \\\  -  /// |   |
                  | \_|  ''\---/''  |   |
                  \  .-\__  `-`  ___/-. /
                ___`. .'  /--.--\  `. . __
             ."""" '<  `.___\_<|>_/___.'  >'"""".
            | | :  `- \`.;`\ _ /`;.`/ - ` : | |
            \  \ `-.   \_ __\ /__ _/   .-` /  /
       ======`-.____`-.___\_____/___.-`____.-'======
                          `=---='
       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                佛祖保佑       永無BUG
       *********************************************/
", Code.IFO_000));

    Communication(new LogInfo(Key.System, MethodBase.GetCurrentMethod()!.DeclaringType!.ToString(), MethodBase.GetCurrentMethod()!.Name, @"
    /**************************************************************

      [       遵從自然規律，讓萬物自行發揮       ]  <-----------
                            |                                   |
                            |                                   |
      [        遵從自然規律，讓萬物自行相剋      ]              |
                            |                                   |
                            |                                   |
      [          精通萬物一切原理熟知於心        ]              |
                            |                                   |
                            |                                   |
      [          運用萬物一切原理強身健體        ]              |
                            |                                   |
                            |                                   |
      [  緣起緣滅，讓萬物一切自動運轉，自生自滅  ]              |
                            |                                   |
                            |                                   |
      [        時有時無，萬物一切可有可無        ]              |
                            |                                   |
                            |                                   |
      [             天人合一，無欲無求           ]              |
                            |                                   |
                            |                                   |
      [              萬物一切歸為無              ]              |
                            |                                   |
                            |                                   |
      [           心有所想，創建美妙世界         ] --------------

    ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
              如何成為長官的秘訣            劉彩萍指揮官
    ***************************************************************/
            ", Code.IFO_000));


    Communication(new LogInfo(Key.System, MethodBase.GetCurrentMethod()!.DeclaringType!.ToString(), MethodBase.GetCurrentMethod()!.Name, $@"Get Case:[{strMN}] by Config", Code.IFO_000));

    //設定模組內的溝通方式
    BMolecule.Communication = Communication;
        
    // 取 Module Dll 檔案全名
    string strMDFFN = ConfigurationManager.AppSettings[Key.ModuleDllFileFullName]!;
    if (string.IsNullOrEmpty(strMDFFN))
    {
        throw new ExpectedInfo($@"Please check ModuleDllFileFullName in Connfig, Get ModuleDllFileFullName is Null Or Empty", Code.ODI_001);
    }

    // 取 Module Dll內對應的名稱使用的 NameSpace
    string strMDNS = ConfigurationManager.AppSettings[Key.ModuleDllNameSpace]!;
    if (string.IsNullOrEmpty(strMDNS))
    {
        throw new ExpectedInfo($@"Please check ModuleDllNameSpace in Connfig, Get ModuleDllNameSpace is Null Or Empty", Code.ODI_001);
    }

    //取得目前目錄下的 dll 路徑
    string strDFP = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strMDFFN!);
    //D:\God\Development\ProjectLifeModule\ProjectLifeModuleConsole\bin\Debug\net8.0\ProjectLifeModuleManagement.dll

    if (!File.Exists(strDFP))
    {
        throw new ExpectedInfo($@"Please check Dll File Path, File Don't Exist:[{strDFP}]", Code.FDE_001);
    }

    Assembly myassembly = Assembly.LoadFrom(strDFP);
    if (myassembly == null)
    {
        throw new ExpectedInfo($@"Please check Dll Content, Set Dll Assembly Object is Null:[{strDFP}]", Code.ODI_005);
    }

    //取得 namespace
    Type type = myassembly.GetType($@"{strMDNS!}{strMN!}")!;
    //{Name = "SelfTest" FullName = "ProjectLifeModuleManagement.Case.SelfTest"}
    if (type == null)
    {
        throw new ExpectedInfo($@"Please check namespace, Type is Null, Assembly GetType:[{strMDNS!}{strMN!}], ModuleDllFileFullName:[{strMDFFN}], ModuleDllNameSpace:[{strMDNS}]", Code.ODI_005);
    }

    // 取 ModuleConfigFolderPath
    string strMCFP = ConfigurationManager.AppSettings[Key.ModuleConfigFolderPath]!;
    //D:\God\Development\ProjectLifeModule\Data\ProjectConfig
    if (string.IsNullOrEmpty(strMCFP))
    {
        throw new ExpectedInfo($@"Please check ModuleConfigFolderPath in Connfig, Get ModuleConfigFolderPath is Null Or Empty", Code.ODI_001);
    }

    //取得目前目錄下的 dll 路徑
    string strPC = Path.Combine(strMCFP, $@"{strMN}.config");
    //D:\God\Development\ProjectLifeModule\Data\ProjectConfig\SelfTest.config
    if (!File.Exists(strPC))
    {
        throw new ExpectedInfo($@"Please check ProjectConfig File, File Don't Exist:[{strPC}]", Code.FDE_001);
    }

    // 取得 Project 資料 from Config
    var projectSection =  FUtility.GetCustomConfig(strPC, strMN)!;
    //{[ProductDataSourcePath, \\TW0701IWFADPDFS01\PD\TC\Advan\xxx\yyy\zzz]}

    IBehavior iDoBehavior = (IBehavior)Activator.CreateInstance(type, new object[] { strMN!, projectSection })!;
    if (iDoBehavior == null)
    {
        throw new ExpectedInfo($@"Please check Case or init Function, Activator CreateInstance Fail:[{strMN!}]", Code.ODI_005);
    }
    else
    {        
        // 顯示視窗 Title
        ShowConsoleTitle($@"{strMN}");

        //開始執行
        iDoBehavior.Start();

#if DEBUG

        SpinWait.SpinUntil(() => false, 10000);
        iDoBehavior.Interruption();

        SpinWait.SpinUntil(() => false, 10000);
        iDoBehavior.Annihilation();

        SpinWait.SpinUntil(() => false, 10000);
        iDoBehavior.Start();

        SpinWait.SpinUntil(() => false, 10000);

#else

#endif

        bool isOpen = true;
        do
        {
            Thread.Sleep(Timeout.Infinite);

        }
        while (isOpen);
    }
}
catch (ExpectedInfo ex)
{
    Communication(new LogInfo(Key.System, MethodBase.GetCurrentMethod()!.DeclaringType!.ToString(), MethodBase.GetCurrentMethod()!.Name, Key.ExpectedInfo, ex.ReasonCode, ILogType.Error, ex, null));
}
catch (Exception ex)
{
    Communication(new LogInfo(Key.System, MethodBase.GetCurrentMethod()!.DeclaringType!.ToString(), MethodBase.GetCurrentMethod()!.Name, Key.Catch, Code.FCT_002, ILogType.Catch, ex, null));
}
finally
{
    SpinWait.SpinUntil(() => false, 1000);
}


/**/
#region Method

/// 溝通(可用來記錄 Log)
async void Communication(ILogInfo li)
{
    //取得一個執行權限
    await semaphoreLogInfo.WaitAsync();

    try
    {    
        await Task.Run(() =>
        {    
            try
            {               
                switch (li.Type)
                {
                    case ILogType.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine($@"{DateTime.Now.ToString(@"yyyy/MM/dd HH:mm:ss")}<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}] {li.Info}");
                        ologinfo!.Logger.Info($@"<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}] {li.Info}");
                        break;
                    case ILogType.Alarm:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine($@"{DateTime.Now.ToString(@"yyyy/MM/dd HH:mm:ss")}<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}] {li.Info}");
                        ologinfo!.Logger.Info($@"<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}] {li.Info}");
                        break;
                    case ILogType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine($@"{DateTime.Now.ToString(@"yyyy/MM/dd HH:mm:ss")}<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        ologinfo!.Logger.Info($@"<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        break;
                    case ILogType.Catch:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine($@"{DateTime.Now.ToString(@"yyyy/MM/dd HH:mm:ss")}<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        ologinfo!.Logger.Info($@"<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        break;
                    case ILogType.Fail:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine($@"{DateTime.Now.ToString(@"yyyy/MM/dd HH:mm:ss")}<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        ologinfo!.Logger.Info($@"<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        break;
                    case ILogType.Pass:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine($@"{DateTime.Now.ToString(@"yyyy/MM/dd HH:mm:ss")}<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}] {li.Info}");
                        ologinfo!.Logger.Info($@"<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}] {li.Info}");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($@"{DateTime.Now.ToString(@"yyyy/MM/dd HH:mm:ss")}<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        ologinfo!.Logger.Info($@"<{li.Name},{li.Class}>[{li.Method}][{li.ResultCode}][{li.Info}] {li.Error}");
                        break;
                }
            }
            catch
            {
            }
            finally
            {
            }
        });
    }
    catch
    {
    }
    finally
    {
        //釋放執行權限資源
        semaphoreLogInfo.Release();
    }
}

//顯示視窗 Title
void ShowConsoleTitle(string strMessage)
{
    string data = $@"{Assembly.GetExecutingAssembly().GetName().Name!.ToString()} [{Assembly.GetExecutingAssembly().GetName().Version!.ToString()}]";
    Console.Title = $@"{data} {strMessage}";
}

#endregion


