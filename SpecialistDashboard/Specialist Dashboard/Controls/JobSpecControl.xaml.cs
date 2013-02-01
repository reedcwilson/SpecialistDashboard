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
using System.IO;
using System.ComponentModel;

namespace Specialist_Dashboard
{
    /// <summary>
    /// Interaction logic for JobSpecControl.xaml
    /// </summary>
    public partial class JobSpecControl : UserControl
    {
        private RollDetails MyRoll { get; set; }
        public Roll MyBasicRoll { get; set; }
        public JobSpecUpdates JSUpdates { get; set; }
        public AllJobSpecUpdates AllJSUpdates { get; set; }
        public ProjectSpecUpdates ProjectSpecUpdates { get; set; }
        public RollPaths MyRollPaths { get; set; }
        public List<ImageNote> FilteredImageNotes { get; set; }

        private BackgroundWorker BackgroundWorker;

        public JobSpecControl(Roll roll)
        {
            InitializeComponent();
            
            MyBasicRoll = roll;


            var nReader = new NoteReader();
            var notes = nReader.GetNotes(MyBasicRoll);

            var hReader = new HistoryReader();
            var histories = hReader.GetHistories(MyBasicRoll);

            //add image notes reader here then initialize a new roll with image notes
            var iNReader = new ImageNotesReader();
            var imageNotes = iNReader.GetNotes(MyBasicRoll);

            MyRoll = new RollDetails(MyBasicRoll.Id,
                MyBasicRoll.ProjectId,
                MyBasicRoll.RollName,
                MyBasicRoll.Spec,
                MyBasicRoll.State,
                MyBasicRoll.Step,
                MyBasicRoll.LastUpdate,
                notes,
                histories,
                imageNotes);

            /// <summary>
            /// Sets ListViews Item Sources
            /// </summary>
            var rDetails = new List<Roll>();
            rDetails.Add(MyBasicRoll);

            DetailsProjectTxt.Text = MyBasicRoll.ProjectId;
            DetailsRollnameTxt.Text = MyBasicRoll.RollName;
            StepLbl.Content = MyBasicRoll.Step;
            StateLbl.Content = MyBasicRoll.State;

            historyLv.ItemsSource = MyRoll.Histories;
            notesLv.ItemsSource = MyRoll.Notes;
            imgNotesLv.ItemsSource = MyRoll.ImageNotes;

            /// <summary>
            /// Sets the values for QE tab
            /// </summary>

            MyRollPaths = new RollPaths(MyBasicRoll);
            JSUpdates = new JobSpecUpdates(MyBasicRoll, MyRollPaths);
            if (JSUpdates.RootElement != null)
            {
                QETabEnabler(true);
                autoCropTS.IsChecked = JSUpdates.AutoCrop;
                if (autoCropTS.IsChecked == false)
                    aggFactorTS.IsEnabled = false;
                aggFactorTS.IsChecked = JSUpdates.AggressiveFactor;
                cropPaddingTxt.Text = Convert.ToString(JSUpdates.CropPadding);
                deskewComboBox.Text = Convert.ToString(JSUpdates.DeskewMaxAngle / 100);
            }
            else
            {
                QETabEnabler(false);
            }

            if (MyRollPaths.GetRootPath() != null)
            {
                RootFolderHLink.Inlines.Clear();
                RootFolderHLink.Inlines.Add(MyRollPaths.GetRootPath());
            }
            RootFolderHLink.IsEnabled = PathExists(MyRollPaths.GetRootPath());

            if (MyRollPaths.GetImgProcessPath() != null)
            {
                ImgProcHLink.Inlines.Clear();
                ImgProcHLink.Inlines.Add(MyRollPaths.GetImgProcessPath());
            }
            ImgProcHLink.IsEnabled = PathExists(MyRollPaths.GetImgProcessPath());

            if (MyRollPaths.GetIdxPath() != null)
            {
                IdxImgHLink.Inlines.Clear();
                IdxImgHLink.Inlines.Add(MyRollPaths.GetIdxPath());
            }
            IdxImgHLink.IsEnabled = PathExists(MyRollPaths.GetIdxPath());

            if (MyRollPaths.GetSiteSuitePath() != null)
            {
                SiteSuiteHLink.Inlines.Clear();
                SiteSuiteHLink.Inlines.Add(MyRollPaths.GetSiteSuitePath());
            }
            SiteSuiteHLink.IsEnabled = PathExists(MyRollPaths.GetSiteSuitePath());

            this.BackgroundWorker = new BackgroundWorker();
            this.BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            this.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            this.BackgroundWorker.RunWorkerAsync();
        }

