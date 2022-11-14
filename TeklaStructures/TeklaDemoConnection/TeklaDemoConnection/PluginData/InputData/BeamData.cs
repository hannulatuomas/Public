using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace TeklaDemoConnection
{
    public class BeamData
    {
        public Beam Beam { get; set; }

        public double ProfileWidth
        {
            get
            {
                double profileWidth = 0.0;
                Beam.GetReportProperty("PROFILE.WIDTH", ref profileWidth);
                return profileWidth;
            }
        }
        public double ProfileHeight
        {
            get
            {
                double profileHeight = 0.0;
                Beam.GetReportProperty("PROFILE.HEIGHT", ref profileHeight);
                return profileHeight;
            }
        }

        public double CoverThicknessTop;
        public double CoverThicknessBottom;
        public double CoverThicknessLeft;
        public double CoverThicknessRight;

        public double BottomHeight;
        public int bItem;

        public StirrupData stirrup;
        public StirrupData uBar;

        public BeamData()
        {
            this.stirrup = new StirrupData();
            this.uBar = new StirrupData();
        }
    }
}
