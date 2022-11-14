using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklaDemoConnection
{
    public static class DefaultData
    {
        public static GeneralData GeneralData { get; set; }
        public static ColumnData ColumnData { get; set; }
        public static BeamData BeamData { get; set; }

        public static string GetAssemblyName()
        {
            try
            {
                return System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(new string[] { ", " }, StringSplitOptions.None)[0];
            }
            catch (Exception) { }
            return "Unknown";
        }

        static DefaultData()
        {
            GeneralData = new GeneralData();

            GeneralData.Type = 0;
            GeneralData.PcAttributeFile = 0;
            GeneralData.PcsAttributeFile = 0;
            GeneralData.LoadClass = 2;
            GeneralData.NumberingType = 0;
            GeneralData.CountryStandard = 3;
            GeneralData.CreateReinforcement = 0;
            GeneralData.ShowWarnings = 0;
            GeneralData.ShoeAnchorDist = 0;

            ColumnData = new ColumnData();

            ColumnData.CoverThickness = 30.0;
            ColumnData.cItem = 0;

            StirrupData stirrupC = new StirrupData();
            stirrupC.prefix = "";
            stirrupC.no = 1;
            stirrupC.name = "HAKA";
            stirrupC.grade = "B500B";
            stirrupC.tsClass = 502;
            ColumnData.stirrup = stirrupC;

            StirrupData uBarC = new StirrupData();
            uBarC.prefix = "";
            uBarC.no = 1;
            uBarC.name = "LISÄTERÄS";
            uBarC.grade = "B500B";
            uBarC.tsClass = 506;
            ColumnData.uBar = uBarC;

            BeamData = new BeamData();

            BeamData.CoverThicknessTop = 25.0;
            BeamData.CoverThicknessBottom = 25.0;
            BeamData.CoverThicknessLeft = 25.0;
            BeamData.CoverThicknessRight = 25.0;

            BeamData.BottomHeight = 100;
            BeamData.bItem = 0;

            StirrupData stirrupB = new StirrupData();
            stirrupB.prefix = "";
            stirrupB.no = 1;
            stirrupB.name = "HAKA";
            stirrupB.grade = "B500B";
            stirrupB.tsClass = 502;
            BeamData.stirrup = stirrupB;

            StirrupData uBarB = new StirrupData();
            uBarB.prefix = "";
            uBarB.no = 1;
            uBarB.name = "LISÄTERÄS";
            uBarB.grade = "B500B";
            uBarB.tsClass = 506;
            BeamData.uBar = uBarB;
        }
    }
}
