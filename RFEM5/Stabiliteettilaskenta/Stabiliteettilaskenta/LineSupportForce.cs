using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Stabiliteettilaskenta
{
    public class SupportReactionsToExcel
    {
        public void GetSupportReactionsToExcel_v2(string modelName)
        {

            IModel iModel = ConnectExcelAndRfem(modelName, out Excel.Worksheet worksheet);

            int[] loadCombinations = { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 };//15-33, 48-66
            int[] lineSupports = { 16683, 7823, 16687, 7975, 16680, 7807, 7819, 16685, 7827, 7839 };

            SupportForceData supportForceData = new SupportForceData(new List<LoadCaseSupportForceData>());

            for (int lc = 0; lc < loadCombinations.Length; lc++)
            {
                int loadingNo = loadCombinations[lc];
                RFem.LoadingCase loadingCase = new RFem.LoadingCase(loadingNo, LoadingType.LoadCombinationType);
                IResults iResults = RFem.Utility.GetResults(iModel, loadingCase);

                supportForceData.loadCaseSupportForceData.Add(new LoadCaseSupportForceData(loadingNo, new List<WallSupportForceData>()));
                int wallNumber = 0;
                float locationIncrement = 0f;

                for (int ln = 0; ln < lineSupports.Length; ln++)
                {
                    int locationIndex = 0;
                    int lineNumber = lineSupports[ln];
                    LineSupportForces[] forces = iResults.GetLineSupportForces(lineSupports[ln], ItemAt.AtNo, false);

                    for (int fn = 0; fn < forces.Length; fn++)
                    {
                        if (lineNumber == 16683 || lineNumber == 7823)
                        {
                            wallNumber = 0;
                            locationIncrement = 3.09f * 2 / 18;

                            if (locationIndex == 0)
                            {
                                if (lineNumber == 7823)
                                {
                                    locationIndex++;
                                    continue;
                                }
                                else
                                {
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces.Add(wallSuportForceData);
                                }
                            }

                            Vector3 tempForces = new Vector3((float)Math.Round(forces[fn].Forces.X / 1000), (float)Math.Round(forces[fn].Forces.Y / 1000), (float)Math.Round(forces[fn].Forces.Z / 1000));

                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wallNumber].lineSupportForces.Add(
                            new LineSupportForce(locationIncrement * fn, locationIncrement, tempForces));

                            locationIndex++;

                            if (locationIndex == 10)
                            {
                                locationIndex = 0;
                            }
                        }
                        else if (lineNumber == 16687 || lineNumber == 7975)
                        {
                            wallNumber = 1;
                            locationIncrement = 3.09f * 2 / 18;

                            if (locationIndex == 0)
                            {
                                if (lineNumber == 7975)
                                {
                                    locationIndex++;
                                    continue;
                                }
                                else
                                {
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces.Add(wallSuportForceData);
                                }
                            }

                            Vector3 tempForces = new Vector3((float)Math.Round(forces[fn].Forces.X / 1000), (float)Math.Round(forces[fn].Forces.Y / 1000), (float)Math.Round(forces[fn].Forces.Z / 1000));

                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wallNumber].lineSupportForces.Add(
                            new LineSupportForce(locationIncrement * fn, locationIncrement, tempForces));

                            locationIndex++;

                            if (locationIndex == 10)
                            {
                                locationIndex = 0;
                            }
                        }
                        else if (lineNumber == 16680 || lineNumber == 7807 || lineNumber == 7819)
                        {
                            wallNumber = 2;
                            locationIncrement = 7.09f / 20;

                            if (locationIndex == 0)
                            {
                                if (lineNumber == 7807 || lineNumber == 7819)
                                {
                                    locationIndex++;
                                    continue;
                                }
                                else
                                {
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces.Add(wallSuportForceData);
                                }
                            }

                            Vector3 tempForces = new Vector3((float)Math.Round(forces[fn].Forces.X / 1000), (float)Math.Round(forces[fn].Forces.Y / 1000), (float)Math.Round(forces[fn].Forces.Z / 1000));

                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wallNumber].lineSupportForces.Add(
                            new LineSupportForce(locationIncrement * fn, locationIncrement, tempForces));

                            locationIndex++;

                            if (lineNumber == 16680 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineNumber == 7807 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineNumber == 7819 && locationIndex == 7)
                            {
                                locationIndex = 0;
                            }
                        }
                        else if (lineNumber == 16685 || lineNumber == 7827 || lineNumber == 7839)
                        {
                            wallNumber = 3;
                            locationIncrement = 7.09f / 20;

                            if (locationIndex == 0)
                            {
                                if (lineNumber == 7827 || lineNumber == 7839)
                                {
                                    locationIndex++;
                                    continue;
                                }
                                else
                                {
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces.Add(wallSuportForceData);
                                }
                            }

                            Vector3 tempForces = new Vector3((float)Math.Round(forces[fn].Forces.X / 1000), (float)Math.Round(forces[fn].Forces.Y / 1000), (float)Math.Round(forces[fn].Forces.Z / 1000));

                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wallNumber].lineSupportForces.Add(
                            new LineSupportForce(locationIncrement * fn, locationIncrement, tempForces));

                            locationIndex++;

                            if (lineNumber == 16685 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineNumber == 7827 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineNumber == 7839 && locationIndex == 7)
                            {
                                locationIndex = 0;
                            }
                        }
                    }
                }

                for (int wn = 0; wn < supportForceData.loadCaseSupportForceData[lc].wallSupportForces.Count; wn++)
                {
                    LineSupportIntegral integral;
                    int integralIndex = 0;

                    for (int wsf = 0; wsf < supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces.Count; wsf++)
                    {
                        if (wsf < supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces.Count - 1)
                        {
                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].NextForces =
                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf + 1].Forces;
                        }

                        supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].location =
                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].locationIncrement * wsf;
                    }

                    for (int wsf = 0; wsf < supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces.Count; wsf++)
                    { 
                        if (wsf == 0)
                        {
                            float lIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].locationIncrement;
                            Vector3 fIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].ForceIncrement;

                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals = new List<LineSupportIntegral>();
                            integral = new LineSupportIntegral(lIncrement, fIncrement);
                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals.Add(integral);
                        }
                        else if (wsf % 3 == 0 && wn < 2)
                        {
                            if (wsf < supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces.Count - 1)
                            {
                                float lIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].locationIncrement;
                                Vector3 fIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].ForceIncrement;

                                integral = new LineSupportIntegral(lIncrement, fIncrement);
                                supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals.Add(integral);
                            }
                        }
                        else if (wn >= 2)
                        {
                            if (wsf == 3 || wsf == 7 || wsf == 11 || wsf == 15 || wsf == 19 || wsf == 23)
                            {
                                float k = wsf - integralIndex;
                                float a = 7.09f / 7;
                                float b = a * (k / 3);
                                float c = locationIncrement * k;

                                float x = c - b;
                                float x0 = c - locationIncrement;
                                float x1 = c;
                                integralIndex++;

                                Vector3 y0 = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf - 1].Forces;
                                Vector3 y1 = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].Forces;
                                Vector3 lerpForce = new Vector3();

                                lerpForce.X = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, y0.X, y1.X);
                                lerpForce.Y = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, y0.Y, y1.Y);
                                lerpForce.Z = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, y0.Z, y1.Z);

                                supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf - 1].locationIncrement -= x;
                                supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf - 1].NextForces = lerpForce;

                                Vector3 temp = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].Forces;

                                supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces.Insert(wsf, new LineSupportForce(locationIncrement * wsf - x, x, lerpForce, temp));

                                if (wsf < supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces.Count - 1)
                                {
                                    int count = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals.Count;
                                    integral = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals[count - 1];

                                    float lIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].locationIncrement;
                                    Vector3 fIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].ForceIncrement;

                                    integral.length -= lIncrement;
                                    integral.value -= fIncrement;

                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals[count - 1] = integral;

                                    integral = new LineSupportIntegral(lIncrement, fIncrement);
                                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals.Add(integral);
                                }
                            }
                            else if (wsf < supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces.Count - 1)
                            {
                                float lIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].locationIncrement;
                                Vector3 fIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].ForceIncrement;

                                int count = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals.Count;
                                integral = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals[count - 1];
                                integral.length += lIncrement;
                                integral.value += fIncrement;

                                supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals[count - 1] = integral;
                            }
                        }
                        else
                        {
                            float lIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].locationIncrement;
                            Vector3 fIncrement = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportForces[wsf].ForceIncrement;

                            int count = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals.Count;
                            integral = supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals[count - 1];
                            integral.length += lIncrement;
                            integral.value += fIncrement;

                            supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].lineSupportIntegrals[count - 1] = integral;
                        }
                    }

                    supportForceData.loadCaseSupportForceData[lc].wallSupportForces[wn].MaxMin();
                }
            }

            supportForceData.MinMaxForces();
            supportForceData.MinMaxIntegrals();

            // Print results to Excel
            int row = 8;
            int column = 1;

            worksheet.Cells[row, "A"] = "Wall";
            //worksheet.Cells[row, "B"] = "Location";
            worksheet.Name = "ExtremeValues";

            row += 2;

            for (int wallNum = 0; wallNum < supportForceData.loadCaseSupportForceData[0].wallSupportForces.Count; wallNum++)
            {
                worksheet.Cells[row, "A"] = supportForceData.loadCaseSupportForceData[0].wallSupportForces[wallNum].wallNumber;
                
                worksheet.Cells[row + 1, "B"] = "Max Fx [kN]";
                worksheet.Cells[row + 2, "B"] = "Max Fy [kN]";
                worksheet.Cells[row + 3, "B"] = "Max Fz [kN]";
                worksheet.Cells[row + 4, "B"] = "Min Fx [kN]";
                worksheet.Cells[row + 5, "B"] = "Min Fy [kN]";
                worksheet.Cells[row + 6, "B"] = "Min Fz [kN]";

                column = 3;
                float loc = 0;

                for (int integrNum = 0; integrNum < supportForceData.loadCaseSupportForceData[0].wallSupportForces[wallNum].lineSupportIntegrals.Count; integrNum++)
                {
                    loc += supportForceData.loadCaseSupportForceData[0].wallSupportForces[wallNum].lineSupportIntegrals[integrNum].length;
                    worksheet.Cells[row, column] = Math.Round(loc, 2);

                    
                    worksheet.Cells[row + 1, column] = Math.Round(supportForceData.maxForceIntegral[wallNum][integrNum].X, 2);
                    worksheet.Cells[row + 2, column] = Math.Round(supportForceData.maxForceIntegral[wallNum][integrNum].Y, 2);
                    worksheet.Cells[row + 3, column] = Math.Round(supportForceData.maxForceIntegral[wallNum][integrNum].Z, 2);
                    worksheet.Cells[row + 4, column] = Math.Round(supportForceData.minForceIntegral[wallNum][integrNum].X, 2);
                    worksheet.Cells[row + 5, column] = Math.Round(supportForceData.minForceIntegral[wallNum][integrNum].Y, 2);
                    worksheet.Cells[row + 6, column] = Math.Round(supportForceData.minForceIntegral[wallNum][integrNum].Z, 2);

                    column++;
                }
                worksheet.Cells[row, column] = "Location [m]";
                row += 8;
            }

            // Print results to Excel

            Excel.Workbook workbook = worksheet.Application.ActiveWorkbook;
            worksheet = workbook.Worksheets.Add();

            row = 8;
            column = 1;

            worksheet.Cells[row, "A"] = "Wall";
            worksheet.Cells[row, "B"] = "LoadCombination";
            //worksheet.Cells[row, "C"] = "Location";
            worksheet.Name = "AllValues";

            row += 2;

            for (int wallNum = 0; wallNum < supportForceData.loadCaseSupportForceData[0].wallSupportForces.Count; wallNum++)
            {
                worksheet.Cells[row, "A"] = supportForceData.loadCaseSupportForceData[0].wallSupportForces[wallNum].wallNumber;

                for (int loadNum = 0; loadNum < supportForceData.loadCaseSupportForceData.Count; loadNum++)
                {
                    worksheet.Cells[row, "B"] = supportForceData.loadCaseSupportForceData[loadNum].loadingNumber;
                    worksheet.Cells[row + 1, "C"] = "Fx [kN]";
                    worksheet.Cells[row + 2, "C"] = "Fy [kN]";
                    worksheet.Cells[row + 3, "C"] = "Fz [kN]";

                    column = 4;
                    float loc = 0;

                    for (int integrNum = 0; integrNum < supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportIntegrals.Count; integrNum++)
                    {
                        loc += supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportIntegrals[integrNum].length;
                        worksheet.Cells[row, column] = Math.Round(loc, 2);

                        worksheet.Cells[row + 1, column] = Math.Round(supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportIntegrals[integrNum].value.X, 2);
                        worksheet.Cells[row + 2, column] = Math.Round(supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportIntegrals[integrNum].value.Y, 2);
                        worksheet.Cells[row + 3, column] = Math.Round(supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportIntegrals[integrNum].value.Z, 2);

                        column++;
                    }
                    worksheet.Cells[row, column] = "Location [m]";
                    row += 5;
                }
                row++;
            }


            // Print results to Excel

            worksheet = workbook.Worksheets.Add();

            row = 8;
            column = 1;

            worksheet.Cells[row, "A"] = "Wall";
            worksheet.Cells[row, "B"] = "LoadCombination";
            //worksheet.Cells[row, "C"] = "Location";
            worksheet.Name = "LocalExtremeValues";

            row += 2;

            for (int wallNum = 0; wallNum < supportForceData.loadCaseSupportForceData[0].wallSupportForces.Count; wallNum++)
            {
                worksheet.Cells[row, "A"] = supportForceData.loadCaseSupportForceData[0].wallSupportForces[wallNum].wallNumber;

                for (int loadNum = 0; loadNum < supportForceData.loadCaseSupportForceData.Count; loadNum++)
                {
                    worksheet.Cells[row, "B"] = supportForceData.loadCaseSupportForceData[loadNum].loadingNumber;
                    worksheet.Cells[row + 1, "C"] = "Fx [kN]";
                    worksheet.Cells[row + 2, "C"] = "Fy [kN]";
                    worksheet.Cells[row + 3, "C"] = "Fz [kN]";

                    column = 4;
                    float loc = 0;

                    for (int forceNum = 0; forceNum < supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportForces.Count; forceNum++)
                    {
                        if ((wallNum < 2 && forceNum % 3 == 0) || (wallNum >= 2 && (forceNum == 0 || forceNum == 3 || forceNum == 7 || forceNum == 11 || forceNum == 15 || forceNum == 19 || forceNum == 23 || forceNum == supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportForces.Count - 1)))
                        {
                            loc = supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportForces[forceNum].location;
                            worksheet.Cells[row, column] = Math.Round(loc, 2);

                            worksheet.Cells[row + 1, column] = Math.Round(supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportForces[forceNum].Forces.X, 2);
                            worksheet.Cells[row + 2, column] = Math.Round(supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportForces[forceNum].Forces.Y, 2);
                            worksheet.Cells[row + 3, column] = Math.Round(supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportForces[forceNum].Forces.Z, 2);

                            column++;
                        }
                    }

                    worksheet.Cells[row, column] = "Location [m]";
                    row += 5;
                }
                row++;                
            }

            // Print Extremes

            worksheet = workbook.Worksheets.Add();

            worksheet.Cells[row, "A"] = "Wall";
            worksheet.Name = "MaxMinValues";

            int r = 8;
            int col = 4;

            for (int wallNum = 0; wallNum < supportForceData.loadCaseSupportForceData[0].wallSupportForces.Count; wallNum++)
            { 
                int count = supportForceData.loadCaseSupportForceData[0].wallSupportForces[wallNum].lineSupportForces.Count;
                worksheet.Cells[r, "A"] = supportForceData.loadCaseSupportForceData[0].wallSupportForces[wallNum].wallNumber;
                col = 4;

                worksheet.Cells[r, col - 1] = "Fx Max [kN]";
                worksheet.Cells[r + 1, col - 1] = "Fy Max [kN]";
                worksheet.Cells[r + 2, col - 1] = "Fz Max [kN]";
                worksheet.Cells[r + 3, col - 1] = "Fx Min [kN]";
                worksheet.Cells[r + 4, col - 1] = "Fy Min [kN]";
                worksheet.Cells[r + 5, col - 1] = "Fz Min [kN]";

                for (int locNum = 0; locNum < count; locNum++)
                {
                    if ((wallNum < 2 && locNum % 3 == 0) || (wallNum >= 2 && (locNum == 0 || locNum == 3 || locNum == 7 || locNum == 11 || locNum == 15 || locNum == 19 || locNum == 23 || locNum == count - 1)))
                    {
                        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                        for (int loadNum = 0; loadNum < supportForceData.loadCaseSupportForceData.Count; loadNum++)
                        {
                            Vector3 f = supportForceData.loadCaseSupportForceData[loadNum].wallSupportForces[wallNum].lineSupportForces[locNum].Forces;

                            if (f.X > max.X)
                            {
                                max.X = f.X;
                            }
                            if (f.Y > max.Y)
                            {
                                max.Y = f.Y;
                            }
                            if (f.Z > max.Z)
                            {
                                max.Z = f.Z;
                            }
                            if (f.X < min.X)
                            {
                                min.X = f.X;
                            }
                            if (f.Y < min.Y)
                            {
                                min.Y = f.Y;
                            }
                            if (f.Z < min.Z)
                            {
                                min.Z = f.Z;
                            }
                        }

                        worksheet.Cells[r, col] = Math.Round(max.X, 2);
                        worksheet.Cells[r + 1, col] = Math.Round(max.Y, 2);
                        worksheet.Cells[r + 2, col] = Math.Round(max.Z, 2);
                        worksheet.Cells[r + 3, col] = Math.Round(min.X, 2);
                        worksheet.Cells[r + 4, col] = Math.Round(min.Y, 2);
                        worksheet.Cells[r + 5, col] = Math.Round(min.Z, 2);

                        col++;
                    }
                }
                r += 8;
            }

            Marshal.ReleaseComObject(worksheet);
        }

        public void GetSupportReactionsToExcel(string modelName)
        {
            IModel iModel = ConnectExcelAndRfem(modelName, out Excel.Worksheet worksheet);
            //RFemConnection.LockLicense();
            //IModelData modelData = iModel.GetModelData();

            int row = 8;

            worksheet.Cells[row, "A"] = "CO";
            worksheet.Cells[row, "B"] = "Line";
            worksheet.Cells[row, "C"] = "Location";

            worksheet.Cells[row, "E"] = "Fx [kN]";
            worksheet.Cells[row, "F"] = "Fy [kN]";
            worksheet.Cells[row, "G"] = "Fz [kN]";

            //worksheet.Cells[row, "I"] = "Mx [kNm]";
            //worksheet.Cells[row, "J"] = "My [kNm]";
            //worksheet.Cells[row, "K"] = "Mz [kNm]";

            row = row + 2;

            int[] loadCombinations = { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 };//15-33, 48-66
            int[] lineSupports = { 16683, 7823, 16687, 7975, 16680, 7807, 7819, 16685, 7827, 7839 }; // Y: 16683, 7823; 16687, 7975 ; X: 16680, 7807, 7819; 16685, 7827, 7839

            List<LoadCaseSupportForceData> loadCaseSupportForceData = new List<LoadCaseSupportForceData>();


            for (int i = 0; i < loadCombinations.Length; i++)
            {
                int loadingNo = loadCombinations[i];
                RFem.LoadingCase loadingCase = new RFem.LoadingCase(loadingNo, LoadingType.LoadCombinationType);
                IResults iResults = RFem.Utility.GetResults(iModel, loadingCase);

                loadCaseSupportForceData.Add(new LoadCaseSupportForceData(loadingNo, new List<WallSupportForceData>()));

                // List<Vector3> wallForceList = new List<Vector3>();
                List<Vector3>[] wallForces = { new List<Vector3>(), new List<Vector3>(), new List<Vector3>(), new List<Vector3>() };
                int locationIndex = 0;
                int wallNumber = 0;

                for (int j = 0; j < lineSupports.Length; j++)
                {
                    LineSupportForces[] forces = iResults.GetLineSupportForces(lineSupports[j], ItemAt.AtNo, false);

                    for (int k = 0; k < forces.Length; k++)
                    {
                        // Line line = modelData.GetLine(forces[k].LineNo, ItemAt.AtNo).GetData();
                        // double length = line.Length;

                        if (lineSupports[j] == 16683 || lineSupports[j] == 7823)
                        {
                            // Wall 1
                            if (lineSupports[j] == 7823 && locationIndex == 0)
                            {
                                locationIndex++;
                            }
                            else
                            {
                                wallForces[0].Add(new Vector3((float)Math.Round(forces[k].Forces.X / 1000), (float)Math.Round(forces[k].Forces.Y / 1000), (float)Math.Round(forces[k].Forces.Z / 1000)));

                                if (locationIndex == 0)
                                {
                                    wallNumber = 0;
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    loadCaseSupportForceData[i].wallSupportForces.Add(wallSuportForceData);
                                }

                                locationIndex++;
                            }

                            if (lineSupports[j] == 16683 && locationIndex == 10)
                            {
                                locationIndex = 0;
                            }
                            else if (lineSupports[j] == 7823 && locationIndex == 10)
                            {
                                locationIndex = 0;
                            }
                        }
                        else if (lineSupports[j] == 16687 || lineSupports[j] == 7975)
                        {
                            // Wall 2
                            if (lineSupports[j] == 7975 && locationIndex == 0)
                            {
                                locationIndex++;
                            }
                            else
                            {
                                wallForces[1].Add(new Vector3((float)Math.Round(forces[k].Forces.X / 1000), (float)Math.Round(forces[k].Forces.Y / 1000), (float)Math.Round(forces[k].Forces.Z / 1000)));

                                if (locationIndex == 0)
                                {
                                    wallNumber = 1;
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    loadCaseSupportForceData[i].wallSupportForces.Add(wallSuportForceData);
                                }

                                locationIndex++;
                            }

                            if (lineSupports[j] == 16687 && locationIndex == 10)
                            {
                                locationIndex = 0;
                            }
                            else if (lineSupports[j] == 7975 && locationIndex == 10)
                            {
                                locationIndex = 0;
                            }
                        }
                        else if (lineSupports[j] == 16680 || lineSupports[j] == 7807 || lineSupports[j] == 7819)
                        {
                            // Wall 3
                            if (lineSupports[j] == 7807 && locationIndex == 0)
                            {
                                locationIndex++;
                            }
                            else if (lineSupports[j] == 7819 && locationIndex == 0)
                            {
                                locationIndex++;
                            }
                            else
                            {
                                wallForces[2].Add(new Vector3((float)Math.Round(forces[k].Forces.X / 1000), (float)Math.Round(forces[k].Forces.Y / 1000), (float)Math.Round(forces[k].Forces.Z / 1000)));

                                if (locationIndex == 0)
                                {
                                    wallNumber = 2;
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    loadCaseSupportForceData[i].wallSupportForces.Add(wallSuportForceData);
                                }

                                locationIndex++;
                            }

                            if (lineSupports[j] == 16680 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineSupports[j] == 7807 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineSupports[j] == 7819 && locationIndex == 7)
                            {
                                locationIndex = 0;
                            }
                        }
                        else if (lineSupports[j] == 16685 || lineSupports[j] == 7827 || lineSupports[j] == 7839)
                        {
                            // Wall 4
                            if (lineSupports[j] == 7827 && locationIndex == 0)
                            {
                                locationIndex++;
                            }
                            else if (lineSupports[j] == 7839 && locationIndex == 0)
                            {
                                locationIndex++;
                            }
                            else
                            {
                                wallForces[3].Add(new Vector3((float)Math.Round(forces[k].Forces.X / 1000), (float)Math.Round(forces[k].Forces.Y / 1000), (float)Math.Round(forces[k].Forces.Z / 1000)));

                                if (locationIndex == 0)
                                {
                                    wallNumber = 3;
                                    WallSupportForceData wallSuportForceData = new WallSupportForceData(wallNumber, new List<LineSupportForce>());
                                    loadCaseSupportForceData[i].wallSupportForces.Add(wallSuportForceData);
                                }

                                locationIndex++;
                            }

                            if (lineSupports[j] == 16685 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineSupports[j] == 7827 && locationIndex == 8)
                            {
                                locationIndex = 0;
                            }
                            else if (lineSupports[j] == 7839 && locationIndex == 7)
                            {
                                locationIndex = 0;
                            }
                        }

                        //worksheet.Cells[row, "A"] = loadingNo;
                        //worksheet.Cells[row, "B"] = lineSupports[j];
                        //worksheet.Cells[row, "C"] = forces[k].Location;

                        //worksheet.Cells[row, "E"] = Math.Round(forces[k].Forces.X / 1000);
                        //worksheet.Cells[row, "F"] = Math.Round(forces[k].Forces.Y / 1000);
                        //worksheet.Cells[row, "G"] = Math.Round(forces[k].Forces.Z / 1000);

                        //worksheet.Cells[row, "I"] = Math.Round(forces[k].Moments.X / 1000);
                        //worksheet.Cells[row, "J"] = Math.Round(forces[k].Moments.Y / 1000);
                        //worksheet.Cells[row, "K"] = Math.Round(forces[k].Moments.Z / 1000);

                        //row++;
                    }
                    //row++;
                }

                float locationIncrement;

                for (int s = 0; s < wallForces.Length; s++)
                {
                    List<Vector3> wallForceList = wallForces[s];
                    List<LineSupportForce> lineForces = new List<LineSupportForce>();

                    worksheet.Cells[row, "A"] = loadingNo;
                    worksheet.Cells[row, "B"] = "Wall :" + s;

                    List<Vector3> segmentForces = new List<Vector3>();
                    Vector3 forceIntegral = Vector3.Zero;

                    if (s < 2)
                    {
                        locationIncrement = 3.09f * 2 / 18;
                    }
                    else
                    {
                        locationIncrement = 7.09f / 20;
                    }

                    for (int f = 0; f < wallForces[s].Count - 1; f++)
                    {
                        Vector3 segment = new Vector3();
                        segment.X = (wallForceList[f].X + wallForceList[f + 1].X) / 2 * locationIncrement;
                        segment.Y = (wallForceList[f].Y + wallForceList[f + 1].Y) / 2 * locationIncrement;
                        segment.Z = (wallForceList[f].Z + wallForceList[f + 1].Z) / 2 * locationIncrement;

                        segmentForces.Add(segment);

                        lineForces.Add(new LineSupportForce(locationIncrement * f, locationIncrement, wallForceList[f], wallForceList[f + 1]));

                        loadCaseSupportForceData[i].wallSupportForces[s].lineSupportForces.Add(
                                new LineSupportForce(locationIncrement * f, locationIncrement, wallForceList[f], wallForceList[f + 1]));
                    }

                    for (int f = 0; f < wallForces[s].Count; f++)
                    {
                        worksheet.Cells[row, "A"] = loadingNo;
                        worksheet.Cells[row, "B"] = locationIncrement * f;

                        worksheet.Cells[row, "E"] = wallForceList[f].X;
                        worksheet.Cells[row, "F"] = wallForceList[f].Y;
                        worksheet.Cells[row, "G"] = wallForceList[f].Z;

                        if (f < segmentForces.Count)
                        {
                            worksheet.Cells[row, "I"] = segmentForces[f].Z;
                        }

                        if (s < 2)
                        {
                            if (f % 3 == 0 && f != 0)
                            {
                                worksheet.Cells[row, "K"] = forceIntegral.Z;

                                if (f < segmentForces.Count)
                                    forceIntegral = segmentForces[f];
                            }
                            else
                            {
                                forceIntegral.X += segmentForces[f].X;
                                forceIntegral.Y += segmentForces[f].Y;
                                forceIntegral.Z += segmentForces[f].Z;
                            }
                        }
                        else
                        {
                            if (f == segmentForces.Count)
                            {
                                worksheet.Cells[row, "K"] = forceIntegral.Z;
                            }
                            else if (((f % 3 == 0) && f != 0))
                            {
                                float a = 7.09f / 7;
                                float b = a * (f / 3);
                                float c = locationIncrement * f;

                                float x = c - b;
                                float x0 = c - locationIncrement;
                                float x1 = c;

                                Vector3 temp = new Vector3();
                                Vector3 lerpForce = new Vector3();

                                if (f < segmentForces.Count)
                                {
                                    temp.X = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, segmentForces[f - 1].X, segmentForces[f].X) * x;
                                    temp.Y = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, segmentForces[f - 1].Y, segmentForces[f].Y) * x;
                                    temp.Z = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, segmentForces[f - 1].Z, segmentForces[f].Z) * x;

                                    lerpForce.X = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, wallForceList[f - 1].X, wallForceList[f].X);
                                    lerpForce.Y = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, wallForceList[f - 1].Y, wallForceList[f].Y);
                                    lerpForce.Z = Utility.Utility.LERP(x0 + locationIncrement - x, x0, x1, wallForceList[f - 1].Z, wallForceList[f].Z);
                                }
                                else
                                {
                                    temp.X = Utility.Utility.LERP(x, x0, x1, segmentForces[segmentForces.Count - 2].X, segmentForces[segmentForces.Count - 1].X) * x;
                                    temp.Y = Utility.Utility.LERP(x, x0, x1, segmentForces[segmentForces.Count - 2].Y, segmentForces[segmentForces.Count - 1].Y) * x;
                                    temp.Z = Utility.Utility.LERP(x, x0, x1, segmentForces[segmentForces.Count - 2].Z, segmentForces[segmentForces.Count - 1].Z) * x;
                                }

                                lineForces[f - 1].locationIncrement -= x;
                                lineForces[f - 1].NextForces = lerpForce;

                                lineForces.Insert(f, new LineSupportForce(locationIncrement * f - x, x, lerpForce, wallForceList[f]));

                                forceIntegral.X += temp.X;
                                forceIntegral.Y += temp.Y;
                                forceIntegral.Z += temp.Z;

                                worksheet.Cells[row, "K"] = forceIntegral.Z;

                                if (f < segmentForces.Count)
                                {
                                    temp.X = Utility.Utility.LERP(x, x0, x1, segmentForces[f - 1].X, segmentForces[f].X) * (a - x);
                                    temp.Y = Utility.Utility.LERP(x, x0, x1, segmentForces[f - 1].Y, segmentForces[f].Y) * (a - x);
                                    temp.Z = Utility.Utility.LERP(x, x0, x1, segmentForces[f - 1].Z, segmentForces[f].Z) * (a - x);
                                }
                                else
                                {
                                    temp.X = Utility.Utility.LERP(x, x0, x1, segmentForces[segmentForces.Count - 2].X, segmentForces[segmentForces.Count - 1].X) * (a - x);
                                    temp.Y = Utility.Utility.LERP(x, x0, x1, segmentForces[segmentForces.Count - 2].Y, segmentForces[segmentForces.Count - 1].Y) * (a - x);
                                    temp.Z = Utility.Utility.LERP(x, x0, x1, segmentForces[segmentForces.Count - 2].Z, segmentForces[segmentForces.Count - 1].Z) * (a - x);
                                }

                                forceIntegral = temp;
                            }
                            else
                            {
                                forceIntegral.X += segmentForces[f].X;
                                forceIntegral.Y += segmentForces[f].Y;
                                forceIntegral.Z += segmentForces[f].Z;
                            }
                        }

                        row++;
                    }
                    row++;
                }
                row++;
            }

            RFemConnection.UnlockLicense();
        }

        private IModel ConnectExcelAndRfem(string modelName, out Excel.Worksheet _worksheet)
        {
            bool clearWB = true;

            Excel.Application excelApp = Marshal.GetActiveObject("Excel.Application") as Excel.Application;
            excelApp.Visible = true;
            Excel.Workbook workbook = excelApp.ActiveWorkbook;

            if (clearWB)
            {
                for (int i = 1; i <= workbook.Worksheets.Count; i++)
                {
                    //if (workbook.Worksheets[i].Name == "ExtremeValues" || workbook.Worksheets[i].Name == "AllValues")
                    {
                        if (workbook.Worksheets.Count > 1)
                        {
                            workbook.Worksheets[i].Delete();
                            i--;
                        }
                    }
                        
                }
            }

            Excel.Worksheet worksheet = workbook.Worksheets.Add();
            _worksheet = worksheet;
            //_worksheet = null;

            if (!RFemConnection.ConnectModel(modelName))
            {
                return null;
            }

            IModel iModel = RFemConnection.RfModel;
            return iModel;
        }
    }
    
    public class SupportForceData
    {
        public List<LoadCaseSupportForceData> loadCaseSupportForceData;
        public List<List<Vector3>> maxForceIntegral;
        public List<List<Vector3>> minForceIntegral;
        public List<List<Vector3>> maxForce;
        public List<List<Vector3>> minForce;

        public SupportForceData(List<LoadCaseSupportForceData> loadCaseSupportForceData)
        {
            this.loadCaseSupportForceData = loadCaseSupportForceData;
        }

        public void MinMaxIntegrals()
        {
            maxForceIntegral = new List<List<Vector3>>();
            minForceIntegral = new List<List<Vector3>>();

            for (int j = 0; j < loadCaseSupportForceData[0].wallSupportForces.Count; j++)
            {
                Vector3 maxValues = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 minValues = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                maxForceIntegral.Add(new List<Vector3>());                
                minForceIntegral.Add(new List<Vector3>());

                for (int k = 0; k < loadCaseSupportForceData[0].wallSupportForces[j].lineSupportIntegrals.Count; k++)
                {
                    maxForceIntegral[j].Add(maxValues);
                    minForceIntegral[j].Add(minValues);
                }
            }

            for (int i = 0; i < loadCaseSupportForceData.Count; i++)
            {
                for (int j = 0; j < loadCaseSupportForceData[i].wallSupportForces.Count; j++)
                {
                    for (int k = 0; k < loadCaseSupportForceData[i].wallSupportForces[j].lineSupportIntegrals.Count; k++)
                    {
                        Vector3 value = loadCaseSupportForceData[i].wallSupportForces[j].lineSupportIntegrals[k].value;
                        Vector3 maxValues = maxForceIntegral[j][k];
                        Vector3 minValues = minForceIntegral[j][k];

                        if (value.X > maxValues.X)
                        {
                            maxValues.X = value.X;
                        }
                        if (value.Y > maxValues.Y)
                        {
                            maxValues.Y = value.Y;
                        }
                        if (value.Z > maxValues.Z)
                        {
                            maxValues.Z = value.Z;
                        }

                        maxForceIntegral[j][k] = maxValues;

                        if (value.X < minValues.X)
                        {
                            minValues.X = value.X;
                        }
                        if (value.Y < minValues.Y)
                        {
                            minValues.Y = value.Y;
                        }
                        if (value.Z < minValues.Z)
                        {
                            minValues.Z = value.Z;
                        }

                        minForceIntegral[j][k] = minValues;
                    }
                }
            }
        }

        public void MinMaxForces()
        {
            maxForce = new List<List<Vector3>>();
            minForce = new List<List<Vector3>>();

            for (int j = 0; j < loadCaseSupportForceData[0].wallSupportForces.Count; j++)
            {
                Vector3 maxValues = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 minValues = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                maxForce.Add(new List<Vector3>());
                minForce.Add(new List<Vector3>());

                for (int k = 0; k < loadCaseSupportForceData[0].wallSupportForces[j].lineSupportForces.Count; k++)
                {
                    maxForce[j].Add(maxValues);
                    minForce[j].Add(minValues);
                }
            }

            for (int i = 0; i < loadCaseSupportForceData.Count; i++)
            {
                for (int j = 0; j < loadCaseSupportForceData[i].wallSupportForces.Count; j++)
                {
                    for (int k = 0; k < loadCaseSupportForceData[i].wallSupportForces[j].lineSupportForces.Count; k++)
                    {
                        Vector3 value = loadCaseSupportForceData[i].wallSupportForces[j].lineSupportForces[k].Forces;
                        Vector3 maxValues = maxForce[j][k];
                        Vector3 minValues = minForce[j][k];

                        if (value.X > maxValues.X)
                        {
                            maxValues.X = value.X;
                        }
                        if (value.Y > maxValues.Y)
                        {
                            maxValues.Y = value.Y;
                        }
                        if (value.Z > maxValues.Z)
                        {
                            maxValues.Z = value.Z;
                        }

                        maxForce[j][k] = maxValues;

                        if (value.X < minValues.X)
                        {
                            minValues.X = value.X;
                        }
                        if (value.Y < minValues.Y)
                        {
                            minValues.Y = value.Y;
                        }
                        if (value.Z < minValues.Z)
                        {
                            minValues.Z = value.Z;
                        }

                        minForce[j][k] = minValues;
                    }
                }
            }
        }
    }

    public class LoadCaseSupportForceData
    {
        public int loadingNumber;
        public List<WallSupportForceData> wallSupportForces;

        public LoadCaseSupportForceData(int loadingNumber, List<WallSupportForceData> wallSupportForces)
        {
            this.loadingNumber = loadingNumber;
            this.wallSupportForces = wallSupportForces;
        }
    }

    public class WallSupportForceData
    {
        public int wallNumber;
        public List<LineSupportForce> lineSupportForces;
        public List<LineSupportIntegral> lineSupportIntegrals;
        public Vector3 maxForce;
        public Vector3 minForce;

        public WallSupportForceData(int wallNumber, List<LineSupportForce> lineSupportForces)
        {
            this.wallNumber = wallNumber;
            this.lineSupportForces = lineSupportForces;
            this.maxForce = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            this.minForce = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        public void MaxMin()
        {
            foreach (LineSupportForce force in lineSupportForces)
            {
                if (force.Forces.X > maxForce.X)
                {
                    maxForce.X = force.Forces.X;
                }
                if (force.Forces.Y > maxForce.Y)
                {
                    maxForce.Y = force.Forces.Y;
                }
                if (force.Forces.Z > maxForce.Z)
                {
                    maxForce.Z = force.Forces.Z;
                }

                if (force.Forces.X < minForce.X)
                {
                    minForce.X = force.Forces.X;
                }
                if (force.Forces.Y < minForce.Y)
                {
                    minForce.Y = force.Forces.Y;
                }
                if (force.Forces.Z < minForce.Z)
                {
                    minForce.Z = force.Forces.Z;
                }
            }
        }
    }

    public class LineSupportIntegral
    {
        public float length;
        public Vector3 value;

        public LineSupportIntegral(float length, Vector3 value)
        {
            this.length = length;
            this.value = value;
        }
    }

    public class LineSupportForce
    {
        public float location;
        public float locationIncrement;

        public LineSupportForce(float location, float locationIncrement, Vector3 forces, Vector3 nextForces = new Vector3())
        {
            this.location = location;
            this.locationIncrement = locationIncrement;
            Forces = forces;
            NextForces = nextForces;
        }

        public Vector3 Forces { get; set; }
        public Vector3 NextForces { get; set; }
        public Vector3 ForceIncrement
        {
            get
            {
                if (NextForces == Vector3.Zero)
                {
                    return Vector3.Zero;
                }

                Vector3 temp = new Vector3();
                temp.X = (Forces.X + NextForces.X) / 2 * locationIncrement;
                temp.Y = (Forces.Y + NextForces.Y) / 2 * locationIncrement;
                temp.Z = (Forces.Z + NextForces.Z) / 2 * locationIncrement;

                return temp;
            }
        }
    }
}
