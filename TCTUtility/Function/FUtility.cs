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
using Judgment;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TCTUtility.Function
{
    /// <summary>
    /// 通用函示, 表示生命、物件、個體, 所有會用到的一致性實作方式
    /// </summary>
    public static class FUtility
    {
        // 取得自定義的文件檔案, 自訂 Config
        /*
        <SelfTest
        MaxSaveDay = "30"
        MaxSaveSize="3"
        MaxSaveLimit="7777"
        />
        */
        public static Dictionary<string, object> GetCustomConfig(string filePath, string section)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new ExpectedInfo($@"Please check File Path, File Don't Exist:[{filePath}]", Code.FDE_001);
                }

                // 使用 XDocument 解析 XML
                XDocument doc = XDocument.Load(filePath);

                // 假設設定檔的根節點是 指定的 Section
                XElement root = doc.Root!;
                
                // 確認是否有內容
                if (root == null)
                {
                    throw new ExpectedInfo($@"Please check Config Data, Data is Null:[{filePath}],[{section}]", Code.ODI_005);
                }

                //確認是否符合指定的 Section
                if (root.Name != section)
                {
                    throw new ExpectedInfo($@"Please check Config Data, Data Don't Match Section:[{filePath}],[{section}]", Code.FCT_003);
                }

                // 將屬性轉換成 Dictionary<string, object>
                return root.Attributes().ToDictionary(attr => attr.Name.LocalName, attr => (object)attr.Value);
            }
            catch (ExpectedInfo ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.ExpectedInfo}[{ex}]", ex.ReasonCode);
            }
            catch (Exception ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.Catch}[{ex}]", Code.FCT_002);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 確認 Dictionary 有包含指定的 Key 及確認資料不為空並回傳資料(字串)
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ExpectedInfo"></exception>
        public static string GetStringAndCheckNOrEFromDic(Dictionary<string, object> dic, string key)
        {
            try
            {
                if (dic == null)
                {
                    throw new ExpectedInfo($@"Please check Dictionary is Null", Code.ODI_005);
                }

                if (!dic.ContainsKey(key))
                {
                    throw new ExpectedInfo($@"Please check Dictionary, Don't have Key:[{key}]", Code.ODI_007);
                }

                if (string.IsNullOrEmpty(dic[key].ToString()))
                {
                    throw new ExpectedInfo($@"Please check Dictionary, Value IsNullOrEmpty, Key:[{key}]", Code.ODI_001);
                }
                else
                {
                    return dic[key].ToString()!;
                }
            }
            catch (ExpectedInfo ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.ExpectedInfo}[{ex}]", ex.ReasonCode);
            }
            catch (Exception ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.Catch}[{ex}]", Code.FCT_002);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 確認 Dictionary 有包含指定的 Key 及確認資料不為空並回傳資料(字串)
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ExpectedInfo"></exception>
        public static double GetDoubleAndCheckNOrEFromDic(Dictionary<string, object> dic, string key)
        {
            try
            {
                if (dic == null)
                {
                    throw new ExpectedInfo($@"Please check Dictionary is Null", Code.ODI_005);
                }

                if (!dic.ContainsKey(key))
                {
                    throw new ExpectedInfo($@"Please check Dictionary, Don't have Key:[{key}]", Code.ODI_007);
                }

                if (string.IsNullOrEmpty(dic[key].ToString()))
                {
                    throw new ExpectedInfo($@"Please check Dictionary, Value IsNullOrEmpty, Key:[{key}]", Code.ODI_001);
                }

                double tpd = 0;
                if (double.TryParse(dic[key].ToString(), out tpd))
                {
                    return tpd;
                }
                else
                {
                    throw new ExpectedInfo($@"Please check Dictionary, Value TryParse to Double Fail, Key:[{key}], Value:[{dic[key].ToString()}]", Code.VDE_008);
                }                
            }
            catch (ExpectedInfo ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.ExpectedInfo}[{ex}]", ex.ReasonCode);
            }
            catch (Exception ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.Catch}[{ex}]", Code.FCT_002);
            }
            finally
            {
            }
        }        

        /// <summary>
        /// 確認 List<string> 元素是否為指定的時間格式
        /// </summary>
        /// <param name="dl">@"08:00"</param>
        /// <param name="message">錯誤資訊</param>
        /// <returns></returns>
        /// <exception cref="ExpectedInfo"></exception>
        public static bool ValidateTimeFormatForHHmm(List<string> dl, out string message)
        {
            try
            {
                if (dl == null)
                {
                    throw new ExpectedInfo($@"Please check List<string> is Null", Code.ODI_004);
                }

                string format = "HH:mm";
                bool isValid = true;
                var tpmessage = string.Empty;

                foreach (var time in dl)
                {
                    if (!DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        tpmessage = $@"{tpmessage}, [{time}]";
                        isValid = false;
                    }
                }

                // 所有時間格式正確
                if (isValid)
                {
                    message = string.Empty;
                }
                // 有時間格式錯誤
                else
                {
                    message = $@"Please check Format doesn't Match:[{format}] {tpmessage}";
                }
                return isValid;
            }
            catch (ExpectedInfo ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.ExpectedInfo}[{ex}]", ex.ReasonCode);
            }
            catch (Exception ex)
            {
                throw new ExpectedInfo($@"[{Key.Static},{MethodBase.GetCurrentMethod()!.DeclaringType!.ToString()},{MethodBase.GetCurrentMethod()!.Name}]:{Key.Catch}[{ex}]", Code.FCT_002);
            }
            finally
            {
            }
        }
    }
}
