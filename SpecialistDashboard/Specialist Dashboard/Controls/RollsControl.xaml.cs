﻿using System;
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
using System.ComponentModel;

namespace Specialist_Dashboard.Controls
{
    /// <summary>
    /// Interaction logic for RollsControl.xaml
    /// </summary>
    public partial class RollsControl : UserControl
    {
        public TabControl DetailsTabControl { get; set; }
        public List<Roll> Rolls { get; set; }
        public RollsControl()
        {
            InitializeComponent();

            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/VS/Styles.xaml", UriKind.RelativeOrAbsolute);

            DetailsTabControl = new TabControl();
            DetailsTabControl.Resources = resourceDictionary;
            DetailsTabControl.SelectionChanged += DetailsTabControl_SelectionChanged;
        }

        void DetailsTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DetailsTabControl.Items.Count < 1)
            {
                VisibilityChanged(false);
            }
        }

        bool _visibility;
        public void VisibilityChanged(bool visible)
        {
            _visibility = visible;
            if (visible)
            {
                rollTabCtrlCC.Visibility = Visibility.Visible;
                DetailsTabControl.Visibility = Visibility.Visible;
                BackBtn.Visibility = Visibility.Visible;
                ShowDetailsBtn.IsEnabled = false;
            }
            else
            {
                rollTabCtrlCC.Visibility = Visibility.Hidden;
                DetailsTabControl.Visibility = Visibility.Hidden;
                BackBtn.Visibility = Visibility.Hidden;
                if (DetailsTabControl.Items.Count > 0)
                    ShowDetailsBtn.IsEnabled = true;
            }
        }

        private void rollsLv_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (rollsLv.SelectedItems.Count > 0)
            {
                var tab = new TabItem();
                int i = 0;
                bool newTab = true;
                var jobSpecControl = new JobSpecControl(Rolls[rollsLv.SelectedIndex]);
                tab.Content = jobSpecControl;
                tab.Header = Rolls[rollsLv.SelectedIndex].RollName;
                foreach (TabItem t in DetailsTabControl.Items)
                {
                    if (t.Header == tab.Header)
                    {
                        newTab = false;
                        break;
                    }
                    i++;
                }

                if (newTab)
                {
                    DetailsTabControl.Items.Add(tab);
                    DetailsTabControl.SelectedIndex = DetailsTabControl.Items.Count - 1;
                    jobSpecControl.historyLv.SelectedIndex = 0;
                    jobSpecControl.notesLv.SelectedIndex = 0;
                }
                else
                    DetailsTabControl.SelectedIndex = i;

                rollTabCtrlCC.Content = DetailsTabControl;

                VisibilityChanged(true);
            }
        }

        
        private void BackBtn_Click_2(object sender, RoutedEventArgs e)
        {
            VisibilityChanged(false);
        }

        private void ShowDetailsBtn_Click_1(object sender, RoutedEventArgs e)
        {
            VisibilityChanged(true);
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        int _lastDirection = 0;
        private void GridViewColumn_Click_1(object sender, RoutedEventArgs e)
        {
            if (Rolls != null)
            {
                GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
                if (headerClicked != null)
                {
                    if (headerClicked.Column != null)
                    {
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
                                    Rolls.Reverse();
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
            }
        }

        private void SortColumn(string header)
        {
            if (header.ToLower() == "roll name")
            {
                Rolls.Sort();
                ReassignListview();
            }
            else if (header.ToLower() == "current")
            {
                var sortOnStep = new SortOnStep();
                Rolls.Sort(sortOnStep);
                ReassignListview();
            }
            else if (header.ToLower() == "state")
            {
                var sortOnState = new SortOnState();
                Rolls.Sort(sortOnState);
                ReassignListview();
            }
            else if (header.ToLower() == "last update")
            {
                var sortOnDate = new SortOnDate();
                Rolls.Sort(sortOnDate);
                ReassignListview();
            }
        }

        private void ReassignListview()
        {
            rollsLv.ItemsSource = null;
            rollsLv.Items.Clear();
            rollsLv.ItemsSource = Rolls;
        }


        string _step = "";
        string _rollname = "";
        DateTime _min;
        DateTime _max;
        private void RollsControl_KeyDown_1(object sender, KeyEventArgs e)
        {
            // Minimizes the tab control
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                if (DetailsTabControl.SelectedItem != null && _visibility)
                {
                    VisibilityChanged(false);
                }
            }
            // Closes your current tab
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.W))
            {
                if (DetailsTabControl.SelectedItem != null && _visibility)
                {
                    DetailsTabControl.Items.Remove(DetailsTabControl.SelectedItem);
                    VisibilityChanged(false);
                }
            }
            // Reads your current search and saves it
            else if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.R))
            {
                _step = rollStepComboBox.Text;
                _rollname = rollRollnameTxt.Text;

                _min = Convert.ToDateTime(fromDTPick.SelectedDate);
                _max = Convert.ToDateTime(toDTPick.SelectedDate);

                if (_min == null)
                    _min = System.DateTime.Today;
                if (_max == null)
                    _max = System.DateTime.Now;
            }
            // Writes your remembered search
            else if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.W))
            {
                rollStepComboBox.Text = _step;
                rollRollnameTxt.Text = _rollname;
                fromDTPick.SelectedDate = _min;
                toDTPick.SelectedDate = _max;

                if (_min == Convert.ToDateTime("1/1/0001"))
                    fromDTPick.Text = "";

                if (_max == Convert.ToDateTime("1/1/0001"))
                    toDTPick.Text = "";
            }
            // Clears search
            else if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.C))
            {
                rollStepComboBox.Text = "";
                rollRollnameTxt.Text = "";
                fromDTPick.SelectedDate = null;
                toDTPick.SelectedDate = null;
                fromDTPick.Text = "";
                toDTPick.Text = "";
            }
        }
    }
}
