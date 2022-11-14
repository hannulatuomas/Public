using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.RFem
{
    public static class Select
    {
        public static List<string> PickLineList(IModel iModel)
        {
            List<string> lines = new List<string>();
            string lineString = "";

            iModel.GetApplication().LockLicense();
            IModelData modelData = iModel.GetModelData();
            IView view = iModel.GetActiveView();
            view.PickObjects(ToolType.SelectLines, ref lineString);
            string[] lineArr = lineString.Split(',');

            foreach (string l in lineArr)
            {
                if (l.Contains("-"))
                {
                    string[] boundaryLines = l.Split('-');
                    int startLine = int.Parse(boundaryLines[0]);
                    int endLine = int.Parse(boundaryLines[1]);

                    for (int i = startLine; i <= endLine; i++)
                    {
                        lines.Add(i.ToString());
                    }
                }
                else
                {
                    lines.Add(l);
                }
            }
            iModel.GetApplication().UnlockLicense();
            return lines;
        }

        public static string PickLines(IModel iModel)
        {
            string lineString = "";

            iModel.GetApplication().LockLicense();
            IView view = iModel.GetActiveView();
            view.PickObjects(ToolType.SelectLines, ref lineString);
            iModel.GetApplication().UnlockLicense();
            
            return lineString;
        }

        public static string PickNodes(IModel iModel)
        {
            string nodeString = "";

            iModel.GetApplication().LockLicense();
            IView view = iModel.GetActiveView();
            view.PickObjects(ToolType.SelectNodes, ref nodeString);
            iModel.GetApplication().UnlockLicense();
            
            return nodeString;
        }

        public static string PickSurfaces(IModel iModel)
        {
            string surfaceString = "";

            iModel.GetApplication().LockLicense();
            IView view = iModel.GetActiveView();
            view.PickObjects(ToolType.SelectSurfaces, ref surfaceString);
            iModel.GetApplication().UnlockLicense();

            return surfaceString;
        }

        public static string PickMembers(IModel iModel)
        {
            string memberString = "";

            iModel.GetApplication().LockLicense();
            IView view = iModel.GetActiveView();
            view.PickObjects(ToolType.SelectMembers, ref memberString);
            iModel.GetApplication().UnlockLicense();

            return memberString;
        }
    }
}
