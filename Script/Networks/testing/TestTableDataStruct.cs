using System;
using System.Collections.Generic;

namespace Networks.parser.testing
{
    /// <summary>
    /// 表结构数据
    /// </summary>
    public class TestTableDataStruct : AbsTableDataStruct
    {
        public override void RegisterBindingTableStrcut()
        {
            typeDict["ModuleProfile"] = typeof(ModuleProfile);
            typeDict["CityOrder"] = typeof(CityOrder);
        }
    }
}
