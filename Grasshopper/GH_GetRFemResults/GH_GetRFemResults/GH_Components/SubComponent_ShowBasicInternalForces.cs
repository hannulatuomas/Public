using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_GetRFemResults.RFem;
using GH_GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Dlubal.RFEM5;

namespace GH_GetRFemResults.GH_Components
{
    class SubComponent_ShowBasicInternalForces : SubComponent
    {
        //GhComponent_ShowSurfaceResult parent;

        //public SubComponent_ShowBasicInternalForces(GhComponent_ShowSurfaceResult _parent)
        //{
        //    parent = _parent;
        //}
        public override string name()
        {
            return "BasicInternalForces";
        }

        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), "BasicInternalForces", "Show basic internal forces of surfaces");
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        protected void Setup(EvaluationUnit unit)
        {
            unit.RegisterInputParam(new Param_Boolean(), "Calculate", "Calculate", "", GH_ParamAccess.item);
            unit.RegisterInputParam(new Param_GenericObject(), "RFemIModel", "RFemIModel", "", GH_ParamAccess.item);

            GH_MenuPanel gH_MenuPanel = new GH_MenuPanel();
            gH_MenuPanel.Pivot = new System.Drawing.PointF(5f, 25f);
            gH_MenuPanel.Height = 135;
            gH_MenuPanel.Width = 100;
            gH_MenuPanel.AdjustWidth = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu();
            gH_ExtendableMenu.Name = "Select Outputs";
            gH_ExtendableMenu.Height = gH_MenuPanel.Height + 20;
            gH_ExtendableMenu.Header = "Select Outputs";
            gH_ExtendableMenu.Expand();
            

            int checkBoxSize = 10;
            MenuCheckBox checkBoxMx = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxMx.Pivot = new System.Drawing.PointF(gH_MenuPanel.Pivot.X, 10f);
            checkBoxMx.Name = "MomentMx";
            checkBoxMx.Header = "MomentMx";
            checkBoxMx.Active = true;
            //checkBoxMx._valueChanged += new ValueChangeEventHandler(parent.checkBox_valueChanged);
            MenuStaticText textMx = new MenuStaticText();
            textMx.Name = "MomentMx";
            textMx.Header = "MomentMx";
            textMx.Text = "MomentMx";
            textMx.Pivot = new System.Drawing.PointF(checkBoxMx.Pivot.X + 20, checkBoxMx.Pivot.Y);

            MenuCheckBox checkBoxMy = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxMy.Pivot = new System.Drawing.PointF(checkBoxMx.Pivot.X, checkBoxMx.Pivot.Y + 15f);
            checkBoxMy.Name = "MomentMy";
            checkBoxMy.Header = "MomentMy";
            //checkBoxMy._valueChanged += new ValueChangeEventHandler(parent.checkBox_valueChanged);
            MenuStaticText textMy = new MenuStaticText();
            textMy.Name = "MomentMy";
            textMy.Header = "MomentMy";
            textMy.Text = "MomentMy";
            textMy.Pivot = new System.Drawing.PointF(checkBoxMy.Pivot.X + 20f, checkBoxMy.Pivot.Y);

            MenuCheckBox checkBoxMxy = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxMxy.Pivot = new System.Drawing.PointF(checkBoxMy.Pivot.X, checkBoxMy.Pivot.Y + 15f);
            checkBoxMxy.Name = "MomentMxy";
            checkBoxMxy.Header = "MomentMxy";
            checkBoxMxy._valueChanged += new ValueChangeEventHandler(checkBox_valueChanged);
            MenuStaticText textMxy = new MenuStaticText();
            textMxy.Name = "MomentMxy";
            textMxy.Header = "MomentMxy";
            textMxy.Text = "MomentMxy";
            textMxy.Pivot = new System.Drawing.PointF(checkBoxMxy.Pivot.X + 20f, checkBoxMxy.Pivot.Y);

            MenuCheckBox checkBoxVx = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxVx.Pivot = new System.Drawing.PointF(checkBoxMxy.Pivot.X, checkBoxMxy.Pivot.Y + 15f);
            checkBoxVx.Name = "ShearForceVx";
            checkBoxVx.Header = "ShearForceVx";
            checkBoxVx._valueChanged += new ValueChangeEventHandler(checkBox_valueChanged);
            MenuStaticText textVx = new MenuStaticText();
            textVx.Name = "ShearForceVx";
            textVx.Header = "ShearForceVx";
            textVx.Text = "ShearForceVx";
            textVx.Pivot = new System.Drawing.PointF(checkBoxVx.Pivot.X + 20f, checkBoxVx.Pivot.Y);

            MenuCheckBox checkBoxVy = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxVy.Pivot = new System.Drawing.PointF(checkBoxVx.Pivot.X, checkBoxVx.Pivot.Y + 15f);
            checkBoxVy.Name = "ShearForceVy";
            checkBoxVy.Header = "ShearForceVy";
            checkBoxVy._valueChanged += new ValueChangeEventHandler(checkBox_valueChanged);
            MenuStaticText textVy = new MenuStaticText();
            textVy.Name = "ShearForceVy";
            textVy.Header = "ShearForceVy";
            textVy.Text = "ShearForceVy";
            textVy.Pivot = new System.Drawing.PointF(checkBoxVy.Pivot.X + 20f, checkBoxVy.Pivot.Y);

            MenuCheckBox checkBoxNx = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxNx.Pivot = new System.Drawing.PointF(checkBoxVy.Pivot.X, checkBoxVy.Pivot.Y + 15f);
            checkBoxNx.Name = "AxialForceNx";
            checkBoxNx.Header = "AxialForceNx";
            checkBoxNx._valueChanged += new ValueChangeEventHandler(checkBox_valueChanged);
            MenuStaticText textNx = new MenuStaticText();
            textNx.Name = "AxialForceNx";
            textNx.Header = "AxialForceNx";
            textNx.Text = "AxialForceNx";
            textNx.Pivot = new System.Drawing.PointF(checkBoxNx.Pivot.X + 20f, checkBoxNx.Pivot.Y);

            MenuCheckBox checkBoxNy = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxNy.Pivot = new System.Drawing.PointF(checkBoxNx.Pivot.X, checkBoxNx.Pivot.Y + 15f);
            checkBoxNy.Name = "AxialForceNy";
            checkBoxNy.Header = "AxialForceNy";
            checkBoxNy._valueChanged += new ValueChangeEventHandler(checkBox_valueChanged);
            MenuStaticText textNy = new MenuStaticText();
            textNy.Name = "AxialForceNy";
            textNy.Header = "AxialForceNy";
            textNy.Text = "AxialForceNy";
            textNy.Pivot = new System.Drawing.PointF(checkBoxNy.Pivot.X + 20f, checkBoxNy.Pivot.Y);

            MenuCheckBox checkBoxNxy = new MenuCheckBox(checkBoxSize, checkBoxSize);
            checkBoxNxy.Pivot = new System.Drawing.PointF(checkBoxNy.Pivot.X, checkBoxNy.Pivot.Y + 15f);
            checkBoxNxy.Name = "AxialForceNxy";
            checkBoxNxy.Header = "AxialForceNxy";
            checkBoxNxy._valueChanged += new ValueChangeEventHandler(checkBox_valueChanged);
            MenuStaticText textNxy = new MenuStaticText();
            textNxy.Name = "AxialForceNxy";
            textNxy.Header = "AxialForceNxy";
            textNxy.Text = "AxialForceNxy";
            textNxy.Pivot = new System.Drawing.PointF(checkBoxNxy.Pivot.X + 20f, checkBoxNxy.Pivot.Y);

            //if (checkBoxMx.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "MomentMx", "mX", "");
            //}
            //if (checkBoxMy.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "MomentMy", "mY", "");
            //}
            //if (checkBoxMxy.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "MomentMxy", "mXY", "");
            //}
            //if (checkBoxVx.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "ShearForceVx", "vX", "");
            //}
            //if (checkBoxVy.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "ShearForceVy", "vY", "");
            //}
            //if (checkBoxNx.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "AxialForceNx", "nX", "");
            //}
            //if (checkBoxNy.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "AxialForceNy", "nY", "");
            //}
            //if (checkBoxNxy.Active)
            //{
            //    unit.RegisterOutputParam(new Param_Number(), "AxialForceNxy", "nXY", "");
            //}
            unit.RegisterOutputParam(new Param_Number(), "MomentMx", "mX", "");
            unit.RegisterOutputParam(new Param_Number(), "MomentMy", "mY", "");
            unit.RegisterOutputParam(new Param_Number(), "MomentMxy", "mXY", "");
            unit.RegisterOutputParam(new Param_Number(), "ShearForceVx", "vX", "");
            unit.RegisterOutputParam(new Param_Number(), "ShearForceVy", "vY", "");
            unit.RegisterOutputParam(new Param_Number(), "AxialForceNx", "nX", "");
            unit.RegisterOutputParam(new Param_Number(), "AxialForceNy", "nY", "");
            unit.RegisterOutputParam(new Param_Number(), "AxialForceNxy", "nXY", "");

            gH_MenuPanel.AddControl(checkBoxMx);
            gH_MenuPanel.AddControl(checkBoxMy);
            gH_MenuPanel.AddControl(checkBoxMxy);
            gH_MenuPanel.AddControl(checkBoxVx);
            gH_MenuPanel.AddControl(checkBoxVy);
            gH_MenuPanel.AddControl(checkBoxNx);
            gH_MenuPanel.AddControl(checkBoxNy);
            gH_MenuPanel.AddControl(checkBoxNxy);

            gH_MenuPanel.AddControl(textMx);
            gH_MenuPanel.AddControl(textMy);
            gH_MenuPanel.AddControl(textMxy);
            gH_MenuPanel.AddControl(textVx);
            gH_MenuPanel.AddControl(textVy);
            gH_MenuPanel.AddControl(textNx);
            gH_MenuPanel.AddControl(textNy);
            gH_MenuPanel.AddControl(textNxy);
            gH_ExtendableMenu.AddControl(gH_MenuPanel);

            for (int i = 0; i < unit.Outputs.Count; i++)
            {
                gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[i]);
            }
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[0]);
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[1]);
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[2]);
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[3]);
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[4]);
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[5]);
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[6]);
            //gH_ExtendableMenu.RegisterOutputPlug(unit.Outputs[7]);

            unit.AddMenu(gH_ExtendableMenu);
        }

        private void checkBox_valueChanged(object sender, EventArgs e)
        {
            bool isChecked = ((MenuCheckBox)sender).Active;
            string name = ((MenuCheckBox)sender).Name;
            if (isChecked)
            {
                MessageBox.Show("CheckBox " + name + " is active");
            }
            else
            {
                MessageBox.Show("CheckBox " + name + " is not active");
            }
            //parent.ExpireSolution(true);
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            SurfaceResult_old surfaceResult = new SurfaceResult_old();

            bool calculate = false;
            IModel iModel = null;

            if (!DA.GetData(0, ref calculate)) return;
            if (!DA.GetData(1, ref iModel)) return;

            if (calculate == true)
            {
                // Get LoadCase/LoadCombination
                // Get Mesh
                // Get Result
            }
        }
    }
}