        private void QETabEnabler(bool enabled)
        {
            autoCropTS.IsEnabled = enabled;
            aggFactorTS.IsEnabled = enabled;
            cropPaddingTxt.IsEnabled = enabled;
            deskewComboBox.IsEnabled = enabled;
            projectSpecBtn.IsEnabled = enabled;
            JobSpecBtn.IsEnabled = enabled;
            allJobSpecsBtn.IsEnabled = enabled;
            JobSpecOpenHLink.IsEnabled = enabled;
        }

        private bool PathExists(string path)
        {
            bool exists = true;
            if (!Directory.Exists(path))
                exists = false;
            return exists;
        }

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
            if (sender == projectChangesLinkLbl)
                System.Diagnostics.Process.Start("http://dpsystems/support/wiki/KnowledgeBase/Imaging/GeneralHelp/Project%20Production%20Changes");
            else if (sender == qeTrainingLinkLbl)
                System.Diagnostics.Process.Start(@"\\dpfs01\dpsfiler\Imaging\Production_Team\Team_Lead\Reed\Training\QE\QE_Training.pptx");
            else if (sender == projectDefectsLinkLbl)
                System.Diagnostics.Process.Start(@"\\dpfs01\dpsfiler\Imaging\Production_Team\Project Defect Logs");
            else if (sender == defectLogLinkLbl)
                System.Diagnostics.Process.Start(@"\\dpfs01\dpsfiler\Imaging\Production_Team\Supervisors\QA_Defects_Log.xlsx");
        }

