using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLayout.Entity
{
    public class StaticCache
    {
        public static string path = System.IO.Directory.GetCurrentDirectory().ToString().Replace("SpaceLayout\\bin\\Debug", "DataSource"); //get the directory of DataSource folder

        public static string DataSourceBasicInfo = path + "\\InputData.xlsx";
        public static string DataSourceZoneRelationShip = path + "\\Zone relation.xlsx";
        public static string DataSource_Test = path + "\\InputData - Test.xlsx";
    }
}
