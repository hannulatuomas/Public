using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GH_GetRFemResults.GH;
using GH_GetRFemResults.RFem;
using GH_GetRFemResults.Utilities;
using GH_GUI;
using Grasshopper.Kernel;
using Dlubal.RFEM5;
using Rhino.Geometry;

namespace GH_GetRFemResults.GH_Components
{
    public class GhComponent_GetLoadingCase : GH_ExtendableComponent
    {
        MenuCheckBox checkBox;
        MenuStaticText menuStaticText;
        MenuDropDown loadingsDropDown;
        IModel iModel;
        LoadingCase selectedLoading = new LoadingCase();
        bool wait = true;
        bool run = false;
        List<LoadingCase> loadingsList;
        Action<object, EventArgs> action;

        /// <summary>
        /// Initializes a new instance of the GetLoadcase class.
        /// </summary>
        public GhComponent_GetLoadingCase()
          : base("GetLoadcase", "LoadCase",
              "",
              "RFemResults", "ModelData")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("RFemModel", "Model", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Loadcase", "Loadcase", "", GH_ParamAccess.item);
        }

        protected override void Setup(GH_ExtendableComponentAttributes attr)
        {
            action = dropDown_valueChanged;

            float componentWidth = 120f;
            int menuHeight = 50;
            float sideMarginal = 5f;
            float topMarginal = 20f;

            GH_MenuPanel gH_MenuPanel = new GH_MenuPanel();
            gH_MenuPanel.Pivot = new System.Drawing.PointF(sideMarginal, topMarginal);
            gH_MenuPanel.Height = menuHeight;
            gH_MenuPanel.Width = componentWidth - 2 * sideMarginal;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu();
            gH_ExtendableMenu.Expand();
            gH_ExtendableMenu.Name = "Run Component";
            gH_ExtendableMenu.Height = gH_MenuPanel.Height + 2 * sideMarginal;
            gH_ExtendableMenu.Header = "Run Component";

            this.checkBox = new MenuCheckBox(20, 15);
            checkBox._valueChanged += new ValueChangeEventHandler(this.checkBox_valueChanged);
            checkBox.Pivot = new System.Drawing.PointF(sideMarginal, sideMarginal);
            checkBox.Name = "Run";
            checkBox.Header = "Run Component";

            menuStaticText = new MenuStaticText();
            menuStaticText.Pivot = new System.Drawing.PointF(checkBox.Pivot.X + 30f, checkBox.Pivot.Y);
            menuStaticText.Text = "Click to run";
            menuStaticText.Header = "Click to run component";

            loadingsDropDown = new MenuDropDown("LoadCase");
            loadingsDropDown.Pivot = new System.Drawing.PointF(checkBox.Pivot.X, checkBox.Pivot.Y + 20f);
            loadingsDropDown._valueChanged += new ValueChangeEventHandler(this.dropDown_valueChanged);
            loadingsDropDown.Width = gH_MenuPanel.Width - 2 * sideMarginal;
            loadingsDropDown.Header = "Select load case or combination";
            loadingsDropDown.addItem("No Data");
            loadingsDropDown.VisibleItemCount = Math.Min(10, loadingsDropDown.Items.Count);

            gH_MenuPanel.AddControl(checkBox);
            gH_MenuPanel.AddControl(menuStaticText);
            gH_MenuPanel.AddControl(loadingsDropDown);
            gH_ExtendableMenu.AddControl(gH_MenuPanel);
            attr.AddMenu(gH_ExtendableMenu);
            attr.MinWidth = componentWidth;
        }

        private void dropDown_valueChanged(object sender, EventArgs e)
        {
            int value = ((MenuDropDown)sender).Value;
            if (((MenuDropDown)sender).Items.Count == 0)
            {
                return;
            }
            string selectedLoading = ((MenuDropDown)sender).Items[value].content;
            if (loadingsList != null && selectedLoading != "" && loadingsList.Count > 0)
            {
                this.selectedLoading = SelectLoading(selectedLoading, loadingsList);
            }
            this.ExpireSolution(true);
        }

        private void checkBox_valueChanged(object sender, EventArgs e)
        {
            bool value = ((MenuCheckBox)sender).Active;

            if (value && !run)
            {
                this.run = true;
                menuStaticText.Text = "Running...";
                this.OnObjectChanged(GH_ObjectEventType.Options);
                wait = true;
                this.ExpireSolution(true);
            }
            else if (value && run)
            {
                checkBox.Active = false;
                run = false;
                menuStaticText.Text = "Click to run";
                this.OnObjectChanged(GH_ObjectEventType.Options);
            }
        }