        private void HighlightText_GotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e)
        {
            if ((sender as TextBox).Text != "Rollname" && (sender as TextBox).Text != "Project")
                (sender as TextBox).SelectAll();
            else (sender as TextBox).Text = "";
        }

        private void HighlightText_GotMouseCapture(Object sender, MouseEventArgs e)
        {
            if ((sender as TextBox).Text != "Rollname" && (sender as TextBox).Text != "Project")
                (sender as TextBox).SelectAll();
            else (sender as TextBox).Text = "";
        }

        private void TextBox_LostFocus_1(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                if (sender == cropPaddingTxt)
                    (sender as TextBox).Text = "Crop Padding";
            }
        }

        private void aggFactorTS_Click_1(object sender, RoutedEventArgs e)
        {
            if (aggFactorTS.IsChecked == true)
                JSUpdates.AggressiveFactor = true;
            else if (aggFactorTS.IsChecked == false)
                JSUpdates.AggressiveFactor = false;
        }

        private void autoCropTS_Click_1(object sender, RoutedEventArgs e)
        {
            if (autoCropTS.IsChecked == true)
            {
                JSUpdates.AutoCrop = true;
                aggFactorTS.IsEnabled = true;
                cropPaddingTxt.IsEnabled = true;
            }
            else if (autoCropTS.IsChecked == false)
            {
                JSUpdates.AutoCrop = false;
                aggFactorTS.IsEnabled = false;
                cropPaddingTxt.IsEnabled = false;
            }
        }

        private void JobSpecBtn_Click_1(object sender, RoutedEventArgs e)
        {
            JSUpdates.CropPadding = Convert.ToInt32(cropPaddingTxt.Text);
            JSUpdates.DeskewMaxAngle = Convert.ToInt32(deskewComboBox.Text) * 100;
        }

        private void historyLv_Selected_1(object sender, RoutedEventArgs e)
        {
            if (historyLv.SelectedItems.Count > 0)
            {
                HistoryUserLbl.Content = MyRoll.Histories[historyLv.SelectedIndex].Spec.Username;

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(MyRoll.Histories[historyLv.SelectedIndex].Message));
                var flowdoc = new FlowDocument();
                flowdoc.Blocks.Add(paragraph);
                MessageRichTxt.Document = flowdoc;
            }
            else
            {
                MessageRichTxt.Document.Blocks.Clear();
                HistoryUserLbl.Content = "User";
            }
        }

        private void notesLv_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (notesLv.SelectedItems.Count > 0)
            {
                NotesUserLbl.Content = MyRoll.Notes[notesLv.SelectedIndex].Spec.Username;

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(MyRoll.Notes[notesLv.SelectedIndex].NoteMessage));
                var flowdoc = new FlowDocument();
                flowdoc.Blocks.Add(paragraph);
                NoteRichTxt.Document = flowdoc;

                if (@"myfamily\" + MyRoll.Notes[notesLv.SelectedIndex].Spec.Username == System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToLower())
                {
                    NoteRichTxt.IsReadOnly = false;
                }
                else NoteRichTxt.IsReadOnly = true;
            }
            else
            {
                NoteRichTxt.Document.Blocks.Clear();
                NotesUserLbl.Content = "User";
            }
        }

        private void RootFolderHLink_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(MyRollPaths.GetRootPath());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open link: " + ex);
            }
        }

        private void ImgProcHLink_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(MyRollPaths.GetImgProcessPath());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open link: " + ex);
            }
        }

        private void IdxImgHLink_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(MyRollPaths.GetIdxPath());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open link: " + ex);
            }
        }

        private void SiteSuiteHLink_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(MyRollPaths.GetSiteSuitePath());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open link: " + ex);
            }
        }

        private void ImgNotesLv_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (imgNotesLv.SelectedItems.Count > 0)
            {
                ImgNoteUpdateTimeLbl.Content = MyRoll.ImageNotes[imgNotesLv.SelectedIndex].UpdateTime;

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(MyRoll.ImageNotes[imgNotesLv.SelectedIndex].NoteMessage));
                var flowdoc = new FlowDocument();
                flowdoc.Blocks.Add(paragraph);
                ImgNoteMessageRichTxt.Document = flowdoc;
            }
            else
            {
                ImgNoteMessageRichTxt.Document.Blocks.Clear();
                ImgNoteUpdateTimeLbl.Content = "Update Time";
            }
        }

        private void textGenerate_Click_1(object sender, RoutedEventArgs e)
        {
            if (MyRoll.ImageNotes != null)
            {
                if (MyRoll.ImageNotes.Count() != 0)
                {
                    var mesAnalyzer = new MessageAnalyzer();
                    var categories = mesAnalyzer.Analyze(MyBasicRoll.RollName);
                    var sBuilder = new StringBuilder();

                    foreach (var category in categories)
                    {
                        if (category.Value.Count > 0)
                        {
                            sBuilder.AppendLine("Images Marked As " + category.Key + ":");
                            int i = 0;
                            foreach (var note in category.Value)
                            {
                                sBuilder.Append(note.ImageNum);
                                if (category.Value.Count > 1)
                                {
                                    if (i != category.Value.Count - 1)
                                        sBuilder.Append(", ");
                                }
                                i++;
                            }
                            sBuilder.AppendLine("");
                            sBuilder.AppendLine("");
                        }
                    }
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run(sBuilder.ToString()));
                    var flowdoc = new FlowDocument();
                    flowdoc.Blocks.Add(paragraph);
                    ImgNoteMessageRichTxt.Document = flowdoc;
                }
            }
        }

        private void JobSpecOpenHLink_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", MyRollPaths.JobSpec);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open link: " + ex);
            }
        }

        private void allJobSpecsBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var rollPaths = new RollPaths(MyBasicRoll);
            AllJSUpdates = new AllJobSpecUpdates();
            var jobSpecDirectory = rollPaths.GetJobSpec() + @"\" + MyBasicRoll.ProjectId;
            foreach (var file in Directory.GetFiles(jobSpecDirectory))
            {
                AllJSUpdates.Initialize(MyBasicRoll.ProjectId, System.IO.Path.GetFileName(file));
                AllJSUpdates.AutoCrop = Convert.ToBoolean(autoCropTS.IsChecked);
                AllJSUpdates.AggressiveFactor = Convert.ToBoolean(aggFactorTS.IsChecked);
                AllJSUpdates.DeskewMaxAngle = Convert.ToInt32(deskewComboBox.Text) * 100;
                AllJSUpdates.CropPadding = Convert.ToInt32(cropPaddingTxt.Text);
            }
        }

        private void projectSpecBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var rollPaths = new RollPaths(MyBasicRoll);
            this.ProjectSpecUpdates = new ProjectSpecUpdates();
            var fileName = System.IO.Path.GetDirectoryName(MyRollPaths.GetJobSpec()) + @"\ProjectSpecs\" + MyBasicRoll.ProjectId + @".xml";
            //this.ProjectSpecUpdates.Initialize(MyBasicRoll.ProjectId, System.IO.Path.GetFileName(ProjectSpecUpdates.JobSpecPath));
            this.ProjectSpecUpdates.Initialize(fileName);
            this.ProjectSpecUpdates.AutoCrop = Convert.ToBoolean(autoCropTS.IsChecked);
            this.ProjectSpecUpdates.AggressiveFactor = Convert.ToBoolean(aggFactorTS.IsChecked);
            this.ProjectSpecUpdates.DeskewMaxAngle = Convert.ToInt32(deskewComboBox.Text) * 100;
            this.ProjectSpecUpdates.CropPadding = Convert.ToInt32(cropPaddingTxt.Text);
        }

        #region historyLv sort

        GridViewColumnHeader _lastHeaderClicked = null;
        int _lastDirection = 0;
        private void GridViewColumn_Click_1(object sender, RoutedEventArgs e)
        {
            if (MyRoll.Histories != null)
            {
                GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
                string header = headerClicked.Column.Header as string;

                if (headerClicked != null)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        SortHistoryColumn(header);
                        _lastDirection = 1;
                    }
                    else
                    {
                        if (_lastDirection == 1)
                        {
                            _lastDirection = -1;
                            MyRoll.Histories.Reverse();
                            ReassignListview(historyLv, MyRoll.Histories);
                        }
                        else
                        {
                            _lastDirection = 1;
                            SortHistoryColumn(header);
                        }
                    }
                }
                _lastHeaderClicked = headerClicked;
            }
        }

        private void SortHistoryColumn(string header)
        {
            if (header.ToLower() == "date")
            {
                MyRoll.Histories.Sort();
                ReassignListview(historyLv, MyRoll.Histories);
            }
            else if (header.ToLower() == "step")
            {
                var sortOnHistoryStep = new SortOnHistoryStep();
                MyRoll.Histories.Sort(sortOnHistoryStep);
                ReassignListview(historyLv, MyRoll.Histories);
            }
        }

        #endregion

        #region notesLv sort

        GridViewColumnHeader _lastNoteHeaderClicked = null;
        int _lastNoteDirection = 0;
        private void GridViewColumn_Click_2(object sender, RoutedEventArgs e)
        {
            if (MyRoll.Notes != null)
            {
                GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
                string header = headerClicked.Column.Header as string;

                if (headerClicked != null)
                {
                    if (headerClicked != _lastNoteHeaderClicked)
                    {
                        SortNoteColumn(header);
                        _lastNoteDirection = 1;
                    }
                    else
                    {
                        if (_lastNoteDirection == 1)
                        {
                            _lastNoteDirection = -1;
                            MyRoll.Notes.Reverse();
                            ReassignListview(notesLv, MyRoll.Notes);
                        }
                        else
                        {
                            _lastNoteDirection = 1;
                            SortNoteColumn(header);
                        }
                    }
                }
                _lastNoteHeaderClicked = headerClicked;
            }
        }

        private void SortNoteColumn(string header)
        {
            if (header.ToLower() == "date")
            {
                MyRoll.Notes.Sort();
                ReassignListview(notesLv, MyRoll.Notes);
            }
            else if (header.ToLower() == "step")
            {
                var sortOnNoteStep = new SortOnNoteStep();
                MyRoll.Notes.Sort(sortOnNoteStep);
                ReassignListview(notesLv, MyRoll.Notes);
            }
        }

        #endregion

        #region imageNotesLv sort

        GridViewColumnHeader _lastImageNoteHeaderClicked = null;
        int _lastImageNoteDirection = 0;
        private void GridViewColumn_Click_3(object sender, RoutedEventArgs e)
        {
            if (MyRoll.ImageNotes != null)
            {
                GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
                string header = headerClicked.Column.Header as string;

                if (headerClicked != null)
                {
                    if (headerClicked != _lastImageNoteHeaderClicked)
                    {
                        SortImageNoteColumn(header);
                        _lastImageNoteDirection = 1;
                    }
                    else
                    {
                        if (_lastImageNoteDirection == 1)
                        {
                            _lastImageNoteDirection = -1;
                            MyRoll.ImageNotes.Reverse();
                            ReassignListview(imgNotesLv, MyRoll.ImageNotes);
                        }
                        else
                        {
                            _lastImageNoteDirection = 1;
                            SortImageNoteColumn(header);
                        }
                    }
                }
                _lastImageNoteHeaderClicked = headerClicked;
            }
        }

        private void SortImageNoteColumn(string header)
        {
            if (header.ToLower() == "image")
            {
                MyRoll.ImageNotes.Sort();
                ReassignListview(imgNotesLv, MyRoll.ImageNotes);
            }
            else if (header.ToLower() == "user")
            {
                var sortOnImageNoteUser = new SortOnImageNoteUser();
                MyRoll.ImageNotes.Sort(sortOnImageNoteUser);
                ReassignListview(imgNotesLv, MyRoll.ImageNotes);
            }
        }

        #endregion

        private void ReassignListview<T>(ListView lv, List<T> list)
        {
            lv.ItemsSource = null;
            lv.Items.Clear();
            lv.ItemsSource = list;
        }

        private void FilteredListTS_Click_1(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(FilteredListTS.IsChecked))
            {
                if (MyRoll.ImageNotes.Count != 0)
                {
                    FilteredImageNotes = new List<ImageNote>();
                    foreach (var note in MyRoll.ImageNotes)
                    {
                        if (note.NoteMessage.Count() > 17)
                        {
                            if (note.NoteMessage.ToLower().Substring(0, 17) != "automated message")
                                FilteredImageNotes.Add(note);
                        }
                        else
                        {
                            FilteredImageNotes.Add(note);
                        }
                    }
                    imgNotesLv.ItemsSource = null;
                    imgNotesLv.Items.Clear();
                    imgNotesLv.ItemsSource = FilteredImageNotes;
                }
            }
            else
            {
                imgNotesLv.ItemsSource = null;
                imgNotesLv.Items.Clear();
                imgNotesLv.ItemsSource = MyRoll.ImageNotes;
            }

        }

        private void currentTextGenerate_Click_1(object sender, RoutedEventArgs e)
        {
            var logReader = new QALogReader(MyBasicRoll);
            var images = logReader.GetImages();
            var sBuilder = new StringBuilder();

            foreach (var category in images)
            {
                if (category.Value.Count > 0)
                {
                    if (category.Key.ToLower() != "midtonebright")
                    {
                        sBuilder.AppendLine("Images Marked As " + category.Key + ":");
                        int i = 0;
                        foreach (var image in category.Value)
                        {
                            sBuilder.Append(image);
                            if (category.Value.Count > 1)
                            {
                                if (i != category.Value.Count - 1)
                                    sBuilder.Append(", ");
                            }
                            i++;
                        }
                        sBuilder.AppendLine("");
                        sBuilder.AppendLine("");
                    } 
                }
            }
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(sBuilder.ToString()));
            var flowdoc = new FlowDocument();
            flowdoc.Blocks.Add(paragraph);
            ImgNoteMessageRichTxt.Document = flowdoc; 
        }

        #region BackgroundWorker events

        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = new JobSpecsControlArgument();
            var rollPaths = new RollPaths(MyBasicRoll);
            
            if (Directory.Exists(rollPaths.GetRootImagesPath()))
	        {
		        args.RootImages = Directory.GetFiles(rollPaths.GetRootImagesPath()).Count(
                        p => System.IO.Path.GetExtension(p) == ".jpg" || 
                            System.IO.Path.GetExtension(p) == ".j2k" || 
                            System.IO.Path.GetExtension(p) == ".tif"); 
	        }

            if (Directory.Exists(rollPaths.GetImgProcessPath()))
            {
                args.ProcessedImages = Directory.GetFiles(rollPaths.GetImgProcessPath()).Count(
                    p => System.IO.Path.GetExtension(p) == ".jpg" ||
                        System.IO.Path.GetExtension(p) == ".j2k" ||
                        System.IO.Path.GetExtension(p) == ".tif");
            }

            if (Directory.Exists(rollPaths.GetIdxPath()))
            {
                args.IndexingImages = Directory.GetFiles(rollPaths.GetIdxPath()).Count(
                    p => System.IO.Path.GetExtension(p) == ".jpg" ||
                        System.IO.Path.GetExtension(p) == ".j2k" ||
                        System.IO.Path.GetExtension(p) == ".tif");
            }
            
            e.Result = args;
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var args = (JobSpecsControlArgument)e.Result;
            rootImagesLbl.Content = args.RootImages;
            imgProcImagesLbl.Content = args.ProcessedImages;
            idxImagesLbl.Content = args.IndexingImages;
        }

        

        #endregion

    }
}
