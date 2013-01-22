using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Specialist_Dashboard.Controls
{
    /// <summary>
    /// Interaction logic for DetailsContainerControl.xaml
    /// </summary>
    public partial class DetailsContainerControl : UserControl
    {
        public List<JobSpecControl> JobSpecControls { get; set; }
        public DetailsContainerControl(Roll roll)
        {
            InitializeComponent();
            JobSpecControls = new List<JobSpecControl>();
            JobSpecControls.Add(NewJobSpecControl(roll));
            //rollTabCtrlCC.Content = new JobSpecControl(roll);
            VisibilityChanged(true);
        }

        public JobSpecControl NewJobSpecControl(Roll roll)
        {
            var tab = new TabItem();
            var control = new JobSpecControl(roll);
            tab.Content = control;
            tab.Header = roll.RollName;
            DetailsTabControl.Items.Add(tab);
            VisibilityChanged(true);
            DetailsTabControl.SelectedIndex = DetailsTabControl.Items.Count -1;

            return control;
        }

        private void DetailsTabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (DetailsTabControl.Items.Count < 1)
            {
                VisibilityChanged(false);
                //QueuesControl.DeleteContainerControl(this);
            }
        }
        
        public void VisibilityChanged(bool visible)
        {
            if (visible)
                this.Visibility = Visibility.Visible;
            else
                this.Visibility = Visibility.Hidden;
        }

        private void BackBtn_Click_2(object sender, RoutedEventArgs e)
        {
            VisibilityChanged(false);
        }
    }
}