        List<string> GetLoadingNames()
        {
            List<string> loadingNames = new List<string>();
            
            if (loadingsList == null)
            {
                loadingsList = new List<LoadingCase>();
            }
            else
            {
                loadingsList.Clear();
            }

            //IApplication app = Marshal.GetActiveObject("RFEM5.Application") as IApplication;
            iModel.GetApplication().LockLicense();

            ILoads iLoads = iModel.GetLoads();
            //iLoads.GetLoadCasesAndCombos(ref loadingNames);
            // Load Cases
            LoadCase[] loadCaseArr = iLoads.GetLoadCases();
            int caseCount = iLoads.GetLoadCaseCount();
            for (int i = 0; i < caseCount; i++)
            {
                //ILoadCase iLC = iLoads.GetLoadCase(i, ItemAt.AtIndex);
                //LoadCase LC = iLC.GetData();
                LoadCase LC = loadCaseArr[i];

                int no = LC.Loading.No;
                LoadingType type = LC.Loading.Type;
                string name = "LC" + no.ToString() + " " + LC.Description;
                //string name = "LoadCase " + no.ToString();
                LoadingCase loading = new LoadingCase(no, type, name);
                loading.loadingIndex = no;
                loadingsList.Add(loading);
                loadingNames.Add(name);
            }

            //Load Combinations
            LoadCombination[] loadCombinationArr = iModel.GetLoads().GetLoadCombinations();
            int comboCount = iLoads.GetLoadCombinationCount();
            for (int i = 0; i < comboCount; i++)
            {
                //ILoadCombination iLC = iLoad.GetLoadCombination(i, ItemAt.AtIndex);
                //LoadCombination LC = iLC.GetData();
                LoadCombination LC = loadCombinationArr[i];

                int no = LC.Loading.No;
                LoadingType type = LC.Loading.Type;
                string name = "CO" + no.ToString() + " " + LC.Description;
                //string name = "LoadCombination " + no.ToString();
                LoadingCase loading = new LoadingCase(no, type, name);
                loading.loadingIndex = caseCount + no;
                loadingsList.Add(loading);
                loadingNames.Add(name);
            }

            //Result Combinations
            ResultCombination[] resultCombinationArr = (ResultCombination[])iLoads.GetResultCombinations();
            int rCombCount = iLoads.GetResultCombinationCount();
            for (int i = 0; i < rCombCount; i++)
            {
                //IResultCombination iLC = iLoads.GetResultCombination(i, ItemAt.AtIndex);
                //ResultCombination LC = iLC.GetData();
                ResultCombination LC = resultCombinationArr[i];

                int no = LC.Loading.No;
                LoadingType type = LC.Loading.Type;
                string name = "RC" + no.ToString() + " " + LC.Description;
                //string name = "ResultCombination " + no.ToString();
                LoadingCase loading = new LoadingCase(no, type, name);
                loading.loadingIndex = caseCount + comboCount + no;
                loadingsList.Add(loading);
                loadingNames.Add(name);
            }

            iModel.GetApplication().UnlockLicense();
            return loadingNames;
        }

        LoadingCase SelectLoading(string name, List<LoadingCase> loadings)
        {
            for (int i = 0; i < loadings.Count; i++)
            {
                if (loadings[i].name == name)
                {
                    return loadings[i];
                }
            }

            return loadings[0];
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (wait)
            {
                wait = false;
                this.ExpireSolution(true);
                return;
            }
            else
            {
                wait = true;
            }
            if (run && iModel != null)
            {
                List<string> loadingNames = GetLoadingNames();
                Utilities_GhComponent.UpdateDropDownMenu(loadingsDropDown, loadingNames, action);
            }
            checkBox_valueChanged(checkBox, EventArgs.Empty);

            RfemModel model = new RfemModel(null);

            if (!DA.GetData(0, ref model)) return;
            this.iModel = model.iModel;

            if (selectedLoading.name != "")
            {
                selectedLoading.model = iModel;
                DA.SetData(0, selectedLoading);
            }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                return Resource1.RFemLoadCaseIcon.ToBitmap();
                //return null;
            }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("e2a06e5f-32b3-48ec-8e4f-2dc31185cdc0"); }
        }
    }
}