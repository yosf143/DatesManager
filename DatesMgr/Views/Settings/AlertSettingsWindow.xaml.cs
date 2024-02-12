using System.Windows;
using System.Windows.Input;

namespace DatesMgr
{
    public partial class AlertSettingsWindow : Window
    {
        public AlertSettingsWindow()
        {
            InitializeComponent();

            
            chkAlert90Days.IsChecked = Properties.Settings.Default.AlertBefore90Days;
            chkAlert60Days.IsChecked = Properties.Settings.Default.AlertBefore60Days;
            chkAlert30Days.IsChecked = Properties.Settings.Default.AlertBefore30Days;
        }



        protected override void OnKeyDown(KeyEventArgs e)
        {
            
            if (e.Key == Key.Escape)
            {
                
                Close();
            }

            base.OnKeyDown(e);
        }
        
        // Saving new settings 
        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
           
            Properties.Settings.Default.AlertBefore90Days = chkAlert90Days.IsChecked ?? false;
            Properties.Settings.Default.AlertBefore60Days = chkAlert60Days.IsChecked ?? false;
            Properties.Settings.Default.AlertBefore30Days = chkAlert30Days.IsChecked ?? false;

            
            Properties.Settings.Default.Save();
            DialogResult = true;
             
            Close();
        }
    }
}
