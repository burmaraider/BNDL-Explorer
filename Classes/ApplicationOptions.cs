using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNDL_Explorer
{
    public class ApplicationOptions
    {
        private System.Drawing.Color texBackgroundColor;
        public bool test;
        
        public ApplicationOptions()
        {
            SetTexBackgroundColor(Color.Red);
        }

        public System.Drawing.Color GetTexBackgroundColor()
        {

            return texBackgroundColor;
        }
        public void SetTexBackgroundColor(Color col)
        {

            texBackgroundColor = col;
        }



    }
}
