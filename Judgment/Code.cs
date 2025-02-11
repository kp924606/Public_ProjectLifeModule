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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Judgment
{
    /// <summary>
    /// Code 表示錯誤類型、原因的說明.
    /// Code represents the explanation of the error type and reason.
    /// </summary>
    public enum Code
    {
        /// <summary>
        /// 紀錄資訊
        /// </summary>
        IFO_000,

        /// <summary>
        /// 成功
        /// </summary>
        GOD_000,

        /// <summary>
        /// 失敗
        /// </summary>
        FFF_000,

        /// <summary>
        /// 物件初始化完成
        /// </summary>
        INI_000,

        /// <summary>
        /// 物件初始化結果為 Null
        /// </summary>
        INI_001,

        /// <summary>
        /// 物件執行 Activator.CreateInstance 為 Null
        /// </summary>
        INI_002,

        /// <summary>
        /// 物件初始化時，包在內部發生 Try Catch 崩潰
        /// </summary>
        INI_003,

        /// <summary>
        /// 爆在最外層 Catch
        /// </summary>
        FCT_000,

        /// <summary>
        /// 包在 Parallel.ForEach 內的 Catch
        /// </summary>
        FCT_001,

        /// <summary>
        /// 此函示(Function) 內的 Try Catch
        /// </summary>
        FCT_002,

        /// <summary>
        /// 執行 if else，沒有符合的條件，判不合理
        /// </summary>
        FCT_003,

        /// <summary>
        /// 包在 switch 的 default，沒有符合的條件，判不合理
        /// </summary>
        FCT_004,

        /// <summary>
        /// 此函示 Task.Factory 內的 Catch
        /// </summary>
        FCT_005,

        /// <summary>
        /// 包在 while 內的 Catch
        /// </summary>
        FCT_006,

        /// <summary>
        /// 包在 Thread 內的 Catch
        /// </summary>
        FCT_007,

        /// <summary>
        /// 包在 Program(Console) 內最外層的 Catch
        /// </summary>
        FCT_008,

        /// <summary>
        /// 包在程式內，某段邏輯運作用的 Catch
        /// </summary>
        FCT_009,

        /// <summary>
        /// Task.wait 被 CancellationTokenSource 做取消
        /// </summary>
        FCT_010,

        /// <summary>
        /// 確認此物件為指定的內容(不是Null)
        /// </summary>
        ODI_000,

        /// <summary>
        /// 確認物件(string)資訊為 Null or Empty
        /// </summary>
        ODI_001,

        /// <summary>
        /// 確認Dictionary<strig,object>物件為 Null
        /// </summary>
        ODI_002,

        /// <summary>
        /// 確認Action<>物件為 Null
        /// </summary>
        ODI_003,

        /// <summary>
        /// 確認List<>物件為 Null
        /// </summary>
        ODI_004,

        /// <summary>
        /// 確認物件為 Null
        /// </summary>
        ODI_005,

        /// <summary>
        /// 設置物件內容時，來源端取得的資訊為空(Null)
        /// </summary>
        ODI_006,

        /// <summary>
        /// 確認Dictionary 內未包含指定的 Key
        /// </summary>
        ODI_007,

        /// <summary>
        /// 確認此字串路徑不存在
        /// </summary>
        ODI_008,

        /// <summary>
        /// 確認List<> 數量大於指定數量
        /// </summary>
        ODI_009,

        /// <summary>
        /// 物件內容，不符合指定的格式
        /// </summary>
        ODI_010,

        /// <summary>
        /// 確認變數為指定的內容
        /// </summary>
        VDE_000,

        /// <summary>
        /// 字串轉為數值(INT)錯誤
        /// </summary>
        VDE_001,

        /// <summary>
        /// 數值(INT) <= 0
        /// </summary>
        VDE_002,

        /// <summary>
        /// 數值(double) <= 0
        /// </summary>
        VDE_003,

        /// <summary>
        /// 數值(double) < 指定的數值
        /// </summary>
        VDE_004,

        /// <summary>
        /// 此 Result(bool) is False
        /// </summary>
        VDE_005,

        /// <summary>
        /// 數值(double) 不在指定的數值範圍內
        /// </summary>
        VDE_006,

        /// <summary>
        /// 數值(int) 不在指定的數值範圍內
        /// </summary>
        VDE_007,

        /// <summary>
        /// 字串轉為數值(double)錯誤
        /// </summary>
        VDE_008,

        /// <summary>
        /// 網路連線正常
        /// </summary>
        NTE_000,

        /// <summary>
        /// 網路連線異常
        /// </summary>
        NTE_001,

        /// <summary>
        /// 網路連線回覆錯誤
        /// </summary>
        NTE_002,

        /// <summary>
        /// HttpResponse 成功
        /// </summary>
        HTR_000,

        /// <summary>
        /// HttpResponse Fail
        /// </summary>
        HTR_001,

        /// <summary>
        /// 網路回覆異常,請確認網路連線
        /// </summary>
        HTR_002,

        /// <summary>
        /// 列舉符合定義的未知項目
        /// </summary>
        ENE_001,

        /// <summary>
        /// 檔案路徑不存在
        /// </summary>
        FDE_001,
    }
}
