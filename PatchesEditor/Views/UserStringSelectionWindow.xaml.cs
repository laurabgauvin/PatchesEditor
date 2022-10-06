using MaterialDesignThemes.Wpf;
using PatchesEditor.Data;
using PatchesEditor.Helpers;
using PatchesEditor.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using Section = PatchesEditor.Data.Section;

namespace PatchesEditor.Views
{
    /// <summary>
    /// Interaction logic for UserStringSelectionWindow.xaml
    /// </summary>
    public partial class UserStringSelectionWindow : Window
    {
        private UserStringSelectionWindowViewModel viewModel;

        /// <summary>
        /// Selected value
        /// </summary>
        public string SelectedString { get; set; }

        /// <summary>
        /// Constructor for User String Selection Window
        /// </summary>
        /// <param name="programName">Name of the program for the window title.</param>
        /// <param name="section">What kind of value is the user selecting. For the window title.</param>
        /// <param name="strings">List of strings to choose from</param>
        public UserStringSelectionWindow(string programName, Section section, List<string> strings)
        {
            InitializeComponent();
            viewModel = new UserStringSelectionWindowViewModel(programName, section, strings);
            DataContext = viewModel;
        }

        /// <summary>
        /// Add a new custom value to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void userStringSelectionDialogHost_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false))
                return;

            if (string.IsNullOrWhiteSpace(newStringTextBox.Text) == false)
                viewModel.AllStrings.Add(newStringTextBox.Text.Trim());
        }

        /// <summary>
        /// OK Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(viewModel.SelectedString))
            {
                MessageBox.Show("Please select a value.");
                return;
            }

            SelectedString = viewModel.SelectedString;
            LogManager.WriteToLog(LogLevel.Everything, $"User selected: '{SelectedString}'.");
            GetWindow(this).DialogResult = true;
            GetWindow(this).Close();
        }

        /// <summary>
        /// Cancel and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            LogManager.WriteToLog(LogLevel.Everything, "User clicked Cancel.");
            SelectedString = string.Empty;
            GetWindow(this).DialogResult = false;
            GetWindow(this).Close();
        }
    }
}
