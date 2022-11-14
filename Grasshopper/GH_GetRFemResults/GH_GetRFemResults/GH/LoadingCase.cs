using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_GetRFemResults.GH
{
    public struct LoadingCase
    {
        public string name;
        public int loadingNumber;
        public LoadingType loadingType;
        public IModel model;
        public int loadingIndex;

        public LoadingCase(int _loadingNumber, LoadingType _loadingType, string _name = "")
        {
            if (_name != "")
            {
                name = _name;
            }
            else
            {
                name = _loadingType.ToString() + " " + _loadingNumber.ToString();
            }
            loadingNumber = _loadingNumber;
            loadingType = _loadingType;
            model = null;
            loadingIndex = loadingNumber;
        }
    }
}
