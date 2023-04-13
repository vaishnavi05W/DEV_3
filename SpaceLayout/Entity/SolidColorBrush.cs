using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SpaceLayout.Entity
{
     public abstract class SolidColorBrush
    {
        public Color color;

        public SolidColorBrush(Color color)
        {
            this.color = color;
        }

      
    }
}
