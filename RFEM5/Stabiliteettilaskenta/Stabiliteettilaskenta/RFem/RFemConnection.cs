using Dlubal.RFEM5;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;

namespace Stabiliteettilaskenta
{
    public static class RFemConnection
    {
        private static IModel _rfModel;
        private static IApplication _rfApplication;

        public static IModel RfModel
        {
            get
            {
                if (_rfModel == null)
                {
                    ConnectModel();
                }

                return _rfModel;
            }
            private set
            {
                _rfModel = value;
            }
        }
        public static IApplication RfApplication
        {
            get
            {
                if (_rfModel == null)
                {
                    Connect();
                }

                return _rfApplication;
            }
            private set
            {
                _rfApplication = value;
            }
        }

        private static void Connect()
        {
            try
            {
                if (IsUserValid())
                {
                    RfApplication = Marshal.GetActiveObject("RFEM5.Application") as IApplication;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Connection Failed!", "ERROR");
                return;
            }
        }

        public static void LockLicense()
        {
            if (RfApplication != null)
            {
                RfApplication.LockLicense();
            }
        }
        public static void UnlockLicense()
        {
            if (RfApplication != null)
            {
                RfApplication.UnlockLicense();
            }
        }

        public static void RefreshGUI()
        {
            if (RfApplication != null)
            {
                RfApplication.RefreshGUI();
            }
        }

        public static List<string> GetModels()
        {
            List<string> names = new List<string>();
            IModel model;
            string modelName;

            if (RfApplication == null)
            {
                return null;
            }

            try
            {
                RfApplication.LockLicense();
                int modelCount = RfApplication.GetModelCount();

                for (int i = 0; i < modelCount; i++)
                {
                    model = RfApplication.GetModel(i);
                    modelName = model.GetName();
                    names.Add(modelName);
                }
                RfApplication.UnlockLicense();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Connection Failed!", "ERROR");
                return null;
            }

            return names;
        }

        public static bool ConnectModel(string name)
        {
            IModel model;
            string modelName;

            if (RfApplication == null)
            {
                return false;
            }

            try
            {
                RfApplication.LockLicense();
                int modelCount = RfApplication.GetModelCount();

                for (int i = 0; i < modelCount; i++)
                {
                    model = RfApplication.GetModel(i);
                    modelName = model.GetName();

                    if (modelName == name)
                    {
                        RfModel = model;
                        RfApplication.UnlockLicense();
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Connection Failed!", "ERROR");
                return false;
            }
            return false;
        }

        public static bool ConnectModel()
        {
            if (RfApplication == null)
            {
                return false;
            }

            try
            {
                RfApplication.LockLicense();
                RfModel = RfApplication.GetActiveModel();
                RfApplication.UnlockLicense();
                return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Connection Failed!", "ERROR");
                return false;
            }
        }

        private static bool IsUserValid()
        {
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            string username = currentIdentity.Name;

            if (username.Trim().StartsWith("AINS"))
            {
                return true;
            }
            return false;

        }
    }
}