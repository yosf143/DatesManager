using System.Windows;
using System.Windows.Input;

namespace DatesMgr
{
    // I'm using WebView2 for viewing the results window to allow saving the output to pdf, print, direct convert to PDF didnt work with arabic encoding, will try to fix it later
    public partial class ResultWindow : Window
    {
        private string fileName;

        public ResultWindow(string fileName)
        {
            InitializeComponent();
            this.fileName = fileName;

           // Calling the HTML file from app directory to display in result window
            webView.Source = new Uri($"file://{System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName)}");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
    
            if (e.Key == Key.Escape)
            {
                
                Close();
            }

            base.OnKeyDown(e);
        }
    }
}
