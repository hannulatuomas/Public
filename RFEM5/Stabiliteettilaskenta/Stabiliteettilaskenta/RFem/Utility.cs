using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stabiliteettilaskenta.RFem
{
    class Utility
    {
        public static IResults GetResults(IModel iModel, LoadingCase loadingCase)
        {
            iModel.GetApplication().LockLicense();
            ICalculation calculation = iModel.GetCalculation();
            IResults iResults;

            try
            {
                iResults = calculation.GetResultsInFeNodes(loadingCase.loadingType, loadingCase.loadingNumber);
            }
            catch (Exception exception)
            {
                ErrorInfo[] errors = (ErrorInfo[])calculation.Calculate(loadingCase.loadingType, loadingCase.loadingNumber);
                if (errors != null && errors.Length > 0)
                {
                    MessageBox.Show("Error during the calculation: " + errors[0].Description);
                    iModel.GetApplication().UnlockLicense();
                    return null;
                }
                iResults = calculation.GetResultsInFeNodes(loadingCase.loadingType, loadingCase.loadingNumber);
            }
            iModel.GetApplication().UnlockLicense();
            return iResults;
        }

        public static LoadingType GetLoadingType(string _loadingType)
        {
            LoadingType loadingType = LoadingType.LoadCaseType;
            switch (_loadingType)
            {
                case "Load Case":
                    loadingType = LoadingType.LoadCaseType;
                    break;
                case "Load Combination":
                    loadingType = LoadingType.LoadCombinationType;
                    break;
                case "Result Combination":
                    loadingType = LoadingType.ResultCombinationType;
                    break;
                default:
                    break;
            }
            return loadingType;
        }
    }

    // Namespace

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