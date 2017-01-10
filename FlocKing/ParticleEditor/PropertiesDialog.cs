using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParticleEditor
{
    public partial class PropertiesDialog : Form
    {
        public object DataSource 
        {
            get { return propertyGrid1.SelectedObject; }
            set { propertyGrid1.SelectedObject = value; }
        }
        public PropertiesDialog()
        {
            InitializeComponent();
        }
        public event Action PropertyChanged;
        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged();
            }
        }
    }
}
