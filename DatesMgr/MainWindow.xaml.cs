using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DM.Class;
using Newtonsoft.Json;
using System.Windows.Input;

namespace DatesMgr
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Product> Products { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Products = LoadProductsFromJson();
            datagridProducts.ItemsSource = Products;
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => ShowAlertMessageOnStartup());
            });

            if (Properties.Settings.Default.AutomaticDeleteExpired)
            {
                AutomaticDeleteExpiredProducts();
                RefreshDataGrid();
            }
        }

        // This will check for outdated products, if user has the setting Auto Delete button on it will delete them from database file automatically
        private void AutomaticDeleteExpiredProducts()
        {
            try
            {
               
                DateTime currentDate = DateTime.Today;

               
                var expiredProducts = Products.Where(p => p.ExpireDate.Date <= currentDate).ToList();

                foreach (var expiredProduct in expiredProducts)
                {
                    Products.Remove(expiredProduct);
                }

                
                SaveProductsToJson(Products);

               
                RefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدثت مشكلة في الحذف التلفائي:  {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterProductsByName();
        }
        

    // Search function, it will filter the products in datagrid with typed one if it exists.
        private void FilterProductsByName()
        {
            string searchText = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredProducts = Products.Where(p => p.ProductName.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                datagridProducts.ItemsSource = new ObservableCollection<Product>(filteredProducts);
            }
            else
            {

                datagridProducts.ItemsSource = Products;
            }
        }



        // Loading Products from json file to the datagrid 
        private ObservableCollection<Product> LoadProductsFromJson()
        {
            try
            {
                if (File.Exists("products.json"))
                {
                    var json = File.ReadAllText("products.json");
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
                MessageBox.Show($"حدثت مشكلة في عرض قاعدة البيانات: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return new ObservableCollection<Product>();
        }

       
        private void datagridProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
        }
        // Delete selected product
        private void DeleteSelectedProducts_Click(object sender, RoutedEventArgs e)
        {
            if (datagridProducts.SelectedItems.Count > 0)
            {
                var selectedProducts = datagridProducts.SelectedItems.Cast<Product>().ToList();

                MessageBoxResult result = MessageBox.Show($"سيتم حذف المنتجات المحددة. هل تريد المتابعة؟",
                                                          "تنبيه", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (var selectedProduct in selectedProducts)
                    {
                        Products.Remove(selectedProduct);
                    }
                    SaveProductsToJson(Products);
                }
            }
            else
            {
                MessageBox.Show("الرجاء تحديد منتجات للحذف.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Delete all products in the json database
        private void DeleteAllProducts_Click(object sender, RoutedEventArgs e)
        {
            if (Products.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("سيتم حذف جميع المنتجات. هل تريد المتابعة؟",
                                                          "تنبيه", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Products.Clear();
                    SaveProductsToJson(Products);
                }
            }
            else
            {
                MessageBox.Show("لا توجد منتجات للحذف.", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        
        private void SaveProductsToJson(ObservableCollection<Product> products)
        {
            var json = JsonConvert.SerializeObject(products, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText("products.json", json);
        }


        // Checking for exp dates in the database depending on selected periods (90, 60, 30) if found it will display on startup
        private void ShowAlertMessageOnStartup()
        {
            
            bool alertBefore90Days = Properties.Settings.Default.AlertBefore90Days;
            bool alertBefore60Days = Properties.Settings.Default.AlertBefore60Days;
            bool alertBefore30Days = Properties.Settings.Default.AlertBefore30Days;

            int productsBefore90Days = alertBefore90Days ? GetProductsBeforeAlertDays(90) : 0;
            int productsBefore60Days = alertBefore60Days ? GetProductsBeforeAlertDays(60) : 0;
            int productsBefore30Days = alertBefore30Days ? GetProductsBeforeAlertDays(30) : 0;

            if (productsBefore90Days > 0 || productsBefore60Days > 0 || productsBefore30Days > 0)
            {
                var products90Days = GetProductsInAlertPeriod(90);
                var products60Days = GetProductsInAlertPeriod(60);
                var products30Days = GetProductsInAlertPeriod(30);

                var alertMessageWindow = new AlertMessageWindow(products90Days, products60Days, products30Days);
                alertMessageWindow.ShowDialog();
            }
        }
        private ObservableCollection<Product> GetProductsInAlertPeriod(int alertDays)
        {
            
            return new ObservableCollection<Product>(Products
                .Where(p => (p.ExpireDate - DateTime.Today).TotalDays <= alertDays && (p.ExpireDate - DateTime.Today).TotalDays > 0)
                .Where(p =>
                {
                    if (alertDays == 90)
                        return Properties.Settings.Default.AlertBefore90Days;
                    else if (alertDays == 60)
                        return Properties.Settings.Default.AlertBefore60Days;
                    else if (alertDays == 30)
                        return Properties.Settings.Default.AlertBefore30Days;

                    return false;
                }));
        }


        private int GetProductsBeforeAlertDays(int alertDays)
        {
            
            return Products.Count(p => (p.ExpireDate - DateTime.Today).TotalDays <= alertDays && (p.ExpireDate - DateTime.Today).TotalDays > 0);
        }

        private void RefreshDataGrid()
        {
           
            datagridProducts.Items.Refresh();

          
            foreach (var product in Products)
            {
                product.RemainingDays = (int)(product.ExpireDate - DateTime.Today).TotalDays;
            }
        }


        // calling edit product window when clicked
        private void EditProductMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (datagridProducts.SelectedItem != null)
            {
                var selectedProduct = (Product)datagridProducts.SelectedItem;
                var editProductWindow = new EditProductWindow(selectedProduct);

               
                if (editProductWindow.ShowDialog() == true)
                {
                   
                    SaveProductsToJson(Products);

                   
                    RefreshDataGrid();
                }
            }
        }


        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                
                AddProductMenuItem_Click(sender, e);
            }
            else if (e.Key == Key.F2)
            {
            
                ViewAlertMessageWindowMenuItem_Click(sender, e);
            }
            else if (e.Key == Key.F3)
            {

                SettingsMenuItem_Click(sender, e);
            }
        }


        private void ViewAlertMessageWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {

            ShowAlertMessageOnStartup();
        }
        private void ShowAboutMe(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Made with ♥ by Yousef Tbakhi\nEmail: contact@s-yousef.com", "About DatesMgr | Version 2.0", MessageBoxButton.OK);
        }



        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }


        private void AddProductMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow(Products);
            addProductWindow.ShowDialog();
        }
    }
}
