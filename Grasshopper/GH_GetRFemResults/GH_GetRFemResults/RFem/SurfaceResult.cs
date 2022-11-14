using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.RFem
{
    public struct SurfaceResult
    {
        public int surfaceNo;
        public Point3d point;
        public float result;
        public string resultType;

        public SurfaceResult(int _surfNo, Point3d _point, float _result, string _resultType)
        {
            this.surfaceNo = _surfNo;
            this.point = _point;
            this.result = _result;
            this.resultType = _resultType;
        }
    }
}
