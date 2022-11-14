using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.RFem
{
    public struct RfemModel
    {
        public IModel iModel;

        public RfemModel(IModel _iModel)
        {
            iModel = _iModel;
        }
    }
}
