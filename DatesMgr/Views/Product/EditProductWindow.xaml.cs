using DM.Class; // this will import Product class
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DatesMgr
{
    public partial class EditProductWindow : Window
    {
        private readonly Product product; // Store the product being edited

        // Constructor for the edit product window
        public EditProductWindow(Product product)
        {
            InitializeComponent();
            this.product = product; // Assigning the product to the private field

            // Setting the initial values of UI elements based on the product data
            txtProductName.Text = product.ProductName;
            dpExpireDate.SelectedDate = product.ExpireDate;

            // Selecting the appropriate unit in the combo box
            cmbUnit.SelectedValue = cmbUnit.Items.OfType<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == product.Unit);

            // Setting the quantity, discount price, and notes
            txtQty.Text = product.Qty.ToString();
            txtDiscountPrice.Text = product.DiscountPrice?.ToString();
            txtNotes.Text = product.Notes;
        }



        protected override void OnKeyDown(KeyEventArgs e)
        {
            
            if (e.Key == Key.Escape)
            {
               
                Close();
            }

            base.OnKeyDown(e);
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Checking for empty or null inputs
            if (string.IsNullOrWhiteSpace(txtProductName.Text) || !dpExpireDate.SelectedDate.HasValue)
            {
                MessageBox.Show("يرجى تعبئة الحقول المطلوبة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Updating product properties with new values from UI elements
            product.ProductName = txtProductName.Text;
            product.ExpireDate = dpExpireDate.SelectedDate.Value.Date;

            // Updating unit with selected value from combo box
            product.Unit = (cmbUnit.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // Parsing quantity input if not empty
            if (!string.IsNullOrWhiteSpace(txtQty.Text))
            {
                product.Qty = int.Parse(txtQty.Text);
            }
            else
            {
                product.Qty = null;
            }

            // Parsing discount price input if not empty
            product.DiscountPrice = string.IsNullOrWhiteSpace(txtDiscountPrice.Text) ? null : (decimal?)decimal.Parse(txtDiscountPrice.Text);
            product.Notes = txtNotes.Text;

            // DialogResult to true to indicate changes were saved
            DialogResult = true;

            // Closing the window
            Close();
        }



    }
}
