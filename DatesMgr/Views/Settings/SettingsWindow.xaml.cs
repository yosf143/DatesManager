using System.IO;
using System.Windows;
using IWshRuntimeLibrary;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using DM.Class;
using System.Windows.Input;

namespace DatesMgr
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            
            chkEnableDesktopShortcut.IsChecked = Properties.Settings.Default.EnableDesktopShortcut;
            chkAutomaticDeleteExpired.IsChecked = Properties.Settings.Default.AutomaticDeleteExpired;
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
           
            Properties.Settings.Default.EnableDesktopShortcut = chkEnableDesktopShortcut.IsChecked ?? false;
            Properties.Settings.Default.AutomaticDeleteExpired = chkAutomaticDeleteExpired.IsChecked ?? false;
            Properties.Settings.Default.Save();

            
            if (chkEnableDesktopShortcut.IsChecked == true)
            {
                CreateDesktopShortcut();
            }
            else
            {
             
                DeleteDesktopShortcut();
            }

            if (chkAutomaticDeleteExpired.IsChecked == true)
            {
                AutomaticDeleteExpiredProducts();
            }


            DialogResult = true;
        }



        private void CreateDesktopShortcut()
        {
            try
            {
                
                string executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

               
                var wshShell = new WshShell();

                
                IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "DatesMgr.lnk"));

                
                shortcut.TargetPath = executablePath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(executablePath);
                shortcut.Description = "إدارة التواريخ";
                shortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدثت مشكلة أثناء إنشاء أيقونة سطح المكتب: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
           
            if (e.Key == Key.Escape)
            {
                
                Close();
            }

            base.OnKeyDown(e);
        }
        private void DeleteDesktopShortcut()
        {
            try
            {
               
                string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "DatesMgr.lnk");
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدثت مشكلة أثناء حذف أيقونة سطح المتكب: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void OpenAlertSettings_Click(object sender, RoutedEventArgs e)
        {
            
            var alertSettingsWindow = new AlertSettingsWindow();
            alertSettingsWindow.ShowDialog();
        }

        private void AutomaticDeleteExpiredProducts()
        {
            try
            {
                
                ObservableCollection<Product> products = LoadProductsFromJson();

               
                DateTime currentDate = DateTime.Today;

          
                var expiredProducts = products.Where(p => p.ExpireDate.Date == currentDate).ToList();

                foreach (var expiredProduct in expiredProducts)
                {
                    products.Remove(expiredProduct);
                }

                
                SaveProductsToJson(products);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدثت مشكلة أثناء الحذف التلقائي: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ObservableCollection<Product> LoadProductsFromJson()
        {
            try
            {
                if (System.IO.File.Exists("products.json"))
                {
                    var json = System.IO.File.ReadAllText("products.json");
                    var products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);

                    
                    foreach (var product in products)
                    {
                        product.RemainingDays = (int)(product.ExpireDate - DateTime.Today).TotalDays;
                    }

                    return products;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدثت مشكلة في عرض قاعد البيانات: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return new ObservableCollection<Product>();
        }

        private void SaveProductsToJson(ObservableCollection<Product> products)
        {
            var json = JsonConvert.SerializeObject(products, Newtonsoft.Json.Formatting.Indented);

            System.IO.File.WriteAllText("products.json", json);
        }
    }
}
