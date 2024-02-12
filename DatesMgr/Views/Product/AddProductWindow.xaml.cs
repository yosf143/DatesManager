using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DM.Class; // this will import Product class
using Newtonsoft.Json;

namespace DatesMgr
{
    public partial class AddProductWindow : Window
    {
        private readonly ObservableCollection<Product> products; // Collection to store products

        public AddProductWindow(ObservableCollection<Product> products)
        {
            InitializeComponent();
            this.products = products;
        }

        // shortcut key to open window
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close(); //
            }
            base.OnKeyDown(e);
        }

        // function for click button when adding a new product
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Checking for empty or null inputs, if empty it will show error
            if (string.IsNullOrWhiteSpace(txtProductName.Text) || !dpExpireDate.SelectedDate.HasValue)
            {
                MessageBox.Show("يرجى تعبئة الحقول المطلوبة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Creating a new product object with user inputs
            var product = new Product
            {
                ProductName = txtProductName.Text,
                ExpireDate = dpExpireDate.SelectedDate.Value.Date,
                Unit = (cmbUnit.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                Qty = string.IsNullOrWhiteSpace(txtQty.Text) ? (int?)null : int.Parse(txtQty.Text),
                DiscountPrice = string.IsNullOrWhiteSpace(txtDiscountPrice.Text) ? null : (decimal?)decimal.Parse(txtDiscountPrice.Text),
                Notes = txtNotes.Text
            };

            // Adding the new product to the collection
            products.Add(product);

            // Calculating remaining days till expiration
            product.RemainingDays = (int)(product.ExpireDate - DateTime.Today).TotalDays;
            SaveProductsToJson(products);
            this.Close();
        }

        // Saving the list of products to a JSON file
        private void SaveProductsToJson(ObservableCollection<Product> products)
        {
            var json = JsonConvert.SerializeObject(products, Newtonsoft.Json.Formatting.Indented);

           
            File.WriteAllText("products.json", json);
        }
    }
}
