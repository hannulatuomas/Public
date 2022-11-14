using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Tekla.Structures.Model;
using System.Collections;
using Component = Tekla.Structures.Model.Component;

namespace Stabiliteettilaskenta
{
    public partial class Form1 : Form
    {
        AutoCad.ACadManager ACadManager;
        AutoCad.ACadDataManager ACadDataManager;
        AutoCad.ACadLayerManager ACadLayerManager;
        RFem.ModelBuilder rfModelBuilder;
        RFem.RFemLoadings rfLoadings;
        TEKLA.TeklaModelManager teklaModelManager;
        public Form1()
        {
            InitializeComponent();
            this.TopMost = checkBox_OnTop.Checked;
            this.radioButton_beamDirX.Checked = true;
            this.radioButton_beamDirY.Checked = false;
            ACadManager = new AutoCad.ACadManager();
            ACadDataManager = new AutoCad.ACadDataManager();
            ACadLayerManager = new AutoCad.ACadLayerManager();
            rfModelBuilder = new RFem.ModelBuilder();
            rfLoadings = new RFem.RFemLoadings();
            teklaModelManager = new TEKLA.TeklaModelManager();
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            List<string> models = RFemConnection.GetModels();

            if (models == null)
            {
                return;
            }

            foreach (string model in models)
            {
                comboBox_ModelName.Items.Add(model);
            }

            comboBox_ModelName.SelectedItem = comboBox_ModelName.Items[0];
        }

        private void button_getSupportForces_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            LoadingType loadingType = RFem.Utility.GetLoadingType(comboBox_LoadingType.Text);
            int loadingNumber = int.Parse(textBox_LoadingNumber.Text);
            RFem.LoadingCase loadingCase = new RFem.LoadingCase(loadingNumber, loadingType);

            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }
            AutoCad.ACadManager cadManager = new AutoCad.ACadManager();

            IModel iModel = RFemConnection.RfModel;
            IResults iResults = RFem.Utility.GetResults(iModel, loadingCase);

            if (checkBox_nodalSupports.Checked)
            {
                NodalSupportForces[] nodalSF = RFem.GetResults.SupportReactions.GetNodalReactions(iModel, iResults);

                foreach (NodalSupportForces sf in nodalSF)
                {
                    Vector3 coordinates = RFem.GetResults.GetNodeCoordinates(iModel, sf.NodeNo);
                    Vector3 point = new Vector3(coordinates.X, coordinates.Y, 0);

                    string Fx = "Fx: " + Math.Round(sf.Forces.X / 1000, 3).ToString();
                    string Fy = "Fy: " + Math.Round(sf.Forces.Y / 1000, 3).ToString();
                    string Fz = "Fz: " + Math.Round(sf.Forces.Z / 1000, 3).ToString();
                    string text = "Point: " + "\n" + Fx + "\n" + Fy + "\n" + Fz;

                    if (sf.Type == ResultsValueType.MinimumValueType)
                    {
                        cadManager.SendText(point, text, 10000, 250);
                    }
                }

            }

            if (checkBox_lineSupports.Checked)
            {
                LineSupportForces[] lineSF = RFem.GetResults.SupportReactions.GetLineReactions(iModel, iResults);
                List<LineSupportForces> supportForces = lineSF.OrderBy(o => o.LineNo).ToList();
                List<Vector3> maxForces = new List<Vector3>();
                List<Vector3> lineCoordinates = new List<Vector3>();
                int lineNo = int.MaxValue;
                int i = 0;

                foreach (LineSupportForces sf in supportForces)
                {
                    float fx = (float)sf.Forces.X;
                    float fy = (float)sf.Forces.Y;
                    float fz = (float)sf.Forces.Z;

                    if (sf.LineNo != lineNo)
                    {
                        Vector3 lineCoordinate = RFem.GetResults.GetLineCoordinate(iModel, sf.LineNo);
                        Vector3 point = new Vector3(lineCoordinate.X, lineCoordinate.Y, 0);
                        lineNo = sf.LineNo;
                        maxForces.Add(new Vector3(fx, fy, fz));
                        lineCoordinates.Add(point);
                        i = maxForces.Count - 1;

                        //IModelData iModelData = iModel.GetModelData();
                        //ILine iLine = iModelData.GetLine(lineNo, ItemAt.AtNo);
                        //Line line = iLine.GetData();

                        //lineForces.Add(new LineForces(line));
                    }
                    else
                    {

                        if (maxForces[i].X < fx) fx = maxForces[i].X;
                        if (maxForces[i].Y < fy) fy = maxForces[i].Y;
                        if (maxForces[i].Z < fz) fz = maxForces[i].Z;

                        maxForces[i] = new Vector3(fx, fy, fz);
                    }
                }

                for (int j = 0; j < maxForces.Count; j++)
                {
                    Vector3 force = maxForces[j];
                    Vector3 coordinate = lineCoordinates[j];

                    string Fx = "Fx: " + Math.Round(force.X / 1000, 3).ToString();
                    string Fy = "Fy: " + Math.Round(force.Y / 1000, 3).ToString();
                    string Fz = "Fz: " + Math.Round(force.Z / 1000, 3).ToString();
                    string text = "Line: " + "\n" + Fx + "\n" + Fy + "\n" + Fz;

                    cadManager.SendText(coordinate, text, 10000, 250);
                }
            }
        }

