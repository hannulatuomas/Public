using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Stabiliteettilaskenta.RFem
{
    public static class GetResults
    {
        public class SupportReactions
        {
            public static NodalSupportForces[] GetNodalReactions(IModel iModel, IResults iResults)
            {
                iModel.GetApplication().LockLicense();
                IModelData iModelData = iModel.GetModelData();

                NodalSupportForces[] supportForces = iResults.GetAllNodalSupportForces(false);

                iModel.GetApplication().UnlockLicense();
                return supportForces;
            }

            public static LineSupportForces[] GetLineReactions(IModel iModel, IResults iResults)
            {
                iModel.GetApplication().LockLicense();
                IModelData iModelData = iModel.GetModelData();

                LineSupportForces[] supportForces = iResults.GetLinesSupportForces(false);

                iModel.GetApplication().UnlockLicense();
                return supportForces;
            }
        }

        public static Vector3 GetNodeCoordinates(IModel iModel, int nodeNumber)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            INode iNode = iModelData.GetNode(nodeNumber, ItemAt.AtNo);
            Node node = iNode.GetData();
            Vector3 coord = new Vector3((float)node.X*1000, (float)node.Y*1000, (float)node.Z*1000);

            iModel.GetApplication().UnlockLicense();
            return coord;
        }

        public static Vector3 GetLineCoordinate(IModel iModel, int lineNumber)
        {
            iModel.GetApplication().LockLicense();
            IModelData iModelData = iModel.GetModelData();

            ILine iLine = iModelData.GetLine(lineNumber, ItemAt.AtNo);
            Line line = iLine.GetData();
            int pointCount = line.ControlPoints.Length -1;
            Vector3 startPoint = new Vector3((float)line.ControlPoints[0].X*1000, (float)line.ControlPoints[0].Y * 1000, (float)line.ControlPoints[0].Z * 1000);
            Vector3 endPoint = new Vector3((float)line.ControlPoints[pointCount].X * 1000, (float)line.ControlPoints[pointCount].Y * 1000, (float)line.ControlPoints[pointCount].Z * 1000);
            Vector3 v;
            if (startPoint.X > endPoint.X || startPoint.Y > endPoint.Y)
            {
                v = startPoint;
                startPoint = endPoint;
                endPoint = v;
            }
            Vector3 coord = new Vector3(startPoint.X + (endPoint.X- startPoint.X)/2, startPoint.Y + (endPoint.Y - startPoint.Y) / 2, startPoint.Z + (endPoint.Z - startPoint.Z) / 2);

            iModel.GetApplication().UnlockLicense();
            return coord;
        }
    }
}
