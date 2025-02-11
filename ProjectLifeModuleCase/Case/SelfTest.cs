using ILogger.AP;
using Judgment;
using ProjectLifeModuleManagement.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectLifeModuleCase.Case
{
    /// <summary>
    /// 自己, 測試用
    /// </summary>
    public class SelfTest : BMolecule
    {
        /**/
        #region Properties

        #endregion

        /**/
        #region Constructor

        public SelfTest(string name)
            : base(name)
        {
            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"HelloWorld", Code.IFO_000));
        }

        public SelfTest(string name, Dictionary<string, object> dic)
            : base(name)
        {
            BMolecule.Communication?.Invoke(new LogInfo(this.Name, this.GetType().Name, MethodBase.GetCurrentMethod()!.Name, $@"HelloWorld", Code.IFO_000));
            //this.AddProject(new MRightNow(@"test", DoTest, DoTestFinish));
            //this.AddProject(new MInfiniteLoop(@"test", DoTest, DoTestFinish));
            //this.AddProject(new MTimer(@"test", DoTest, DoTestFinish, 20000, true, true));
            //this.AddProject(new MTimer(@"test2", DoTest2, DoTestFinish, 10000, false, true));
            //this.AddProject(new MTimer(@"test2", DoTest2, DoTestFinish, 10000, false, false));
        }

        #endregion
    }
}
