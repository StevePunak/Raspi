using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KanoopControls.DataGridViewDB
{
    public partial class DataGridViewDoubleBuffered : DataGridView
    {
        // https://stackoverflow.com/a/27836679

        public DataGridViewDoubleBuffered()
        {
            DoubleBuffered = true;
        }
    }

}
