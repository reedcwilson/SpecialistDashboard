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
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

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
        private BackgroundWorker InfoBackgroundWorker;

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
            RollsTabQueuesControl.QueuesRolls = new List<Roll>(); 
            RollsTabRollsControl = new Controls.RollsControl();
            RollsTabRollsControl.Rolls = new List<Roll>();
            rollsDisplay();

            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            this.InfoBackgroundWorker = new BackgroundWorker();
            this.InfoBackgroundWorker.DoWork += InfoBackgroundWorker_DoWork;
            this.InfoBackgroundWorker.RunWorkerCompleted += InfoBackgroundWorker_RunWorkerCompleted;

            QueuesNumbersInitializer("");
        }

        void QueuesNumbersInitializer(string project)
        {
            var argument = new GetRollsArgument(project);
            this.InfoBackgroundWorker.RunWorkerAsync(argument);
        }

        #region BackgroundWorker events

        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataContext data_context;
            var args = (GetRollsArgument)e.Argument;
            
            if (args.Tab == 0 && !args.ToggleChecked)
            {
                data_context = new DataContext("epdb01", "JWF_Live"); 
                var rollsReader = new QueuesRollsReader(data_context);
                DoWorkForQueuesRolls(e, rollsReader); 
            }
            else if (args.Tab == 0 && args.ToggleChecked)
            {
                data_context = new DataContext("epdb01", "Dexter_DeeDee"); 
                var rollsReader = new SpecialistRollsReader(data_context);
                DoWorkForSpecialistRolls(e, rollsReader);
            }
            else if (args.Tab == 1)
            {
                data_context = new DataContext("epdb01", "JWF_Live"); 
                var numbersReader = new DailyNumbersReader();
                DoWorkForNumbers(e, numbersReader);
            }
        }

        void InfoBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = (GetRollsArgument)e.Argument; 
            if (args.Tab == 2)
            {
                var rollCounter = new RollsCounter();
                DoWorkForQueuesNumbers(e, rollCounter);
            }
        }

        private int _rollBatchSizeQueue = 200;
        private int _rollBatchSizeRolls = 600;
        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var arg = (GetRollsArgument)e.Result;
            if (arg.Tab == 0 && Convert.ToBoolean(!rollsToggleBtn.IsChecked))
            {
                QueuesRollsThreadDisplay(e);
            }
            else if (arg.Tab == 0 && Convert.ToBoolean(rollsToggleBtn.IsChecked))
            {
                SpecialistRollsThreadDisplay(e);
            }
            else if (arg.Tab == 1)
            {
                NumbersThreadDisplay(e);
            }
        }

        void InfoBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var arg = (GetRollsArgument)e.Result;
            if (arg.Tab == 2)
            {
                QueuesNumbersThreadDisplay(e);
            }
        }

        private static void DoWorkForQueuesRolls(DoWorkEventArgs e, QueuesRollsReader rollsReader)
        {
            var args = (GetRollsArgument)e.Argument;

            List<Roll> rolls = rollsReader.GetRolls(
                        args.Step,
                        args.State,
                        args.Priority,
                        args.Specialist,
                        args.Project,
                        args.Roll);

            args.Rolls = rolls;
            e.Result = args;
        }

        private static void DoWorkForSpecialistRolls(DoWorkEventArgs e, SpecialistRollsReader rollsReader)
        {
            var args = (GetRollsArgument)e.Argument;

            List<Roll> rolls = rollsReader.GetRolls(
                        args.Specialist,
                        args.MinDate,
                        args.MaxDate,
                        args.Step,
                        args.Roll);

            args.Rolls = rolls;
            e.Result = args;
        }

        private static void DoWorkForNumbers(DoWorkEventArgs e, DailyNumbersReader numbersReader)
        {
            var args = (GetRollsArgument)e.Argument;

            var numList = numbersReader.GetNumbers(args.Specialist, args.Step);

            args.RollNumbers = numList;
            e.Result = args;
        }

        private static void DoWorkForQueuesNumbers(DoWorkEventArgs e, RollsCounter rollsCounter)
        {
            var args = (GetRollsArgument)e.Argument;
            var queuesNums = rollsCounter.GetQueueNumbers(args.Project);

            if (queuesNums != null)
            {
                var usedSpaceFinder = new DriveSpaceFinder();

                var scanningDiskSpace = usedSpaceFinder.NetDriveSpaceFinder(@"\\dpfs02\scanning");
                queuesNums.ScanningUsed = scanningDiskSpace.UsedSpace;
                queuesNums.ScanningTotal = scanningDiskSpace.TotalSpace;

                var imagingDiskSpace = usedSpaceFinder.NetDriveSpaceFinder(@"\\dpfs02\imaging");
                queuesNums.ImagingUsed = imagingDiskSpace.UsedSpace;
                queuesNums.ImagingTotal = imagingDiskSpace.TotalSpace;

                var stagingDiskSpace = usedSpaceFinder.NetDriveSpaceFinder(@"\\dpfs01\staging");
                queuesNums.StagingUsed = stagingDiskSpace.UsedSpace;
                queuesNums.StagingTotal = stagingDiskSpace.TotalSpace;

                args.QueueNumbers = queuesNums; 
            }
            e.Result = args;
        }

        private void QueuesRollsThreadDisplay(RunWorkerCompletedEventArgs e)
        {
            var arg = (GetRollsArgument)e.Result;
            if (arg.Rolls != null)
            {
                var rolls = (IEnumerable<Roll>)arg.Rolls;
                if (RollsTabQueuesControl.QueuesRolls != null)
                    RollsTabQueuesControl.QueuesRolls.Clear();
                foreach (var roll in rolls.Take(_rollBatchSizeQueue))
                    RollsTabQueuesControl.QueuesRolls.Add(roll);

                PopulateListView(RollsTabQueuesControl.queuesLv,
                    RollsTabQueuesControl.QueuesRolls,
                    RollsTabQueuesControl.emptyTxtBlck,
                    true);
                if (rolls.Count() > 200)
                {
                    LoadingProgressRing.IsActive = false;
                    MessageBox.Show(this,
                                    "Your query returned " + rolls.Count() + " results but only 200 have been displayed for performance reasons. Refine your filters and try again.",
                                    "Roll Limit Exceeded",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
                }
            }
            else
            {
                var rolls = new List<Roll>();
                PopulateListView(RollsTabQueuesControl.queuesLv, rolls, RollsTabQueuesControl.emptyTxtBlck, false);
            }
            WorkingDisplay(LoadingProgressRing, false);
            rollsToggleBtn.IsEnabled = true;
        }

        private void SpecialistRollsThreadDisplay(RunWorkerCompletedEventArgs e)
        {
            var arg = (GetRollsArgument)e.Result;
            if (arg.Rolls != null)
            {
                var rolls = (IEnumerable<Roll>)arg.Rolls;
                if (RollsTabRollsControl.Rolls != null)
                    RollsTabRollsControl.Rolls.Clear();
                foreach (var roll in rolls.Take(_rollBatchSizeRolls))
                    RollsTabRollsControl.Rolls.Add(roll);

                PopulateListView(RollsTabRollsControl.rollsLv,
                    RollsTabRollsControl.Rolls,
                    RollsTabRollsControl.emptyTxtBlck,
                    true);
                if (rolls.Count() > 600)
                {
                    LoadingProgressRing.IsActive = false;
                    MessageBox.Show(this,
                                    "Your query returned " + rolls.Count() + " results but only 600 have been displayed for performance reasons. Refine your filters and try again.",
                                    "Roll Limit Exceeded",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
                }
            }
            else
            {
                var rolls = new List<Roll>();
                PopulateListView(RollsTabRollsControl.rollsLv, rolls, RollsTabRollsControl.emptyTxtBlck, false);
            }
            WorkingDisplay(LoadingProgressRing, false);
            rollsToggleBtn.IsEnabled = true;
        }

        private void NumbersThreadDisplay(RunWorkerCompletedEventArgs e)
        {
            var arg = (GetRollsArgument)e.Result;
            if (arg.RollNumbers != null)
            {
                var numbers = (List<RollNumbers>)arg.RollNumbers;
                int images = 0;
                double imgsPerHour = 0;
                int seconds = 0;
                foreach (var num in numbers)
                {
                    images += num.ImageCount;
                    imgsPerHour += num.ImagesPerHour;
                    seconds += num.Seconds;
                }

                imgsPerHour = imgsPerHour / numbers.Count;
                double hours = (seconds / 60.0) / 60.0;

                numbersLv.ItemsSource = numbers;
                RollsNumLbl.Content = numbers.Count;
                ImagesLbl.Content = images;
                ImagesPerHourLbl.Content = Math.Round(imgsPerHour, 2);
                RollsPerHourLbl.Content = Math.Round(numbers.Count / hours, 2);
            }
            else emptyTxtBlck.Visibility = Visibility.Visible;

            WorkingDisplay(NumbersLoadingProgressRing, false);
        }

        private void QueuesNumbersThreadDisplay(RunWorkerCompletedEventArgs e)
        {
            var arg = (GetRollsArgument)e.Result;
            if (arg.QueueNumbers != null)
            {
                var queuesNums = (QueueNumbers)arg.QueueNumbers;
                ScanLbl.Content = queuesNums.Scan;
                FramingLbl.Content = queuesNums.Framing;
                MekelExtractingLbl.Content = queuesNums.MekelExtracting;
                BatchValidatingLbl.Content = queuesNums.BatchValidating;
                ImgProcLbl.Content = queuesNums.ImageProcessing;
                QALbl.Content = queuesNums.ImageQA;
                QELbl.Content = queuesNums.ImageQE;
                GridlinesLbl.Content = queuesNums.GridlinesQA;
                TotalLbl.Content = queuesNums.Scan +
                    queuesNums.Framing +
                    queuesNums.MekelExtracting +
                    queuesNums.BatchValidating +
                    queuesNums.ImageProcessing +
                    queuesNums.ImageQA +
                    queuesNums.ImageQE +
                    queuesNums.GridlinesQA;

                ScanningShareProgressBar.Maximum = queuesNums.ScanningTotal;
                ScanningShareProgressBar.Value = queuesNums.ScanningUsed;
                ImagingShareProgressBar.Maximum = queuesNums.ImagingTotal;
                ImagingShareProgressBar.Value = queuesNums.ImagingUsed;
                StagingShareProgressBar.Maximum = queuesNums.StagingTotal;
                StagingShareProgressBar.Value = queuesNums.StagingUsed;

                ScanningRatioLbl.Content = String.Format(@"{0:F1}/{1:F1} TB",queuesNums.ScanningUsed, queuesNums.ScanningTotal);
                ImagingRatioLbl.Content = String.Format(@"{0:F1}/{1:F1} TB", queuesNums.ImagingUsed, queuesNums.ImagingTotal);
                StagingRatioLbl.Content = String.Format(@"{0:F1}/{1:F1} TB", queuesNums.StagingUsed, queuesNums.StagingTotal);
            }
            else
            {
                ScanLbl.Content = "0";
                FramingLbl.Content = "0";
                MekelExtractingLbl.Content = "0";
                BatchValidatingLbl.Content = "0";
                ImgProcLbl.Content = "0";
                QALbl.Content = "0";
                QELbl.Content = "0";
                GridlinesLbl.Content = "0";
                TotalLbl.Content = "0";
            }
            WorkingDisplay(InfoLoadingProgressRing, false);
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
                System.Diagnostics.Process.Start("http://dpsystems/support/wiki/KnowledgeBase/Imaging/Quick/Roll_Note_Templates");
            else if (sender == auditPrioritiesLinkLbl)
                System.Diagnostics.Process.Start("http://dpsystems/support/wiki/KnowledgeBase/Imaging/ProjectsAndPriorities/Priorities");
            else if (sender == projectInstLinkLbl)
                System.Diagnostics.Process.Start("http://dpsystems/support/wiki/KnowledgeBase/Imaging/ProjectsAndPriorities/Project_Instructions");
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

        /// <summary>
        /// Changes the display while the background worker is working
        /// </summary>
        /// <param name="working"></param>
        private void WorkingDisplay(ProgressRing progressRing, bool working)
        {
            if (working)
            {
                progressRing.IsActive = true;
                tabControl.IsEnabled = false;
                rollsToggleBtn.IsEnabled = false;
            }
            else
            {
                progressRing.IsActive = false;
                tabControl.IsEnabled = true;
                rollsToggleBtn.IsEnabled = true;
            }
        }

        private void refreshBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (!this.BackgroundWorker.IsBusy && !this.InfoBackgroundWorker.IsBusy)
            {
                if (tabControl.SelectedIndex == 0)
                {
                    if (rollsToggleBtn.IsChecked == false)
                    {
                        RollsTabQueuesControl.queuesLv.ItemsSource = null;
                        RollsTabQueuesControl.queuesLv.Items.Clear();
                        QueuesRollsSearch();
                        return;
                    }
                    else if (rollsToggleBtn.IsChecked == true)
                    {
                        RollsTabRollsControl.rollsLv.ItemsSource = null;
                        RollsTabRollsControl.rollsLv.Items.Clear();
                        SpecialistRollsSearch();
                        return;
                    }
                }
                else if (tabControl.SelectedIndex == 1)
                {
                    numbersLv.ItemsSource = null;
                    numbersLv.Items.Clear();
                    NumbersSearch();
                    return;
                }
                else if (tabControl.SelectedIndex == 2)
                {
                    WorkingDisplay(InfoLoadingProgressRing, true);
                    QueuesNumbersInitializer(infoQueueProjectTxt.Text);
                } 
            }
        }

        private void NumbersSearch()
        {
            numbersLv.ItemsSource = null;
            numbersLv.Items.Clear();
            emptyTxtBlck.Visibility = Visibility.Hidden;
            var numbersReader = new DailyNumbersReader();
            var specReader = new SpecialistsReader();
            var spec = specReader.GetSpecialist(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            WorkingDisplay(NumbersLoadingProgressRing, true);
            var arg = new GetRollsArgument(spec,
                    numbersStepComboBox.Text);

            this.BackgroundWorker.RunWorkerAsync(arg);
            //NumList = numbersReader.GetNumbers(spec, numbersStepComboBox.Text);
        }

        private void SpecialistRollsSearch()
        {
            if (RollsTabRollsControl.rollStepComboBox.Text != "")
            {
                if (RollsTabRollsControl.rollsLv.Items.Count > 0)
                    RollsTabRollsControl.Rolls.Clear();

                var data_context = new DataContext("epdb01", "Dexter_DeeDee");
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

                WorkingDisplay(LoadingProgressRing, true);
                rollsToggleBtn.IsEnabled = false;
                var arg = new GetRollsArgument(spec,
                    Convert.ToDateTime(minDate),
                    Convert.ToDateTime(maxDate),
                    RollsTabRollsControl.rollStepComboBox.Text,
                    RollsTabRollsControl.rollRollnameTxt.Text.TrimEnd());

                this.BackgroundWorker.RunWorkerAsync(arg); 
            }
            else MessageBox.Show(this, "Please enter a Step", "Filter Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void QueuesRollsSearch()
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

            var sReader = new SpecialistsReader();
            var spec = sReader.GetSpecialist(null, RollsTabQueuesControl.userComboBox.Text);

            var step = RollsTabQueuesControl.stepComboBox.Text;
            var st = RollsTabQueuesControl.stateComboBox.Text;
            var priority = RollsTabQueuesControl.priorityComboBox.Text;
            var project = RollsTabQueuesControl.queueProjectTxt.Text.TrimEnd();
            var roll = RollsTabQueuesControl.queueRollTxt.Text.TrimEnd();

            WorkingDisplay(LoadingProgressRing, true);
            rollsToggleBtn.IsEnabled = false;
            var arg = new GetRollsArgument(step, st, priority, project, roll, spec);
            this.BackgroundWorker.RunWorkerAsync(arg);
            return;
        }

        public void PopulateListView<T>(ListView lv, List<T> list, TextBlock textBlock, bool rollsExist)
        {
            if (rollsExist)
            {
                lv.ItemsSource = null;
                lv.Items.Clear();
                lv.ItemsSource = list;
                textBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                lv.ItemsSource = null;
                lv.Items.Clear();
                textBlock.Visibility = Visibility.Visible;
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

                BitmapImage img = new BitmapImage(new Uri("/images/General-query.png", UriKind.Relative));
                ToggleBtnImg.Source = img;
            }
            else if (rollsToggleBtn.IsChecked == true)
            {
                rollsTab.Content = RollsTabRollsControl;

                BitmapImage img = new BitmapImage(new Uri("/images/Personal-query.png", UriKind.Relative));
                ToggleBtnImg.Source = img;
            }
        }

        private void tabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
                rollsToggleBtn.Visibility = Visibility.Visible;
            else rollsToggleBtn.Visibility = Visibility.Hidden;
        }

        #region Show Numbers methods

        int _lastCommand = 0;
        private void MetroWindow_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.F5))
            {
                refreshBtn_Click_1(sender, e);
                _lastCommand = 1;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightCtrl) && Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (_lastCommand != 2)
                {
                    RollsTabQueuesControl.Project = RollsTabQueuesControl.queueProjectTxt.Text;
                    RollsTabQueuesControl.RollName = RollsTabQueuesControl.queueRollTxt.Text;
                    RollsTabQueuesControl.State = RollsTabQueuesControl.stateComboBox.Text;
                    RollsTabQueuesControl.Step = RollsTabQueuesControl.stepComboBox.Text;
                    RollsTabQueuesControl.User = RollsTabQueuesControl.userComboBox.Text;
                    RollsTabQueuesControl.Priority = RollsTabQueuesControl.priorityComboBox.Text;

                    RollsTabQueuesControl.queueProjectTxt.Text = "Project Count: " + ListCounter(RollsTabQueuesControl.queueProjectTxt);
                    RollsTabQueuesControl.queueRollTxt.Text = "Roll Count: " + ListCounter(RollsTabQueuesControl.queueRollTxt);
                    RollsTabQueuesControl.stateComboBox.Text = "State Count: " + ListCounter(RollsTabQueuesControl.stateComboBox);
                    RollsTabQueuesControl.stepComboBox.Text = "Step Count: " + ListCounter(RollsTabQueuesControl.stepComboBox);
                    RollsTabQueuesControl.userComboBox.Text = "User Count: " + ListCounter(RollsTabQueuesControl.userComboBox);
                    RollsTabQueuesControl.priorityComboBox.Text = "Priority Count: " + ListCounter(RollsTabQueuesControl.priorityComboBox);

                    _lastCommand = 2;
                }
            }
            else
                _lastCommand = 3;

        }

        private void MetroWindow_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyUp(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.LeftAlt) || Keyboard.IsKeyUp(Key.RightCtrl) && Keyboard.IsKeyUp(Key.RightAlt))
            {
                if (_lastCommand == 2)
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
                else
                {

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
            else
            {
                return;
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
