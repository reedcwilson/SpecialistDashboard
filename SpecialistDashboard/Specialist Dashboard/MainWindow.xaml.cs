using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Specialist_Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public QueuesControl RollsTabQueuesControl { get; set; }
        public Controls.RollsControl RollsTabRollsControl { get; set; }
        public List<string> SpecNames { get; set; }
        public List<RollNumbers> NumList { get; set; }

        private BackgroundWorker BackgroundWorker;

        public MainWindow()
        {
            InitializeComponent();

            SpecialistsReader sr = new SpecialistsReader();
            var specialists = sr.GetSpecialists();
            var specNames = new List<string>();
            foreach (var specialist in specialists)
            {
                specNames.Add(specialist.SpecialistName);
            }
            SpecNames = specNames;


            RollsTabQueuesControl = new QueuesControl(SpecNames);
            RollsTabRollsControl = new Controls.RollsControl();
            rollsDisplay();

            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        #region BackgroundWorker events

        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var data_context = new DataContext("epdb01", "JWF_Live");
            var rollsReader = new QueuesRollsReader(data_context);
            
            var args = (GetRollsArgument) e.Argument;

            RollsTabQueuesControl.QueuesRolls = rollsReader.GetRolls(
                        args.Step,
                        args.State,
                        args.Priority,
                        args.Specialist,
                        args.Project,
                        args.Roll);
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (RollsTabQueuesControl.QueuesRolls != null)
            {
                //everyone says I need to use invoke
                //this.invoke or use this.begininvoke
                RollsTabQueuesControl.queuesLv.Dispatcher.Invoke(new Action(() => { PopulateListView(true); }));
            }
            else
                PopulateListView(false);
        }

        #endregion

        #region MouseEvents
        private void LinkLbl_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Label).Foreground = Brushes.LightCoral;
        }

        private void LinkLbl_MouseLeave_1(object sender, MouseEventArgs e)
        {
            (sender as Label).Foreground = Brushes.DarkSlateGray;
        }

        private void LinkLbl_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            if (sender == rollNoteLinkLbl)
                System.Diagnostics.Process.Start("http://dpsystems/support/wiki/KnowledgeBase/Imaging/GeneralHelp/Roll%20Notes");
            else if (sender == auditPrioritiesLinkLbl)
                System.Diagnostics.Process.Start("http://dpsystems/support/wiki/KnowledgeBase/Imaging/GeneralHelp/PrioritiesAndInstructionsForSpecialists");
            else if (sender == projectInstLinkLbl)
                //    System.Diagnostics.Process.Start("");
                MessageBox.Show("Tell Sam or Miranda to make a project instructions wiki page");
            else if (sender == shiftReplacementLinkLbl)
                System.Diagnostics.Process.Start("http://intranet/teams/dp/imaging/Lists/Shift%20Replacement/Open.aspx");
            else if (sender == scanShareLinkLbl)
                System.Diagnostics.Process.Start(@"\\dpfs02\scanning");
            else if (sender == dexterToolsLinkLbl)
                System.Diagnostics.Process.Start(@"\\dpfs01\tools\DexterLaunchpad");
            else if (sender == adminToolsLinkLbl)
                System.Diagnostics.Process.Start(@"\\dpfs01\dpsfiler\Imaging Admin Tools");
            else if (sender == ezLaborLinkLbl)
                System.Diagnostics.Process.Start("https://ezlmappdc1f.adp.com/ezLaborManagerNet/Login/Login.aspx?cID=1514&lng=en-US");
        }
        #endregion


        private void refreshBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                if (rollsToggleBtn.IsChecked == false)
                {
                    if (RollsTabQueuesControl.stepComboBox.Text == "" && 
                        RollsTabQueuesControl.stateComboBox.Text == "" && 
                        RollsTabQueuesControl.priorityComboBox.Text == "" && 
                        RollsTabQueuesControl.userComboBox.Text == "" && 
                        RollsTabQueuesControl.queueProjectTxt.Text == "" && 
                        RollsTabQueuesControl.queueRollTxt.Text == "")
                    {
                        MessageBox.Show(this, "Please enter at least one filter", "Filter Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    if (RollsTabQueuesControl.queuesLv.Items.Count > 0)
                        RollsTabQueuesControl.QueuesRolls.Clear();


                    //var data_context = new DataContext("epdb01", "JWF_Live");
                    //var rollsReader = new QueuesRollsReader(data_context);
                    var sReader = new SpecialistsReader();
                    var spec = sReader.GetSpecialist(null, RollsTabQueuesControl.userComboBox.Text);

                    //start a new thread to do expensive query
                    //var syncContext = System.Threading.SynchronizationContext.Current;

                    var step = RollsTabQueuesControl.stepComboBox.Text;
                    var st = RollsTabQueuesControl.stateComboBox.Text;
                    var priority = RollsTabQueuesControl.priorityComboBox.Text;
                    var project = RollsTabQueuesControl.queueProjectTxt.Text;
                    var roll = RollsTabQueuesControl.queueRollTxt.Text;

                    var arg = new GetRollsArgument(step, st, priority, project, roll, spec);
                    this.BackgroundWorker.RunWorkerAsync(arg);

                    //System.Threading.Tasks.Task.Factory.StartNew(() =>
                    //    {
                    //        RollsTabQueuesControl.QueuesRolls = rollsReader.GetRolls(
                    //            step,
                    //            st,
                    //            priority,
                    //            spec,
                    //            project,
                    //            roll);

                        //    if (RollsTabQueuesControl.QueuesRolls != null)
                        //    {
                        //        RollsTabQueuesControl.queuesLv.Dispatcher.Invoke(() => { RollsTabQueuesControl.queuesLv.ItemsSource = RollsTabQueuesControl.QueuesRolls; });
                        //        RollsTabQueuesControl.emptyTxtBlck.Dispatcher.Invoke(() => { RollsTabQueuesControl.emptyTxtBlck.Visibility = Visibility.Hidden; });
                        //    }
                        //    else
                        //        PopulateListView(false);
                        //});

                    
                }
                else if (rollsToggleBtn.IsChecked == true)
                {
                    if (RollsTabRollsControl.rollStepComboBox.Text != "")
                    {
                        if (RollsTabRollsControl.rollsLv.Items.Count > 0)
                            RollsTabRollsControl.Rolls.Clear();

                        var data_context = new DataContext("epdb01", "JWF_Live");
                        var rollsReader = new SpecialistRollsReader(data_context);

                        string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        var sReader = new SpecialistsReader();
                        var spec = sReader.GetSpecialist(user);

                        var minDate = RollsTabRollsControl.fromDTPick.SelectedDate;
                        var maxDate = RollsTabRollsControl.toDTPick.SelectedDate;

                        if (minDate == null)
                            minDate = System.DateTime.Today;
                        if (maxDate == null)
                            maxDate = System.DateTime.Now;

                        RollsTabRollsControl.Rolls = rollsReader.GetRolls(
                            spec,
                            Convert.ToDateTime(minDate),
                            Convert.ToDateTime(maxDate),
                            RollsTabRollsControl.rollStepComboBox.Text,
                            RollsTabRollsControl.rollRollnameTxt.Text);

                        if (RollsTabRollsControl.rollsLv != null)
                        {
                            RollsTabRollsControl.rollsLv.ItemsSource = RollsTabRollsControl.Rolls;
                            RollsTabRollsControl.emptyTxtBlck.Visibility = Visibility.Hidden;
                        }
                        else RollsTabRollsControl.emptyTxtBlck.Visibility = Visibility.Visible;
                        RollsTabRollsControl.rollsLv.ItemsSource = RollsTabRollsControl.Rolls;
                    }
                    else MessageBox.Show(this, "Please enter a Step", "Filter Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else if (tabControl.SelectedIndex == 1)
            {
                numbersLv.ItemsSource = null;
                numbersLv.Items.Clear();
                emptyTxtBlck.Visibility = Visibility.Hidden;
                var numbersReader = new DailyNumbersReader();
                var specReader = new SpecialistsReader();
                var spec = specReader.GetSpecialist(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                NumList = numbersReader.GetNumbers(spec, numbersStepComboBox.Text);

                if (NumList != null)
                {
                    int images = 0;
                    double imgsPerHour = 0;
                    int seconds = 0;
                    foreach (var num in NumList)
                    {
                        images += num.ImageCount;
                        imgsPerHour += num.ImagesPerHour;
                        seconds += num.Seconds;
                    }
                    imgsPerHour = imgsPerHour / NumList.Count;
                    double hours = (seconds / 60.0) / 60.0;

                    numbersLv.ItemsSource = NumList;
                    RollsNumLbl.Content = NumList.Count;
                    ImagesLbl.Content = images;
                    ImagesPerHourLbl.Content = Math.Round(imgsPerHour, 2);
                    RollsPerHourLbl.Content = Math.Round(NumList.Count / hours, 2);
                }
                else emptyTxtBlck.Visibility = Visibility.Visible;
            }
        }

        public void PopulateListView(bool rollsExist)
        {
            if (rollsExist)
            {
                RollsTabQueuesControl.queuesLv.ItemsSource = RollsTabQueuesControl.QueuesRolls;
                RollsTabQueuesControl.emptyTxtBlck.Visibility = Visibility.Hidden;
            }
            else
            {
                RollsTabQueuesControl.queuesLv.ItemsSource = null;
                RollsTabQueuesControl.queuesLv.Items.Clear();
                RollsTabQueuesControl.emptyTxtBlck.Visibility = Visibility.Visible;
            }
        }

        private void rollsToggleBtn_Click_1(object sender, RoutedEventArgs e)
        {
            rollsDisplay();
        }

        public void rollsDisplay()
        {
            if (rollsToggleBtn.IsChecked == false)
            {
                rollsTab.Content = RollsTabQueuesControl;
            }
            else if (rollsToggleBtn.IsChecked == true)
            {

                rollsTab.Content = RollsTabRollsControl;
            }
        }

        private void tabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                rollsToggleBtn.Visibility = Visibility.Visible;
            else rollsToggleBtn.Visibility = Visibility.Hidden;
        }

        #region Show Numbers methods

        private void MetroWindow_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.F5))
                refreshBtn_Click_1(sender, e);
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightCtrl) && Keyboard.IsKeyDown(Key.RightAlt))
            {
                RollsTabQueuesControl.queueProjectTxt.Text = "Project Count: " + ListCounter(RollsTabQueuesControl.queueProjectTxt);
                RollsTabQueuesControl.queueRollTxt.Text = "Roll Count: " + ListCounter(RollsTabQueuesControl.queueRollTxt);
                RollsTabQueuesControl.stateComboBox.Text = "State Count: " + ListCounter(RollsTabQueuesControl.stateComboBox);
                RollsTabQueuesControl.stepComboBox.Text = "Step Count: " + ListCounter(RollsTabQueuesControl.stepComboBox);
                RollsTabQueuesControl.userComboBox.Text = "User Count: " + ListCounter(RollsTabQueuesControl.userComboBox);
                RollsTabQueuesControl.priorityComboBox.Text = "Priority Count: " + ListCounter(RollsTabQueuesControl.priorityComboBox);
            }
        }

        private void MetroWindow_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.LeftAlt) || Keyboard.IsKeyUp(Key.RightCtrl) && Keyboard.IsKeyUp(Key.RightAlt))
            {
                if (RollsTabQueuesControl.queueProjectTxt.Text.Length >= 14 && RollsTabQueuesControl.queueProjectTxt.Text.Substring(0, 14) == "Project Count:")
                {
                    RollsTabQueuesControl.queueProjectTxt.Text = RollsTabQueuesControl.Project;
                    RollsTabQueuesControl.queueRollTxt.Text = RollsTabQueuesControl.RollName;
                    RollsTabQueuesControl.stateComboBox.Text = RollsTabQueuesControl.State;
                    RollsTabQueuesControl.stepComboBox.Text = RollsTabQueuesControl.Step;
                    RollsTabQueuesControl.userComboBox.Text = RollsTabQueuesControl.User;
                    RollsTabQueuesControl.priorityComboBox.Text = RollsTabQueuesControl.Priority;
                }
            }
        }

        //counts the number of items in the listview per filter
        private int ListCounter(object sender)
        {
            if (RollsTabQueuesControl.queuesLv.Items.Count > 0)
            {
                var list = new List<string>();

                if (sender == RollsTabQueuesControl.queueRollTxt)
                    return RollsTabQueuesControl.queuesLv.Items.Count;

                foreach (var roll in RollsTabQueuesControl.QueuesRolls)
                {
                    if (list.Count != 0)
                    {
                        bool newItem = true;
                        foreach (var item in list)
                        {
                            if (sender == RollsTabQueuesControl.queueProjectTxt)
                            {
                                if (roll.ProjectId == item)
                                {
                                    newItem = false;
                                    break;
                                }
                            }
                            else if (sender == RollsTabQueuesControl.stateComboBox)
                            {
                                if (roll.State == item)
                                {
                                    newItem = false;
                                    break;
                                }
                            }
                            else if (sender == RollsTabQueuesControl.stepComboBox)
                            {
                                if (roll.Step == item)
                                {
                                    newItem = false;
                                    break;
                                }
                            }
                            else if (sender == RollsTabQueuesControl.userComboBox)
                            {
                                if (roll.Spec.Username == item)
                                {
                                    newItem = false;
                                    break;
                                }
                            }
                            else if (sender == RollsTabQueuesControl.priorityComboBox)
                                if (Convert.ToString(roll.Priority) == item)
                                {
                                    newItem = false;
                                    break;
                                }
                        }
                        if (newItem)
                        {
                            if (sender == RollsTabQueuesControl.queueProjectTxt)
                                list.Add(roll.ProjectId);
                            else if (sender == RollsTabQueuesControl.stateComboBox)
                                list.Add(roll.State);
                            else if (sender == RollsTabQueuesControl.stepComboBox)
                                list.Add(roll.Step);
                            else if (sender == RollsTabQueuesControl.userComboBox)
                                list.Add(roll.Spec.Username);
                            else if (sender == RollsTabQueuesControl.priorityComboBox)
                                list.Add(Convert.ToString(roll.Priority));
                        }
                    }
                    else
                    {
                        if (sender == RollsTabQueuesControl.queueProjectTxt)
                            list.Add(roll.ProjectId);
                        else if (sender == RollsTabQueuesControl.stateComboBox)
                            list.Add(roll.State);
                        else if (sender == RollsTabQueuesControl.stepComboBox)
                            list.Add(roll.Step);
                        else if (sender == RollsTabQueuesControl.userComboBox)
                            list.Add(roll.Spec.Username);
                        else if (sender == RollsTabQueuesControl.priorityComboBox)
                            list.Add(Convert.ToString(roll.Priority));
                    }
                }
                return list.Count();
            }
            else return 0;
        }

        #endregion

        GridViewColumnHeader _lastHeaderClicked = null;
        int _lastDirection = 0;
        private void GridViewColumn_Click_1(object sender, RoutedEventArgs e)
        {
            if (NumList != null)
            {
                GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
                string header = headerClicked.Column.Header as string;

                if (headerClicked != null)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        SortColumn(header);
                        _lastDirection = 1;
                    }
                    else
                    {
                        if (_lastDirection == 1)
                        {
                            _lastDirection = -1;
                            NumList.Reverse();
                            ReassignListview();
                        }
                        else
                        {
                            _lastDirection = 1;
                            SortColumn(header);
                        }
                    }
                }
                _lastHeaderClicked = headerClicked; 
            }
        }

        private void SortColumn(string header)
        {
            if (header.ToLower() == "roll name")
            {
                NumList.Sort();
                ReassignListview();
            }
            else if (header.ToLower() == "image count")
            {
                var sortOnImageCount = new SortOnImageCount();
                NumList.Sort(sortOnImageCount);
                ReassignListview();
            }
            else if (header.ToLower() == "images per hour")
            {
                var sortOnImagesPerHour = new SortOnImagesPerHour();
                NumList.Sort(sortOnImagesPerHour);
                ReassignListview();
            }
        }

        private void ReassignListview()
        {
            numbersLv.ItemsSource = null;
            numbersLv.Items.Clear();
            numbersLv.ItemsSource = NumList;
        }
    }
}