private void button_getSupportForceResultant_Click(object sender, EventArgs e)
        {
            //string[] loadingNumberString = textBox_loadingNo.Text.Split(',');
            //string[] beamNumberString = textBox_memberNo.Text.Split(',');

            //int[] loadCombinations = new int[loadingNumberString.Length];
            //int[] beamNumbers = new int[beamNumberString.Length];

            //for (int i = 0; i < loadingNumberString.Length; i++)
            //{
            //    try
            //    {
            //        int n = int.Parse(loadingNumberString[i]);
            //        loadCombinations[i] = n;
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}
            //for (int i = 0; i < beamNumberString.Length; i++)
            //{
            //    try
            //    {
            //        int n = int.Parse(beamNumberString[i]);
            //        beamNumbers[i] = n;
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}

            int[] loadCombinations = { 1,2,3 };//15-33, 48-66
            int[] beamNumbers = { 43,49 }; //5820, 5832, 5833

            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet worksheet = excelApp.ActiveSheet;

            // Establish column headings
            int row = 8;

            if (checkBox_beamStart.Checked)
            {
                worksheet.Cells[row, "A"] = "CO";
                worksheet.Cells[row, "B"] = "N [kN]";
                worksheet.Cells[row, "C"] = "Vy [kN]";
                worksheet.Cells[row, "D"] = "Vz [kN]";
                worksheet.Cells[row, "E"] = "Mt [kNm]";
                worksheet.Cells[row, "F"] = "My [kNm]";
                worksheet.Cells[row, "G"] = "Mz [kNm]";
            }
            if (checkBox_beamEnd.Checked)
            {
                worksheet.Cells[row, "J"] = "CO";
                worksheet.Cells[row, "K"] = "N [kN]";
                worksheet.Cells[row, "L"] = "Vy [kN]";
                worksheet.Cells[row, "M"] = "Vz [kN]";
                worksheet.Cells[row, "N"] = "Mt [kNm]";
                worksheet.Cells[row, "O"] = "My [kNm]";
                worksheet.Cells[row, "P"] = "Mz [kNm]";
            }
            
            row = row + 2;

            float N = 0f;
            float Vy = 0f;
            float Vz = 0f;
            float Mt = 0f;
            float My = 0f;
            float Mz = 0f;

            for (int i = 0; i < loadCombinations.Length; i++)
            {
                int loadingNo = loadCombinations[i];
                RFem.LoadingCase loadingCase = new RFem.LoadingCase(loadingNo, LoadingType.ResultCombinationType);
                IResults iResults = RFem.Utility.GetResults(iModel, loadingCase);

                for (int j = 0; j < beamNumbers.Length; j++)
                {
                    MemberForces[] forces = iResults.GetMemberInternalForces(beamNumbers[j], ItemAt.AtNo, true);

                    double location = 0;
                    N = 0f;
                    Vz = 0f;
                    My = 0f;

                    for (int k = 0; k < forces.Length; k++)
                    {
                        if (forces[k].Location - location < 0.001)
                        {
                            if (Math.Abs(forces[k].Forces.X / 1000) > N)
                            {
                                N = Math.Abs((float)forces[k].Forces.X / 1000);
                            }
                            if (Math.Abs(forces[k].Forces.Z / 1000) > Vz)
                            {
                                Vz = Math.Abs((float)forces[k].Forces.Z / 1000);
                            }
                            if (Math.Abs(forces[k].Moments.Y / 1000) > My)
                            {
                                My = Math.Abs((float)forces[k].Moments.Y / 1000);
                            }
                        }
                        else
                        {
                            worksheet.Cells[row, "A"] = "CO" + loadingNo;
                            worksheet.Cells[row, "B"] = N;
                            worksheet.Cells[row, "D"] = Vz;
                            worksheet.Cells[row, "F"] = My;
                            worksheet.Cells[row, "H"] = "Member: " + beamNumbers[j];
                            worksheet.Cells[row, "I"] = location;

                            row++;

                            location = forces[k].Location;
                            N = 0f;
                            Vz = 0f;
                            My = 0f;
                            k--;
                        }
                    }

                    //if (checkBox_beamStart.Checked)
                    //{
                    //    N = (float)forces[0].Forces.X / 1000;
                    //    Vy = (float)forces[0].Forces.Y / 1000;
                    //    Vz = (float)forces[0].Forces.Z / 1000;
                    //    Mt = (float)forces[0].Moments.X / 1000;
                    //    My = (float)forces[0].Moments.Y / 1000;
                    //    Mz = (float)forces[0].Moments.Z / 1000;

                    //    int r = row + j * (loadCombinations.Length + 2);

                    //    if (i == 0)
                    //    {
                    //        worksheet.Cells[r - 1, "A"] = "Member: ";
                    //        worksheet.Cells[r - 1, "B"] = beamNumbers[j];
                    //        worksheet.Cells[r - 1, "C"] = "Start";
                    //    }

                    //    worksheet.Cells[r, "A"] = "CO" + loadingNo;
                    //    worksheet.Cells[r, "B"] = N;
                    //    worksheet.Cells[r, "C"] = Vy;
                    //    worksheet.Cells[r, "D"] = Vz;
                    //    worksheet.Cells[r, "E"] = Mt;
                    //    worksheet.Cells[r, "F"] = My;
                    //    worksheet.Cells[r, "G"] = Mz;
                    //}
                    //if (checkBox_beamEnd.Checked)
                    //{
                    //    N = (float)forces[forces.Length-1].Forces.X / 1000;
                    //    Vy = (float)forces[forces.Length - 1].Forces.Y / 1000;
                    //    Vz = (float)forces[forces.Length - 1].Forces.Z / 1000;
                    //    Mt = (float)forces[forces.Length - 1].Moments.X / 1000;
                    //    My = (float)forces[forces.Length - 1].Moments.Y / 1000;
                    //    Mz = (float)forces[forces.Length - 1].Moments.Z / 1000;

                    //    int r = row + j * (loadCombinations.Length + 2);

                    //    if (i == 0)
                    //    {
                    //        worksheet.Cells[r - 1, "J"] = "Member: ";
                    //        worksheet.Cells[r - 1, "K"] = beamNumbers[j];
                    //        worksheet.Cells[r - 1, "L"] = "End";
                    //    }

                    //    worksheet.Cells[r, "J"] = "CO" + loadingNo;
                    //    worksheet.Cells[r, "K"] = N;
                    //    worksheet.Cells[r, "L"] = Vy;
                    //    worksheet.Cells[r, "M"] = Vz;
                    //    worksheet.Cells[r, "N"] = Mt;
                    //    worksheet.Cells[r, "O"] = My;
                    //    worksheet.Cells[r, "P"] = Mz;
                    //}

                }
                //row++;
            }
            
            RFemConnection.UnlockLicense();
        }

        private void button_writeMembers_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }
            AutoCad.ACadManager cadManager = new AutoCad.ACadManager();

            /////////////////
            ///
            string memberString = "4297;3495;3362;3361;4383;3371;3297;3296;3440;3529;3379;3381;3315;3203;3205;3491;3497;3298;3299;3303;3305;3306;3496;3300;3301;3302;3304;3360;3363;3369;3375;3374;4382;4385;3389;3390;3372;3373;3370;4386;4387;3388;3294;3295;3527;3531;3533;3528;3427;3428;3430;3432;3436;3437;3438;3439;3530;3532;3534;3433;3434;3435;3441;3385;3386;3382;3387;3383;3384;3201;3204;3208;3209;3313;3314;3317;3318;3206;3202;3207;3316";
            string nameString = "Kuilu 1: ;Kuilu 2: ;Kuilu 3: ;Kuilu 4: ;Kuilu 5: ;Kuilu 6: ;Kuilu 7: ;Kuilu 8: ;Kuilu 9: ;Kuilu 10: ;Kuilu 11: ;Kuilu 12: ;Kuilu 13: ;Kuilu 14: ;Kuilu 15: ;Seinä 1: ;Seinä 2: ;Seinä 3: ;Seinä 4: ;Seinä 5: ;Seinä 6: ;Seinä 7: ;Seinä 8: ;Seinä 9: ;Seinä 10: ;Seinä 11: ;Seinä 12: ;Seinä 13: ;Seinä 14: ;Seinä 15: ;Seinä 16: ;Seinä 17: ;Seinä 18: ;Seinä 19: ;Seinä 20: ;Seinä 21: ;Seinä 22: ;Seinä 23: ;Seinä 24: ;Seinä 25: ;Seinä 26: ;Seinä 27: ;Seinä 28: ;Seinä 29: ;Seinä 30;Seinä 31;Seinä 32;Seinä 33;Seinä 34;Seinä 35;Seinä 36;Seinä 37;Seinä 38;Seinä 39;Seinä 40;Seinä 41;Seinä 42;Seinä 43;Seinä 44;Seinä 45;Seinä 46;Seinä 47;Seinä 48;Seinä 49;Seinä 50;Seinä 51;Seinä 52;Seinä 53;Seinä 54;Seinä 55;Seinä 56;Seinä 57;Seinä 58;Seinä 59;Seinä 60;Seinä 61;Seinä 62;Seinä 63;Seinä 64;Seinä 65;Seinä 66";
            string kvXstring = "5,09;6,22;2,44;4,66;4,59;10,07;1,19;1,65;5,76;8,54;2,46;4,48;6,16;3,05;7,73;2,14;2,1;7,88;12,19;8,33;8,8;6,1;43,37;47,6;36,14;99,29;99,13;106,02;5,41;3,48;4,49;1,52;4,66;6,86;6,29;4,07;46,45;33,24;33,79;10,39;8,46;7,46;5,89;1,62;2,91;4,2;3,61;2,34;11,84;5,49;2,3;5,27;1,61;1,73;5,35;6,98;4,73;5,12;8,1;15,78;1,99;14,51;1,96;2,6;3,75;1,82;3,8;13,97;3,21;3,16;9,26;1,83;1,35;7,35;7,4;1,66;1,96;0,88;3,43;1,01;2,04";
            string kvYstring = "1,37;3,86;7,63;44,78;1,33;2,68;17,3;20,68;2,32;4,11;8,67;28,11;1,08;1;1,08;69,57;18,8;46,84;86,2;84,21;128,89;169,12;4,71;2,99;2,67;4,95;5,66;10,43;9,37;5,16;61,73;52,46;98,49;8,24;15,41;6,82;1,62;2,18;3,75;4,85;2,44;2,94;13,52;5,41;6,06;10,51;5,85;5,55;6,82;6,7;3,85;88,1;73,85;84,57;4,4;1,11;2,02;2,21;2,1;2;12,12;1,71;13,36;4,6;6,45;3,33;114,95;14,97;5,57;4,68;1,97;3,7;3,67;1,66;2,15;1,08;2,59;0,65;2,25;1,42;12,31";
            string ppXstring = "528,73;779,43;1221,2;444,83;765,51;1027,76;1066,62;1344,92;785,47;1031,59;581,77;945,13;597,74;623,19;422,81;535,55;1476,89;1049,09;1016,2;1046,06;1015,5;1118,02;780,58;858,74;1021,2;1024,19;1142,48;821,75;105,03;870,09;723,36;898,58;1029,88;961,42;956,07;1188,4;898,01;752,26;991,35;1028,38;949,48;1064,44;852,98;289,27;1048,34;691,39;688,41;962,57;985,08;1172,12;1400,65;1017,98;1048,42;1176,65;862,58;904,95;806,66;638,83;797,25;714,94;367,22;1006,95;482,5;368,38;386,73;456,14;495,7;757,78;402,21;673,09;650,66;480,97;606,46;837,76;418,98;322,43;463,07;234,41;685,93;270,25;489,71";
            string ppYstring = "654,6;834,37;1221,2;444,83;765,51;1027,76;1066,62;1344,92;785,47;1031,59;581,77;945,13;597,74;623,19;422,81;526,49;1325,4;1049,09;1016,2;1046,06;1015,5;1118,02;780,58;858,74;1021,2;1024,19;1142,48;821,75;105,03;870,09;723,36;898,58;1029,88;961,42;956,07;1188,4;898,01;752,26;991,35;1028,38;949,48;1064,44;852,98;289,27;1048,34;691,39;688,41;962,57;985,08;1172,12;1400,65;1017,98;1048,42;1176,65;862,58;904,95;806,66;638,83;797,25;714,94;367,22;1006,95;482,5;368,38;386,73;456,14;495,7;757,78;402,21;673,09;650,66;480,97;606,46;837,76;418,98;322,43;463,07;234,41;685,93;270,25;489,71";
            string dimensionString = "9,2x3,8x1;9,14x3,14x1;7,81x3,14x1;7,81x6,86x1;9,14x3,14x1;9,14x3,14x1;5,19x3,54x1;5,6x3,16x1;9,14x3,14x1;9,14x3,14x1;7,81x6,86x1;7,81x3,14x1;9,2x3,4x1;11,39x3,8x1;3,64x8,5x1;51,88x1x1;4,05x1x1;7,33x1x1;25,67x1x1;6,17x1x1;10,2x1x1;29,7x1x1;1x22,35x1;1x6,15x1;1x12,85x1;1x6,15x1;1x9,5x1;1x23,19x1;1x23,19x1;33,2x1x1;6,15x1x1;10,2x1x1;25,67x1x1;10,2x1x1;6,17x1x1;27x1x1;1x9,5x1;1x12,85x1;1x6,16x1;1x6,16x1;1x12,85x1;1x9,5x1;1x23,22x1;1x23,22x1;27,17x1x1;6,15x1x1;10,2x1x1;28,15x1x1;14,89x1x1;8,08x1x1;4,03x1x1;9,67x1x1;5,6x1x1;6,7x1x1;9,48x1x1;14,89x1x1;1x9,5x1;1x12,85x1;1x6,13x1;1x13,4x1;1x25x1;1x9,5x1;1x28,48x1;6,03x1x1;6,03x1x1;1x23,2x1;1x13,9x1;1x13,9x1;1x9,3x1;39,32x1x1;56,7x1x1;6x1x1;6x1x1;22,97x1x1;25,67x1x1;26,4x1x1;26,4x1x1;1x9,5x1;1x9,5x1;1x9,5x1;1x28,5x1";
            string fzString = "17877;21304;27525;22921;21017;28897;15174;20142;21698;28782;28829;22474;18068;25041;12268;27206;5060;7619;25991;6247;10125;32988;17112;5244;12944;6298;10828;18676;2412;28818;4303;8479;26425;9586;5805;31428;8486;9468;6052;6257;12001;10054;19658;6264;28308;4075;6890;26643;14451;9200;4632;9675;4537;7689;7953;13195;7608;8081;4809;8794;5756;8704;13716;1924;2117;10259;6861;8999;3710;25953;36837;2766;3016;19088;10621;7967;11555;2180;6484;2503;13948";
            string fyString = "2317;198;565;75;2192;1063;219;62;904;1227;536;375;3403;2750;6618;143;74;68;106;27;26;57;2014;873;1336;465;762;355;149;1751;19;42;86;370;136;1418;1391;1448;444;481;1441;1513;816;532;1470;140;376;1583;666;418;218;34;19;26;581;3751;999;1317;754;1750;1173;1146;2061;134;103;329;92;862;321;1824;5973;217;230;3943;1733;2136;1404;1957;4150;2053;2788";
            string fxString = "1888;386;1354;537;1628;705;926;811;773;724;3634;1335;967;1162;452;3515;1301;536;1105;615;524;1260;121;33;149;22;43;20;219;3548;1008;904;1474;937;816;2175;67;108;38;166;431;334;1215;1434;2164;819;1140;2606;751;782;325;367;531;1323;1051;1512;339;458;142;106;914;139;2248;442;312;2157;576;32;400;1864;5853;1634;1360;2309;457;972;659;632;626;661;2261";
            string mzString = "5027;502;2186;611;3805;1030;493;531;1286;2314;3918;767;283;3318;1662;1618;167;264;168;345;576;967;340;309;318;394;624;36;477;1413;204;422;1016;502;271;2380;476;360;164;261;774;537;458;1572;2091;191;357;5969;612;148;158;179;76;100;72;802;396;830;252;286;2164;309;1597;59;26;4297;68;343;466;14161;7870;52;27;1848;1502;2913;1486;128;4;97;498";
            string myString = "10200;14828;33692;13226;14573;9022;24878;24323;12203;11483;31688;12349;9093;42521;2170;273531;2732;761;17500;1662;4648;64414;58;10;30;8;6;64;4;89226;1433;21015;53762;4944;2098;90057;18;33;28;16;33;13;29;79;95501;2164;5925;122976;8341;4352;2768;5596;7128;4432;4582;10534;11;21;15;135;573;147;11;1838;1453;56;5;256;5;135070;69191;1166;3917;20303;14799;63304;77806;8;6;11;5";
            string mxString = "14551;5831;4659;1412;14459;10608;1128;1377;9613;6452;9948;649;16021;31671;32116;22;53;13;37;8;9;30;30876;3053;25831;2019;5249;17869;2798;24;8;17;25;11;8;16;10167;23787;3467;2613;26791;6892;15491;11604;19;8;8;19;8;17;1;18;11;17;43;16;6999;19395;2702;25145;5097;13067;7514;4;9;35172;342;3230;2713;13;9;17;12;17;13;16;10;6596;6092;5307;9402";

            string[] memberStringArray = memberString.Split(';');
            string[] nameStringArray = nameString.Split(';');
            string[] kvXstringArray = kvXstring.Split(';');
            string[] kvYstringArray = kvYstring.Split(';');
            string[] ppXstringArray = ppXstring.Split(';');
            string[] ppYstringArray = ppYstring.Split(';');
            string[] dimensionStringArray = dimensionString.Split(';');
            string[] fzStringArray = fzString.Split(';');
            string[] fyStringArray = fyString.Split(';');
            string[] fxStringArray = fxString.Split(';');
            string[] mzStringArray = mzString.Split(';');
            string[] myStringArray = myString.Split(';');
            string[] mxStringArray = mxString.Split(';');

            Dictionary<string, string> names = new Dictionary<string, string>();
            Dictionary<string, string> kvX = new Dictionary<string, string>();
            Dictionary<string, string> kvY = new Dictionary<string, string>();
            Dictionary<string, string> ppX = new Dictionary<string, string>();
            Dictionary<string, string> ppY = new Dictionary<string, string>();
            Dictionary<string, string> dimensions = new Dictionary<string, string>();
            Dictionary<string, string> fz = new Dictionary<string, string>();
            Dictionary<string, string> fy = new Dictionary<string, string>();
            Dictionary<string, string> fx = new Dictionary<string, string>();
            Dictionary<string, string> mz = new Dictionary<string, string>();
            Dictionary<string, string> my = new Dictionary<string, string>();
            Dictionary<string, string> mx = new Dictionary<string, string>();

            for (int i = 0; i < memberStringArray.Length; i++)
            {
                names.Add(memberStringArray[i], nameStringArray[i]);
                kvX.Add(memberStringArray[i], kvXstringArray[i]);
                kvY.Add(memberStringArray[i], kvYstringArray[i]);
                ppX.Add(memberStringArray[i], ppXstringArray[i]);
                ppY.Add(memberStringArray[i], ppYstringArray[i]);
                dimensions.Add(memberStringArray[i], dimensionStringArray[i]);
                fz.Add(memberStringArray[i], fzStringArray[i]);
                fy.Add(memberStringArray[i], fyStringArray[i]);
                fx.Add(memberStringArray[i], fxStringArray[i]);
                mz.Add(memberStringArray[i], mzStringArray[i]);
                my.Add(memberStringArray[i], myStringArray[i]);
                mx.Add(memberStringArray[i], mxStringArray[i]);
            }
            /////////////////
            ///

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();
            Member[] members = iModelData.GetMembers();
            

            foreach (Member member in members)
            {
                if (member.Type != MemberType.ResultBeamType)
                {
                    continue;
                }

                Vector3 lineCoordinate = RFem.GetResults.GetLineCoordinate(iModel, member.LineNo);
                Vector3 point = new Vector3(lineCoordinate.X, lineCoordinate.Y, 0);

                string memberNumber = member.No.ToString();
                string s_name = "";
                string s_kvx = "";
                string s_kvy = "";
                string s_ppx = "";
                string s_ppy = "";
                string s_dimension = "";
                string s_fz = "";
                string s_fy = "";
                string s_fx = "";
                string s_mz = "";
                string s_my = "";
                string s_mx = "";
                names.TryGetValue(memberNumber, out s_name);
                kvX.TryGetValue(memberNumber, out s_kvx);
                kvY.TryGetValue(memberNumber, out s_kvy);
                ppX.TryGetValue(memberNumber, out s_ppx);
                ppY.TryGetValue(memberNumber, out s_ppy);
                dimensions.TryGetValue(memberNumber, out s_dimension);
                fz.TryGetValue(memberNumber, out s_fz);
                fy.TryGetValue(memberNumber, out s_fy);
                fx.TryGetValue(memberNumber, out s_fx);
                mz.TryGetValue(memberNumber, out s_mz);
                my.TryGetValue(memberNumber, out s_my);
                mx.TryGetValue(memberNumber, out s_mx);

                string text = s_name + "\n" + 
                                "Member: " + member.No + "\n" +
                                s_dimension + "\n" +
                                "Kaatumisvarmuus: " + "\n" +
                                "x: " + s_kvx + "\n" +
                                "y: " + s_kvy + "\n" +
                                "Pohjapaine [kN/m2]: " + "\n" +
                                "x: " + s_ppx + "\n" +
                                "y: " + s_ppy + "\n" +
                                "Suurin puristus [kN]: " + s_fz + "\n" +
                                "Suurimmat leikkausvoimat [kN]: " + "\n" +
                                "x: " + s_fx + "\n" +
                                "y: " + s_fy + "\n" +
                                "Suurimmat taivutusmomentit [kNm]: " + "\n" +
                                "x: " + s_mx + "\n" +
                                "y: " + s_my;

                cadManager.SendText(point, text, 10000, 250);
            }

            RFemConnection.UnlockLicense();
        }

        private void textBox_LoadingNumber_TextChanged(object sender, EventArgs e)
        {
            int value = 1;
            try
            {
                value = int.Parse(textBox_LoadingNumber.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers allowed!", "Error!");
                textBox_LoadingNumber.Text = value.ToString();
            }
        }

        private void textBox_columnStartHeight_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            try
            {
                value = int.Parse(textBox_columnStartHeight.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers allowed!", "Error!");
                textBox_columnStartHeight.Text = value.ToString();
            }
        }

        private void textBox_columnLength_TextChanged(object sender, EventArgs e)
        {
            int value = 1000;
            try
            {
                value = int.Parse(textBox_columnLength.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers allowed!", "Error!");
                textBox_columnLength.Text = value.ToString();
            }
        }

        private void textBox_startHinge_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            try
            {
                value = int.Parse(textBox_startHinge.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers allowed!", "Error!");
                textBox_startHinge.Text = value.ToString();
            }
            if (value < 0)
            {
                MessageBox.Show("Must be >= 0 !", "Error!");
                textBox_startHinge.Text = "0";
            }
        }

        private void textBox_endHinge_TextChanged(object sender, EventArgs e)
        {
            int value = 0;
            try
            {
                value = int.Parse(textBox_endHinge.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers allowed!", "Error!");
                textBox_endHinge.Text = value.ToString();
            }
            if (value < 0)
            {
                MessageBox.Show("Must be >= 0 !", "Error!");
                textBox_endHinge.Text = "0";
            }
        }

        private void textBox_crossSection_TextChanged(object sender, EventArgs e)
        {
            int value = 1;
            try
            {
                value = int.Parse(textBox_crossSection.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers allowed!", "Error!");
                textBox_crossSection.Text = value.ToString();
            }
            if (value <= 0)
            {
                MessageBox.Show("Must be > 0 !", "Error!");
                textBox_crossSection.Text = "1";
            }
        }

        private void textBox_beamHeight_TextChanged(object sender, EventArgs e)
        {
            int value = int.Parse(textBox_columnStartHeight.Text) + int.Parse(textBox_columnLength.Text);
            try
            {
                value = int.Parse(textBox_beamHeight.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Only numbers allowed!", "Error!");
                textBox_beamHeight.Text = value.ToString();
            }
        }


        //////////////////
        /////////////////
        ////////////////
        private void button_unlock_Click(object sender, EventArgs e)
        {
            RFemConnection.UnlockLicense();
        }

        private void checkBox_OnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = checkBox_OnTop.Checked;
        }

        private void button_maxN_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            LoadingType loadingType = LoadingType.ResultCombinationType;
            int loadingNumber = int.Parse(textBox_LoadingNumber.Text);
            RFem.LoadingCase loadingCase = new RFem.LoadingCase(loadingNumber, loadingType);

            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }
            AutoCad.ACadManager cadManager = new AutoCad.ACadManager();

            bool nodalSupport = true;
            bool lineSupport = true;

            IModel iModel = RFemConnection.RfModel;
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();
            //RFem.LoadingCase[] loadingCases = { new RFem.LoadingCase(6, loadingType), new RFem.LoadingCase(5, loadingType) };
            RFem.LoadingCase[] loadingCases = { new RFem.LoadingCase(6, loadingType) };

            for (int i = 0; i < loadingCases.Length; i++)
            {
                IResults iResults = RFem.Utility.GetResults(iModel, loadingCases[i]);

                if (nodalSupport)
                {
                    NodalSupportForces[] nodalSF = RFem.GetResults.SupportReactions.GetNodalReactions(iModel, iResults);

                    foreach (NodalSupportForces sf in nodalSF)
                    {
                        if (sf.Type == ResultsValueType.MinimumValueType)
                        {
                            Vector3 coordinates = RFem.GetResults.GetNodeCoordinates(iModel, sf.NodeNo);
                            Vector3 point = new Vector3(coordinates.X, coordinates.Y, 0);

                            string text = "Fz = " + Math.Round(sf.Forces.Z / 1000, 3).ToString() + " kN";

                            //string layer = "";

                            //if (loadingCases[i].loadingNumber == 5)
                            //{
                            //    layer = "__ULS_";
                            //}
                            //else if (loadingCases[i].loadingNumber == 6)
                            //{
                            //    layer = "__SLS_";
                            //}

                            //cadManager.SendText(point, text, 10000, 250, layer);
                            cadManager.SendText(point, text, 10000, 250);
                        }
                    }
                }

                if (lineSupport)
                {
                    LineSupportForces[] lineSF = RFem.GetResults.SupportReactions.GetLineReactions(iModel, iResults);
                    List<LineSupportForces> supportForces = lineSF.OrderBy(o => o.LineNo).ToList();
                    int lineNo = int.MaxValue;
                    List<LineForces> lineForces = new List<LineForces>();
                    int n = 0;

                    foreach (LineSupportForces sf in supportForces)
                    {
                        if (sf.LineNo != lineNo)
                        {
                            lineNo = sf.LineNo;

                            ILine iLine = iModelData.GetLine(lineNo, ItemAt.AtNo);
                            Line line = iLine.GetData();

                            lineForces.Add(new LineForces(line));
                        }

                        n = lineForces.Count - 1;
                        lineForces[n].supportReactions.Add(sf);
                    }

                    foreach (LineForces force in lineForces)
                    {
                        float f = force.LineForceZ;

                        //string delta = "";
                        //for (int k = 0; k < force.delta.Count; k++)
                        //{
                        //    delta = delta + Math.Round(force.delta[k], 3) + ", ";
                        //}
                        //string q = "";
                        //for (int k = 0; k < force.supportReactions.Count; k++)
                        //{
                        //    if (force.supportReactions[k].Type == ResultsValueType.MinimumValueType)
                        //    {
                        //        q = q + Math.Round(force.supportReactions[k].Forces.Z / 1000, 3) + ", ";
                        //    }
                        //}
                        //string text = "Line: " + force.line.No + "\n" +
                        //    "Length: " + Math.Round(force.line.Length, 3) + "\n" +
                        //    "Div: " + force.delta.Count + "\n" +
                        //    f.ToString() + "\n" +
                        //    "Delta: " + delta + "\n" +
                        //    "Q: " + q;
                        string text = "Fz = " + f.ToString() + " kN/m";

                        //string layer = "";

                        //if (loadingCases[i].loadingNumber == 5)
                        //{
                        //    layer = "__ULS_";
                        //}
                        //else if (loadingCases[i].loadingNumber == 6)
                        //{
                        //    layer = "__SLS_";
                        //}

                        //cadManager.SendText(force.Coordinate, text, 10000, 250, layer);
                        cadManager.SendText(force.Coordinate, text, 10000, 250);
                    }
                }
            }

            iModel.GetApplication().UnlockLicense();
        }

        private void button_GetLayers_Click(object sender, EventArgs e)
        {
            List<string> layers = ACadManager.ACad_GetLayers();
            string ark = this.comboBox_ArkLayer.Text;
            string column = this.textBox_columnLayer.Text;
            string beam = this.textBox_beamLayer.Text;
            this.comboBox_ArkLayer.Items.Clear();
            this.comboBox_ColumnLayer.Items.Clear();
            this.comboBox_BeamLayer.Items.Clear();
            this.comboBox_deleteLayer.Items.Clear();

            foreach (string layer in layers)
            {
                this.comboBox_ArkLayer.Items.Add(layer);
                this.comboBox_ColumnLayer.Items.Add(layer);
                this.comboBox_BeamLayer.Items.Add(layer);
                this.comboBox_deleteLayer.Items.Add(layer);

                if (layer == ark)
                {
                    this.comboBox_ArkLayer.SelectedItem = layer;
                }
                if (layer == column)
                {
                    this.comboBox_ColumnLayer.SelectedItem = layer;
                }
                if (layer == beam)
                {
                    this.comboBox_BeamLayer.SelectedItem = layer;
                }
            }
            if (this.comboBox_ArkLayer.SelectedItem == null)
            {
                this.comboBox_ArkLayer.SelectedItem = comboBox_ArkLayer.Items[0];
            }
            if (this.comboBox_ColumnLayer.SelectedItem == null)
            {
                this.comboBox_ColumnLayer.SelectedItem = comboBox_ColumnLayer.Items[0];
            }
            if (this.comboBox_BeamLayer.SelectedItem == null)
            {
                this.comboBox_BeamLayer.SelectedItem = comboBox_BeamLayer.Items[0];
            }
        }

        private void button_markColumns_Click(object sender, EventArgs e)
        {
            ACadDataManager.MarkColumns(ACadManager.GetDocument(), (string)comboBox_ArkLayer.SelectedItem, textBox_columnLayer.Text, 300);
        }

        private void button_markBeams_Click(object sender, EventArgs e)
        {
            ACadDataManager.MarkBeams(ACadManager.GetDocument(), (string)comboBox_ColumnLayer.SelectedItem, textBox_beamLayer.Text, radioButton_beamDirX.Checked);
        }

        private void button_columnsToRFem_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }
            string layer = comboBox_ColumnLayer.Text;
            float startHeight = float.Parse(textBox_columnStartHeight.Text);
            float endHeight = startHeight + float.Parse(textBox_columnLength.Text);
            int crossSection = int.Parse(textBox_crossSection.Text);
            int startHinge = int.Parse(textBox_startHinge.Text);
            int endHinge = int.Parse(textBox_endHinge.Text);

            List<double[]> columns = ACadDataManager.GetColumns(ACadManager.GetDocument().Database, layer);
            int digits = 2;

            foreach (double[] column in columns)
            {
                Vector3 startPoint = new Vector3((float)Math.Round(column[0] / 1000, digits), (float)Math.Round(column[1] / 1000, digits), startHeight / 1000);
                Vector3 endPoint = new Vector3((float)Math.Round(column[0] / 1000, digits), (float)Math.Round(column[1] / 1000, digits), endHeight / 1000);

                rfModelBuilder.AddMember(startPoint, endPoint, modelName, crossSection, startHinge, endHinge);
            }
        }

        private void button_beamsToRFem_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }
            string layer = comboBox_BeamLayer.Text;
            float beamHeight = float.Parse(textBox_beamHeight.Text);
            int crossSection = int.Parse(textBox_crossSection.Text);
            int startHinge = int.Parse(textBox_startHinge.Text);
            int endHinge = int.Parse(textBox_endHinge.Text);

            List<double[]> beams = ACadDataManager.GetBeams(ACadManager.GetDocument().Database, layer);
            int digits = 2;

            foreach (double[] beam in beams)
            {
                Vector3 startPoint = new Vector3((float)Math.Round(beam[0] / 1000, digits), (float)Math.Round(beam[1] / 1000, digits), (float)Math.Round(beam[2] / 1000, digits) + (beamHeight / 1000));
                Vector3 endPoint = new Vector3((float)Math.Round(beam[3] / 1000, digits), (float)Math.Round(beam[4] / 1000, digits), (float)Math.Round(beam[5] / 1000, digits) + (beamHeight / 1000));

                rfModelBuilder.AddMember(startPoint, endPoint, modelName, crossSection, startHinge, endHinge);
            }
        }

        private void button_deleteLayer_Click(object sender, EventArgs e)
        {
            if (comboBox_deleteLayer.SelectedItem == null)
            {
                return;
            }

            string layer = (string)comboBox_deleteLayer.SelectedItem;
            ACadLayerManager.DeleteLayer(ACadManager.GetDocument(), layer);
        }

        private void button_getLoadings_Click(object sender, EventArgs e)
        {
            //rfLoadings.XmlSerializeLoadCases(@"C:\Users\TuHan\OneDrive - A-Insinöörit Oy\Työpöytä");
            //rfLoadings.XmlSerializeCrossSections(@"C:\Users\TuHan\OneDrive - A-Insinöörit Oy\Työpöytä");
            //rfLoadings.XmlDeserializeLoadingData();
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }
            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;
            rfModelBuilder.WriteProfiles(modelName);

            //rfModelBuilder.AddNode(iModel, Vector3.One);
            //iModel.GetApplication().LockLicense();
            //IModelData iModelData = iModel.GetModelData();
            //MODEL.LineManager lineManager = new MODEL.LineManager();
            //for (int i = 0; i < iModelData.GetLineCount(); i++)
            //{
            //    ILine iLine = iModelData.GetLine(i, ItemAt.AtIndex);
            //    Line line = iLine.GetData();
            //    string[] nodes = line.NodeList.Split(',');
            //    int nodeNo = int.Parse(nodes[0]);
            //    INode iNode = iModelData.GetNode(nodeNo, ItemAt.AtNo);
            //    Node node = iNode.GetData();
            //    Vector3 startPoint = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
            //    nodeNo = int.Parse(nodes[nodes.Length - 1]);
            //    iNode = iModelData.GetNode(nodeNo, ItemAt.AtNo);
            //    node = iNode.GetData();
            //    Vector3 endPoint = new Vector3((float)node.X, (float)node.Y, (float)node.Z);
            //    lineManager.AddLine(startPoint, endPoint);
            //}
            //iModel.GetApplication().UnlockLicense();
            //lineManager.pointManager.GetNode(Vector3.Zero);
        }

        private void button_beams_RFemToTekla_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            List<Vector3[]> coordinates = rfModelBuilder.GetMemberCoordinates(rfModelBuilder.GetMembers(modelName), modelName);
            
            foreach (Vector3[] coordinate in coordinates)
            {
                teklaModelManager.AddBeam(coordinate[0], coordinate[1]);
            }
        }

        private void button_getProfiles_Click(object sender, EventArgs e)
        {
            teklaModelManager.GetPartData();
            //string modelName = "";
            //if (comboBox_ModelName.SelectedItem != null)
            //{
            //    modelName = comboBox_ModelName.SelectedItem.ToString();
            //}
            //rfModelBuilder.AddMaterial(modelName);
            //Mapping.ProfileMapping.GetProfiles();
        }

        private void button_copyLayerSettings_Click(object sender, EventArgs e)
        {
            ACadLayerManager.GetLayerStates(ACadManager.GetDocument());
        }

        private void button_pasteLayerSettings_Click(object sender, EventArgs e)
        {
            if (ACadLayerManager.layerStates.Count < 1)
            {
                return;
            }

            ACadLayerManager.SetLayerStates(ACadManager.GetDocument(), ACadLayerManager.layerStates);
        }

        private void button_getUCS_Click(object sender, EventArgs e)
        {
            ACadManager.GetUCS();
        }

        private void button_setUCS_Click(object sender, EventArgs e)
        {
            //ACadManager.SetUCS();
            ACadManager.ModifyLine();
            ACadManager.FitLine2(500f, "AINS_FITLINE");
            ACadManager.ModifyLine();
        }

        private void button_getForceResultant_Click(object sender, EventArgs e)
        {
            Excel.Application excelApp = Marshal.GetActiveObject("Excel.Application") as Excel.Application;
            excelApp.Visible = true;
            Excel.Workbook workbook = excelApp.ActiveWorkbook;
            Excel.Worksheet worksheet = workbook.Worksheets.Add();

            int[] loadCombinations = { 15,16,17,18,19,20,21,22,23,24,25,26,27,48,49,50,51,52,53,54,55,56,57,58,59,60 };//15-33, 48-66
            int[] beamNumbers = { 5833 }; //5820, 5832, 5833

            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;

            // Establish column headings
            int row = 8;

            if (checkBox_beamStart.Checked)
            {
                worksheet.Cells[row, "A"] = "CO";
                worksheet.Cells[row, "B"] = "N [kN]";
                worksheet.Cells[row, "C"] = "Vy [kN]";
                worksheet.Cells[row, "D"] = "Vz [kN]";
                worksheet.Cells[row, "E"] = "Mt [kNm]";
                worksheet.Cells[row, "F"] = "My [kNm]";
                worksheet.Cells[row, "G"] = "Mz [kNm]";
            }

            row = row + 2;

            double[] maxN = { double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue };
            double[] minN = { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MinValue };
            double[] maxVy = { double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue };
            double[] minVy = { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MinValue };
            double[] maxVz = { double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue };
            double[] minVz = { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MinValue };
            double[] maxMy = { double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue };
            double[] minMy = { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MinValue };
            double[] maxMz = { double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue };
            double[] minMz = { double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MinValue };

            for (int i = 0; i < loadCombinations.Length; i++)
            {
                int loadingNo = loadCombinations[i];
                RFem.LoadingCase loadingCase = new RFem.LoadingCase(loadingNo, LoadingType.LoadCombinationType);
                IResults iResults = RFem.Utility.GetResults(iModel, loadingCase);

                for (int j = 0; j < beamNumbers.Length; j++)
                {
                    MemberForces[] forces = iResults.GetMemberInternalForces(beamNumbers[j], ItemAt.AtNo, true);

                    double location = 40.5f; //31.5f, 40.5f

                    for (int k = 0; k < forces.Length; k++)
                    {
                        if (Math.Abs(forces[k].Location - location) < 0.5)
                        {
                            if (forces[k].Forces.X > maxN[0])
                            {
                                maxN = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Forces.X < minN[0])
                            {
                                minN = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Forces.Y > maxVy[1])
                            {
                                maxVy = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Forces.Y < minVy[1])
                            {
                                minVy = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Forces.Z > maxVz[2])
                            {
                                maxVz = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Forces.Z < minVz[2])
                            {
                                minVz = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }

                            if (forces[k].Moments.Y > maxMy[4])
                            {
                                maxMy = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Moments.Y < minMy[4])
                            {
                                minMy = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Moments.Z > maxMz[5])
                            {
                                maxMz = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                            if (forces[k].Moments.Z < minMz[5])
                            {
                                minMz = new double[] { forces[k].Forces.X, forces[k].Forces.Y, forces[k].Forces.Z, forces[k].Moments.X, forces[k].Moments.Y, forces[k].Moments.Z, (double)loadingNo };
                            }
                        }
                    }
                }
            }

            worksheet.Cells[row, "A"] = Math.Round(maxN[6]);
            worksheet.Cells[row, "B"] = Math.Round(maxN[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(maxN[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(maxN[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(maxN[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(maxN[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(maxN[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(minN[6]);
            worksheet.Cells[row, "B"] = Math.Round(minN[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(minN[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(minN[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(minN[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(minN[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(minN[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(maxVy[6]);
            worksheet.Cells[row, "B"] = Math.Round(maxVy[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(maxVy[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(maxVy[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(maxVy[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(maxVy[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(maxVy[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(minVy[6]);
            worksheet.Cells[row, "B"] = Math.Round(minVy[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(minVy[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(minVy[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(minVy[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(minVy[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(minVy[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(maxVz[6]);
            worksheet.Cells[row, "B"] = Math.Round(maxVz[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(maxVz[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(maxVz[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(maxVz[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(maxVz[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(maxVz[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(minVz[6]);
            worksheet.Cells[row, "B"] = Math.Round(minVz[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(minVz[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(minVz[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(minVz[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(minVz[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(minVz[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(maxMy[6]);
            worksheet.Cells[row, "B"] = Math.Round(maxMy[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(maxMy[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(maxMy[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(maxMy[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(maxMy[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(maxMy[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(minMy[6]);
            worksheet.Cells[row, "B"] = Math.Round(minMy[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(minMy[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(minMy[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(minMy[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(minMy[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(minMy[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(maxMz[6]);
            worksheet.Cells[row, "B"] = Math.Round(maxMz[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(maxMz[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(maxMz[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(maxMz[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(maxMz[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(maxMz[5] / 1000);

            row++;

            worksheet.Cells[row, "A"] = Math.Round(minMz[6]);
            worksheet.Cells[row, "B"] = Math.Round(minMz[0] / 1000);
            worksheet.Cells[row, "C"] = Math.Round(minMz[1] / 1000);
            worksheet.Cells[row, "D"] = Math.Round(minMz[2] / 1000);
            worksheet.Cells[row, "E"] = Math.Round(minMz[3] / 1000);
            worksheet.Cells[row, "F"] = Math.Round(minMz[4] / 1000);
            worksheet.Cells[row, "G"] = Math.Round(minMz[5] / 1000);

            row++;

            RFemConnection.UnlockLicense();
        }

        private void button_modifySlabs_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();
            List<Beam> beams = new List<Beam>();
            List<ContourPlate> plates = new List<ContourPlate>();

            HashSet<Tekla.Structures.Geometry3d.Point> pointSet = new HashSet<Tekla.Structures.Geometry3d.Point>();
            //double x,y,z;
            double tolerance = 10;

            foreach (ModelObject mo in selectedObjects)
            {
                Beam beam = mo as Beam;
                ContourPlate plate = mo as ContourPlate;
                BooleanPart cut = mo as BooleanPart;

                if (cut != null)
                {
                    beam = cut.OperativePart as Beam;
                    plate = cut.OperativePart as ContourPlate;
                    //beam.GetSolid();
                    //plate.GetSolid();
                }
                if (beam != null)
                {
                    //string profile = beam.Profile.ProfileString;

                    //string[] t =  profile.Split('*');

                    //beam.Profile.ProfileString = "2935*" + t[1];

                    beams.Add(beam);

                    //beam.Modify();
                }
                else if (plate != null)
                {
                    plates.Add(plate);
                }
            }

            for (int i = 0; i < beams.Count; i++)
            {
                //x = beams[i].StartPoint.X;
                //y = beams[i].StartPoint.Y;
                //z = beams[i].StartPoint.Z;

                if(!pointSet.Contains(beams[i].StartPoint)) pointSet.Add(beams[i].StartPoint);
                if (!pointSet.Contains(beams[i].EndPoint)) pointSet.Add(beams[i].EndPoint);
            }

            for (int i = 0; i < plates.Count; i++)
            {
                ArrayList points = plates[i].Contour.ContourPoints;
                //List<Tekla.Structures.Geometry3d.Point> pointList = new List<Tekla.Structures.Geometry3d.Point>();

                foreach (object p in points)
                {
                    Tekla.Structures.Geometry3d.Point point = p as Tekla.Structures.Geometry3d.Point;

                    if (point != null)
                    {
                        //pointList.Add(point);
                        if (!pointSet.Contains(point)) pointSet.Add(point);
                    }
                }
            }

            foreach (var p in pointSet)
            {
                for (int i = 0; i < beams.Count; i++)
                {
                    ModifyPoints(beams[i], p, tolerance);
                }

                for (int i = 0; i < plates.Count; i++)
                {
                    ModifyPoints(plates[i], p, tolerance);
                }
            }

            Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();

            model.CommitChanges();
        }

        void ModifyPoints(Part part, Tekla.Structures.Geometry3d.Point point, double tolerance)
        {
            Beam beam = part as Beam;
            ContourPlate plate = part as ContourPlate;

            bool modify = false;

            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            if (beam != null)
            {
                if (beam.StartPoint.X - x < tolerance && beam.StartPoint.X - x > 0.01)
                {
                    beam.StartPoint.X = x;
                    modify = true;
                }
                if (beam.StartPoint.Y - y < tolerance && beam.StartPoint.Y - y > 0.01)
                {
                    beam.StartPoint.Y = y;
                    modify = true;
                }
                if (beam.StartPoint.Z - z < tolerance && beam.StartPoint.Z - z > 0.01)
                {
                    beam.StartPoint.Z = z;
                    modify = true;
                }

                if (beam.EndPoint.X - x < tolerance && beam.EndPoint.X - x > 0.01)
                {
                    beam.EndPoint.X = x;
                    modify = true;
                }
                if (beam.EndPoint.Y - y < tolerance && beam.EndPoint.Y - y > 0.01)
                {
                    beam.EndPoint.Y = y;
                    modify = true;
                }
                if (beam.EndPoint.Z - z < tolerance && beam.EndPoint.Z - z > 0.01)
                {
                    beam.EndPoint.Z = z;
                    modify = true;
                }

                if (modify)
                {
                    try
                    {
                        beam.Modify();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else if (plate != null)
            {
                ArrayList points = plate.Contour.ContourPoints;
                Contour contour = new Contour();

                for (int i = 0; i < points.Count; i++)
                {
                    Tekla.Structures.Geometry3d.Point platePoint = points[i] as Tekla.Structures.Geometry3d.Point;
                    Tekla.Structures.Geometry3d.Point cPoint = platePoint;

                    if (platePoint != null)
                    {
                        if (platePoint.X - x < tolerance && platePoint.X - x > 0.01)
                        {
                            points[i] = new Tekla.Structures.Geometry3d.Point(x, platePoint.Y, platePoint.Z);
                            cPoint = new Tekla.Structures.Geometry3d.Point(x, platePoint.Y, platePoint.Z);
                            modify = true;
                        }
                        if (platePoint.Y - y < tolerance && platePoint.Y - y > 0.01)
                        {
                            points[i] = new Tekla.Structures.Geometry3d.Point(platePoint.X, y, platePoint.Z);
                            cPoint = new Tekla.Structures.Geometry3d.Point(platePoint.X, y, platePoint.Z);
                            modify = true;
                        }
                        if (platePoint.Z - z < tolerance && platePoint.Z - z > 0.01)
                        {
                            points[i] = new Tekla.Structures.Geometry3d.Point(platePoint.X, platePoint.Y, z);
                            cPoint = new Tekla.Structures.Geometry3d.Point(platePoint.X, platePoint.Y, z);
                            modify = true;
                        }

                        Chamfer chamfer = new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_NONE);
                        ContourPoint contourPoint = new ContourPoint(cPoint, chamfer);
                        contour.AddContourPoint(contourPoint);
                    }
                }

                if (modify)
                {
                    plate.Contour = contour;
                    plate.Modify();

                    //ContourPlate contourPlate = new ContourPlate();
                    //contourPlate.AssemblyNumber = plate.AssemblyNumber;
                    //contourPlate.CastUnitType = plate.CastUnitType;
                    //contourPlate.Class = plate.Class;
                    //contourPlate.Contour = contour;
                    //contourPlate.DeformingData = plate.DeformingData;
                    //contourPlate.Finish = plate.Finish;
                    //contourPlate.Material = plate.Material;
                    //contourPlate.Name = plate.Name;
                    //contourPlate.Position = plate.Position;
                    //contourPlate.Profile = plate.Profile;
                    //contourPlate.PartNumber = plate.PartNumber;
                    //contourPlate.Insert();

                    //Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
                    //model.CommitChanges();
                }
            }
        }

        private void button_testScpipt_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
            //Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            //ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();
            ModelObjectSelector mos = model.GetModelObjectSelector();
            ModelObjectEnumerator selectedObjects = mos.GetAllObjects();
            List<Beam> beams = new List<Beam>();
            List<ContourPlate> plates = new List<ContourPlate>();
            List<BaseComponent> components = new List<BaseComponent>();
            List<Reinforcement> rebars = new List<Reinforcement>();
            List<ControlPoint> points = new List<ControlPoint>();

            foreach (ModelObject mo in selectedObjects)
            {
                Beam beam = mo as Beam;
                ContourPlate plate = mo as ContourPlate;
                BaseComponent component = mo as BaseComponent;
                Reinforcement rebar = mo as Reinforcement;
                ControlPoint point = mo as ControlPoint;

                if (beam != null)
                {
                    beams.Add(beam);
                }
                else if (plate != null)
                {
                    plates.Add(plate);
                }
                else if (component != null)
                {
                    //component.GetChildren();
                    //component.Delete();
                    components.Add(component);                   
                }
                else if (rebar != null)
                {
                    //rebar.Delete();
                    rebars.Add(rebar);
                }
                else if (point != null)
                {
                    //point.Delete();
                    points.Add(point);
                }
            }

            model.CommitChanges();
        }

        void MoveRefLine(Part part)
        {
            Beam beam = part as Beam;
            ContourPlate plate = part as ContourPlate;

            if (beam != null)
            {
                beam.GetCenterLine(true);

            }
            else if (plate != null)
            {
                plate.GetCenterLine(true);

            }
        }

        private void button_moveTsParts_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
            Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();

            foreach (ModelObject mo in selectedObjects)
            {
                Beam beam = mo as Beam;
                ContourPlate plate = mo as ContourPlate;
                
                Contour contour = new Contour();
                Chamfer chamfer = new Chamfer(0, 0, Chamfer.ChamferTypeEnum.CHAMFER_NONE);

                if (beam != null)
                {
                    ArrayList cPoints = beam.GetCenterLine(true);
                    ArrayList rPoints = beam.GetReferenceLine(true);
                    double x = beam.StartPoint.X;
                    double y = beam.StartPoint.Y;
                    double z = beam.StartPoint.Z;

                    double dOffset = beam.Position.DepthOffset;
                    if (beam.Position.Depth == Position.DepthEnum.BEHIND) dOffset = -dOffset;

                    Tekla.Structures.Geometry3d.Point sPoint = cPoints[0] as Tekla.Structures.Geometry3d.Point;
                    Tekla.Structures.Geometry3d.Point ePoint = cPoints[cPoints.Count - 1] as Tekla.Structures.Geometry3d.Point;

                    beam.StartPoint = new Tekla.Structures.Geometry3d.Point(sPoint.X, sPoint.Y, beam.EndPoint.Z + dOffset);
                    beam.EndPoint = new Tekla.Structures.Geometry3d.Point(ePoint.X, ePoint.Y, beam.EndPoint.Z + dOffset);

                    beam.Position.Plane = Position.PlaneEnum.MIDDLE;
                    beam.Position.PlaneOffset = 0;
                    //beam.Position.Depth = Position.DepthEnum.FRONT;
                    beam.Position.DepthOffset = 0;

                    try
                    {
                        beam.Modify();
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (plate != null)
                {
                    ArrayList cPoints = plate.GetCenterLine(true);
                    ArrayList points = plate.Contour.ContourPoints;

                    for (int i = 0; i < points.Count; i++)
                    {
                        Tekla.Structures.Geometry3d.Point platePoint = points[i] as Tekla.Structures.Geometry3d.Point;
                        Tekla.Structures.Geometry3d.Point centerPoint = cPoints[i] as Tekla.Structures.Geometry3d.Point;
                        Tekla.Structures.Geometry3d.Point cPoint = new Tekla.Structures.Geometry3d.Point(platePoint.X, platePoint.Y, centerPoint.Z);

                        ContourPoint contourPoint = new ContourPoint(cPoint, chamfer);
                        contour.AddContourPoint(contourPoint);
                    }

                    plate.Contour = contour;
                    plate.Position.Depth = Position.DepthEnum.MIDDLE;
                    plate.Position.DepthOffset = 0;

                    plate.Modify();
                }
            }

            model.CommitChanges();
        }

        private void button_setToLine_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
            Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();

            double x = 0;
            double y = 0;
            double z = 9130;
            Tekla.Structures.Geometry3d.Point point = new Tekla.Structures.Geometry3d.Point(x, y, z);

            foreach (ModelObject mo in selectedObjects)
            {
                Beam beam = mo as Beam;
                ContourPlate plate = mo as ContourPlate;
                //bool isPanel = false;

                //if (beam.Type == Beam.BeamTypeEnum.PANEL) isPanel = true; // beam is a panel
                //_ = isPanel;

                if (beam != null)
                {
                    if (x != 0)
                    {
                        beam.StartPoint = new Tekla.Structures.Geometry3d.Point(x, beam.StartPoint.Y, beam.StartPoint.Z);
                        beam.EndPoint = new Tekla.Structures.Geometry3d.Point(x, beam.EndPoint.Y, beam.EndPoint.Z);

                        try
                        {
                            beam.Modify();
                        }
                        catch (Exception)
                        {

                        }                       
                    }
                    else if (y != 0)
                    {
                        beam.StartPoint = new Tekla.Structures.Geometry3d.Point(beam.StartPoint.X, y, beam.StartPoint.Z);
                        beam.EndPoint = new Tekla.Structures.Geometry3d.Point(beam.EndPoint.X, y, beam.EndPoint.Z);

                        try
                        {
                            beam.Modify();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else if (z != 0)
                    {
                        beam.StartPoint = new Tekla.Structures.Geometry3d.Point(beam.StartPoint.X, beam.StartPoint.Y, z);
                        beam.EndPoint = new Tekla.Structures.Geometry3d.Point(beam.EndPoint.X, beam.EndPoint.Y, z);

                        try
                        {
                            beam.Modify();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                else if (plate != null)
                {
                    if (z != 0)
                    {
                        ArrayList platePoints = plate.Contour.ContourPoints;

                        for (int i = 0; i < platePoints.Count; i++)
                        {
                            Tekla.Structures.Geometry3d.Point platePoint = platePoints[i] as Tekla.Structures.Geometry3d.Point;
                            platePoint.Z = z;
                            platePoints[i] = platePoint;
                        }

                        try
                        {
                            plate.Modify();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }

            model.CommitChanges();
        }

        private void button_setProfiles_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();

            foreach (ModelObject mo in selectedObjects)
            {
                Beam beam = mo as Beam;

                if (beam != null)
                {
                    string profile = beam.Profile.ProfileString;
                    string[] t = profile.Split('*');
                    beam.Profile.ProfileString = "3005*" + t[1];

                    try
                    {
                        beam.Modify();
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
            model.CommitChanges();
        }

        private void button_modifyNodes_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            if (!RFemConnection.ConnectModel(modelName))
            {
                return;
            }

            IModel iModel = RFemConnection.RfModel;

            RFemConnection.LockLicense();

            int tolerance = 5;
            IModelData iModelData = iModel.GetModelData();
            Node[] nodes = iModelData.GetNodes();

            iModelData.PrepareModification();

            for (int i = 0; i < nodes.Length; i++)
            {
                Node node = new Node();
                
                node.No = nodes[i].No;
                node.X = Math.Round(nodes[i].X / tolerance, 3) * tolerance;
                node.Y = Math.Round(nodes[i].Y / tolerance, 3) * tolerance;
                node.Z = Math.Round(nodes[i].Z / tolerance, 3) * tolerance;

                //iModelData.DeleteObjects(ModelObjectType.NodeObject, node.No.ToString());
                nodes[i] = node;

                iModelData.SetNode(node);
            }

            iModelData.FinishModification();

            RFemConnection.UnlockLicense();
        }

        private void button_closeGap_Click(object sender, EventArgs e)
        {
            TEKLA.TeklaMoveTools moveTools = new TEKLA.TeklaMoveTools();
            moveTools.CloseGap();
        }

        private void button_getLineSupportReactions_Click(object sender, EventArgs e)
        {
            string modelName = "";
            if (comboBox_ModelName.SelectedItem != null)
            {
                modelName = comboBox_ModelName.SelectedItem.ToString();
            }

            SupportReactionsToExcel supportReactionsToExcel = new SupportReactionsToExcel();
            supportReactionsToExcel.GetSupportReactionsToExcel_v2(modelName);
        }

        private void button_beamToPanel_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
            Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();

            double x = 0;
            double y = 0;
            double z = 9130;
            Tekla.Structures.Geometry3d.Point point = new Tekla.Structures.Geometry3d.Point(x, y, z);

            foreach (ModelObject mo in selectedObjects)
            {
                Beam beam = mo as Beam;
                ContourPlate plate = mo as ContourPlate;
                bool isPanel = false;

                if (beam.Type == Beam.BeamTypeEnum.PANEL) isPanel = true; // beam is a panel
                _ = isPanel;

                if (beam != null && beam.Type != Beam.BeamTypeEnum.PANEL)
                {
                    Beam panel = new Beam(Beam.BeamTypeEnum.PANEL);

                    panel.AssemblyNumber = beam.AssemblyNumber;
                    panel.CastUnitType = beam.CastUnitType;
                    panel.Class = beam.Class;
                    panel.EndPoint = beam.EndPoint;
                    panel.EndPointOffset = beam.EndPointOffset;
                    panel.Material = beam.Material;
                    panel.Name = beam.Name;
                    panel.PartNumber = beam.PartNumber;
                    panel.Position = beam.Position;
                    panel.Profile = beam.Profile;
                    panel.StartPoint = beam.StartPoint;
                    panel.StartPointOffset = beam.StartPointOffset;
                    panel.GetSolid();
                    panel.Insert();
                    beam.Delete();
                }
            }
            model.CommitChanges();
        }

        private void button_AnalysisModelFix_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Analysis.Analysis analysis = new Tekla.Structures.Analysis.Analysis();
            Tekla.Structures.Analysis.AnalysisModelHandler analysisModelHandler = new Tekla.Structures.Analysis.AnalysisModelHandler();
            Tekla.Structures.Analysis.AnalysisModel analysisModel = analysisModelHandler.GetActiveModel();
            //analysisModel.Select();
            //analysis.GetConnectionStatus();

            Tekla.Structures.Analysis.UI.AnalysisObjectSelector selector = new Tekla.Structures.Analysis.UI.AnalysisObjectSelector();
            Tekla.Structures.Analysis.AnalysisObjectEnumerator analysisObjects = selector.GetSelectedObjects(analysisModel.AnalysisModelName);

            List<Tekla.Structures.Analysis.AnalysisNode> nodes = new List<Tekla.Structures.Analysis.AnalysisNode>();
            List<Tekla.Structures.Analysis.AnalysisArea> areas = new List<Tekla.Structures.Analysis.AnalysisArea>();
            List<Tekla.Structures.Analysis.AnalysisBar> bars = new List<Tekla.Structures.Analysis.AnalysisBar>();
            List<Tekla.Structures.Analysis.AnalysisPart> parts = new List<Tekla.Structures.Analysis.AnalysisPart>();
            List<Tekla.Structures.Analysis.AnalysisNodeLink> links = new List<Tekla.Structures.Analysis.AnalysisNodeLink>();
            List<Tekla.Structures.Analysis.AnalysisObject> aObjects = new List<Tekla.Structures.Analysis.AnalysisObject>();

            while (analysisObjects.MoveNext())
            {
                Tekla.Structures.Analysis.AnalysisObject analysisObject = analysisObjects.Current;
                Tekla.Structures.Analysis.AnalysisNode node = analysisObject as Tekla.Structures.Analysis.AnalysisNode;
                Tekla.Structures.Analysis.AnalysisArea area = analysisObject as Tekla.Structures.Analysis.AnalysisArea;
                Tekla.Structures.Analysis.AnalysisBar bar = analysisObject as Tekla.Structures.Analysis.AnalysisBar;
                Tekla.Structures.Analysis.AnalysisPart part = analysisObject as Tekla.Structures.Analysis.AnalysisPart;
                Tekla.Structures.Analysis.AnalysisNodeLink link = analysisObject as Tekla.Structures.Analysis.AnalysisNodeLink;

                if (node != null)
                {
                    nodes.Add(node);
                }
                else if (area != null)
                {
                    areas.Add(area);
                }
                else if (bar != null)
                {
                    bars.Add(bar);
                }
                else if (part != null)
                {
                    parts.Add(part);

                    if (part.AnalysisAreas.Count > 0)
                    {
                        foreach (var a in part.AnalysisAreas)
                        {
                            areas.Add(a);
                        }
                    }
                    if (part.AnalysisBars.Count > 0)
                    {
                        foreach (var b in part.AnalysisBars)
                        {
                            bars.Add(b);
                        }
                    }
                }
                else if (link != null)
                {
                    links.Add(link);
                }
                else
                {
                    aObjects.Add(analysisObject);
                }
            }

            for (int i = 0; i < links.Count; i++)
            {
                links[i].Delete();
                i--;
            }

            analysisModel.Modify();
        }

        private void button_getSolid_Click(object sender, EventArgs e)
        {
            Tekla.Structures.Model.UI.ModelObjectSelector objectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            ModelObjectEnumerator selectedObjects = objectSelector.GetSelectedObjects();
            List<Beam> beams = new List<Beam>();
            List<ContourPlate> plates = new List<ContourPlate>();

            HashSet<Tekla.Structures.Geometry3d.Point> pointSet = new HashSet<Tekla.Structures.Geometry3d.Point>();

            foreach (ModelObject mo in selectedObjects)
            {
                Beam beam = mo as Beam;
                ContourPlate plate = mo as ContourPlate;
                BooleanPart cut = mo as BooleanPart;

                if (cut != null)
                {
                    beam = cut.OperativePart as Beam;
                    plate = cut.OperativePart as ContourPlate;
                }
                if (beam != null)
                {
                    beams.Add(beam);

                    if (beam != null && beam.Type == Beam.BeamTypeEnum.PANEL)
                    {
                        Tekla.Structures.Geometry3d.Vector dir = new Tekla.Structures.Geometry3d.Vector((beam.EndPoint.X - beam.StartPoint.X), (beam.EndPoint.Y - beam.StartPoint.Y), (beam.EndPoint.Z - beam.StartPoint.Z));
                        Tekla.Structures.Geometry3d.Vector up = new Tekla.Structures.Geometry3d.Vector(0, 0, 1);
                        Tekla.Structures.Model.Solid solid = beam.GetSolid();

                        var faceEnumerator = solid.GetFaceEnumerator();

                        while (faceEnumerator.MoveNext())
                        {
                            Tekla.Structures.Solid.Face face = faceEnumerator.Current;
                            Tekla.Structures.Geometry3d.Vector normal = face.Normal;
                            if (Utility.Utility.GetVectorAngle(dir, normal) < 0.1 && Utility.Utility.GetVectorAngle(up, normal) > 85)
                            {
                                var loopEnumerator = face.GetLoopEnumerator();
                                while (loopEnumerator.MoveNext())
                                {
                                    Tekla.Structures.Solid.Loop loop = loopEnumerator.Current;
                                    var vertexEnumerator = loop.GetVertexEnumerator();
                                    while (vertexEnumerator.MoveNext())
                                    {
                                        Tekla.Structures.Geometry3d.Point point = vertexEnumerator.Current;
                                        pointSet.Add(point);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                else if (plate != null)
                {
                    plates.Add(plate);

                    Tekla.Structures.Geometry3d.Vector up = new Tekla.Structures.Geometry3d.Vector(0, 0, 1);
                    Tekla.Structures.Model.Solid solid = plate.GetSolid();

                    var faceEnumerator = solid.GetFaceEnumerator();
                    
                    while (faceEnumerator.MoveNext())
                    {
                        Tekla.Structures.Solid.Face face = faceEnumerator.Current;
                        Tekla.Structures.Geometry3d.Vector normal = face.Normal;
                        if (Utility.Utility.GetVectorAngle(up, normal) < 1)
                        {
                            var loopEnumerator = face.GetLoopEnumerator();
                            while (loopEnumerator.MoveNext())
                            {
                                Tekla.Structures.Solid.Loop loop = loopEnumerator.Current;
                                var vertexEnumerator = loop.GetVertexEnumerator();
                                while (vertexEnumerator.MoveNext())
                                {
                                    Tekla.Structures.Geometry3d.Point point = vertexEnumerator.Current;
                                    pointSet.Add(point);
                                }
                            }
                        }
                        break;
                    }
                }
            }

            Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
            model.CommitChanges();
        }
    }

    public class LineForces
    {
        Line line;
        List<double> delta;
        Vector3 coordinate;
        double length;

        public List<LineSupportForces> supportReactions;

        public double Length { get => length; private set => length = value; }
        public Vector3 Coordinate { get => coordinate; private set => coordinate = value; }
        public float LineForceZ
        {
            get
            {
                return (float)(GetResultantForceZ() / length);
            }
        }

        public LineForces(Line _line)
        {
            line = _line;

            Coordinate = SetCoordinate(line);
            Length = line.Length;
            supportReactions = new List<LineSupportForces>();
            delta = new List<double>();
        }

        Vector3 SetCoordinate(Line line)
        {
            int pointCount = line.ControlPoints.Length - 1;
            Vector3 startPoint = new Vector3((float)line.ControlPoints[0].X * 1000, (float)line.ControlPoints[0].Y * 1000, (float)line.ControlPoints[0].Z * 1000);
            Vector3 endPoint = new Vector3((float)line.ControlPoints[pointCount].X * 1000, (float)line.ControlPoints[pointCount].Y * 1000, (float)line.ControlPoints[pointCount].Z * 1000);
            Vector3 v;
            if (startPoint.X > endPoint.X || startPoint.Y > endPoint.Y)
            {
                v = startPoint;
                startPoint = endPoint;
                endPoint = v;
            }
            return new Vector3(startPoint.X + (endPoint.X - startPoint.X) / 2, startPoint.Y + (endPoint.Y - startPoint.Y) / 2, startPoint.Z + (endPoint.Z - startPoint.Z) / 2);
        }

        public float GetResultantForceZ()
        {
            float resultant = 0;
            int n = 0;
            List<LineSupportForces> sr = new List<LineSupportForces>();

            for (int i = 0; i < supportReactions.Count; i++)
            {
                if (supportReactions[i].Type == ResultsValueType.MinimumValueType)
                {
                    delta.Add(supportReactions[i].Location);
                    sr.Add(supportReactions[i]);
                }
            }

            for (int i = 1; i < sr.Count; i++)
            {
                double d;
                double f;

                d = (delta[i] - delta[i - 1]);
                f = (sr[i].Forces.Z / 1000 + sr[i - 1].Forces.Z / 1000) / 2;

                resultant += (float)(f * d);
            }

            //for (int i = 0; i < supportReactions.Count; i++)
            //{
            //    double d;

            //    if (supportReactions[i].Type == ResultsValueType.MinimumValueType)
            //    {
            //        if (n == 0)
            //        {
            //            d = delta[1] / 2;
            //        }
            //        else if (n == delta.Count - 1)
            //        {
            //            d = delta[n] / 2;
            //        }
            //        else
            //        {
            //            d = (delta[n + 1] - delta[n]) / 2 + (delta[n] - delta[n - 1]) / 2;
            //        }

            //        resultant += (float)(supportReactions[i].Forces.Z / 1000 * d);
            //        n++;
            //    }
            //}
            return (float)Math.Round(resultant, 3);
        }
    }
}
