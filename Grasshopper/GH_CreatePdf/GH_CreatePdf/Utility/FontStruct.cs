using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_CreatePdf.Utility
{
    public struct FontStruct
    {
        public XFont font;
        public Color color;

        public FontStruct(XFont _font, Color _color)
        {
            this.font = _font;
            this.color = _color;
        }
    }
}
