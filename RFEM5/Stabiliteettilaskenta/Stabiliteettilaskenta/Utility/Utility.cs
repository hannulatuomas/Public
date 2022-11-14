using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures.Geometry3d;

namespace Stabiliteettilaskenta.Utility
{
    public static class Utility
    {
        public static double GetVectorAngle(Vector vector1, Vector vector2)
        {
            double angle = vector1.GetAngleBetween(vector2) * (180 / Math.PI);

            Vector dir = new Vector(vector2 - vector1);
            double length = dir.GetLength();
            dir.X = Math.Round(dir.X / length, 4);
            dir.Y = Math.Round(dir.Y / length, 4);
            dir.Z = Math.Round(dir.Z / length, 4);

            if (dir.X == 0 || dir.Y == 0 || dir.Z == 0)
            {
                if (dir.X == 0)
                {
                    if ((dir.Y < 0 && dir.Z > 0) || (dir.Y > 0 && dir.Z < 0))
                    {
                        angle = -angle;
                    }
                    else if (dir.Y == 0 || dir.Z == 0)
                    {
                        if (Math.Min(dir.Y, dir.Z) < 0)
                        {
                            angle = -angle;
                        }
                    }
                }
                else if (dir.Y == 0)
                {
                    if ((dir.X < 0 && dir.Z > 0) || (dir.X > 0 && dir.Z < 0))
                    {
                        angle = -angle;
                    }
                    else if (dir.Z == 0)
                    {
                        if (dir.X < 0)
                        {
                            angle = -angle;
                        }
                    }
                }
                else if (dir.Z == 0)
                {
                    if ((dir.X < 0 && dir.Y > 0) || (dir.X > 0 && dir.Y < 0))
                    {
                        angle = -angle;
                    }
                }
            }
            else if (dir.X > 0)
            {
                if (dir.Y > 0)
                {
                    if (dir.Z < 0)
                    {
                        angle = -angle;
                    }
                }
                else
                {
                    if (dir.Z < 0 == false)
                    {
                        angle = -angle;
                    }
                }
            }
            else if (dir.X < 0)
            {
                if (dir.Y < 0)
                {
                    if (dir.Z > 0)
                    {
                        angle = -angle;
                    }
                }
                else
                {
                    if (dir.Z > 0 == false)
                    {
                        angle = -angle;
                    }
                }
            }
            return angle;
        }

        public static float LERP(float x, float x0, float x1, float y0, float y1)
        {
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        static ToolTip tooltip = new ToolTip();

        public static void ShowTooltip(Control control, string text)
        {
            tooltip.AutoPopDelay = 5000;
            tooltip.InitialDelay = 500;
            tooltip.ReshowDelay = 500;
            tooltip.IsBalloon = false;
            tooltip.ShowAlways = true;
            tooltip.UseAnimation = true;
            tooltip.UseFading = true;
            tooltip.SetToolTip(control, text);
        }
    }
}
