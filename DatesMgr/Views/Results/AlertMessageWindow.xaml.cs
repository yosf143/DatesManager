using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text;
using DM.Class; // this will import Product class




namespace DatesMgr
{
    public partial class AlertMessageWindow : Window
    {
        private string currentFileName; // Store the current file name for alert message window

        // Constructor for alert message window
        public AlertMessageWindow(ObservableCollection<Product> products90Days, ObservableCollection<Product> products60Days, ObservableCollection<Product> products30Days)
        {
            InitializeComponent();

            // this will filter products based on expiration days
            var filteredProducts60Days = new ObservableCollection<Product>(products60Days.Where(p => !products90Days.Contains(p)).ToList());
            var filteredProducts30Days = new ObservableCollection<Product>(products30Days.Where(p => !products90Days.Contains(p) && !filteredProducts60Days.Contains(p)).ToList());

            // this will combine filtered product lists into one for display
            SetDataGridItemsSource(datagridAllProducts, new ObservableCollection<Product>(products90Days.Concat(filteredProducts60Days).Concat(filteredProducts30Days).ToList()));

            // this will generate an html file, i used html to view it in a WebView2 window because i had some issues generating the products list to a PDF because it looks it doesnt support Arabic encoding, i will try to fix it later
            currentFileName = $"ExpDates{DateTime.Now.ToString("yyyy-MM-dd")}.html";

           
            Application.Current.Exit += Current_Exit;
        }


        // shortcut key to open window
        protected override void OnKeyDown(KeyEventArgs e)
        {
         
            if (e.Key == Key.Escape)
            {
                
                Close();
            }

            base.OnKeyDown(e);
        }

        
        private void SaveAsHtml_Click(object sender, RoutedEventArgs e)
        {
            SaveAsHtml(currentFileName);
            OpenResultWindow(currentFileName);
        }

        private void OpenResultWindow(string fileName)
        {
            var resultWindow = new ResultWindow(fileName);
            resultWindow.Show();
        }

        // this will generate the Products List in a webView2 window
        private void SaveAsHtml(string fileName)
        {
            try
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append("<html lang=\"ar\">");
                stringBuilder.Append("<head>");
                stringBuilder.Append("<meta charset=\"UTF-8\">");
                stringBuilder.Append("<style>");
                stringBuilder.Append("body { direction: rtl; user-select: none; }");
                stringBuilder.Append("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
                stringBuilder.Append("th, td { border: 1px solid #dddddd; text-align: right; padding: 8px; }");
                stringBuilder.Append("th { background-color: #f2f2f2; }");
                stringBuilder.Append("h1 { text-align: center; }");
                stringBuilder.Append("</style>");

               
                stringBuilder.Append("<script>");
                stringBuilder.Append("document.addEventListener('DOMContentLoaded', function() {");

              
                stringBuilder.Append("  setTimeout(function() { window.print(); }, 1000);");

                stringBuilder.Append("  window.onbeforeprint = function() {");
                stringBuilder.Append("    var style = document.createElement('style');");
                stringBuilder.Append("    style.type = 'text/css';");
                stringBuilder.Append("    style.innerHTML = '@media print { @page { size: auto; margin: 0mm; } body { margin: 5mm; } }';");
                stringBuilder.Append("    document.head.appendChild(style);");
                stringBuilder.Append("  };");

                stringBuilder.Append("});");
                stringBuilder.Append("</script>");

                stringBuilder.Append("</head>");
                stringBuilder.Append("<body>");

                stringBuilder.Append("<h1>تواريخ قريبة إنتهاء الصلاحية</h1>");

                stringBuilder.Append($"<p>تاريخ الطباعة: {DateTime.Now.ToString("yyyy-MM-dd")}</p>");

                SaveDataGridToHtml(stringBuilder, datagridAllProducts, "");

                stringBuilder.Append("</body>");
                stringBuilder.Append("</html>");

                File.WriteAllText(fileName, stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء حفظ الملف: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // this will generate the products table
        private void SaveDataGridToHtml(StringBuilder stringBuilder, DataGrid dataGrid, string period)
        {
            if (dataGrid.Items.Count > 0)
            {
                stringBuilder.Append($"<h2>{period}</h2>");
                stringBuilder.Append("<table>");

                stringBuilder.Append("<tr>");
                foreach (var column in dataGrid.Columns)
                {
                    stringBuilder.Append($"<th>{((DataGridColumn)column).Header}</th>");
                }
                stringBuilder.Append("</tr>");

                foreach (var item in dataGrid.Items)
                {
                    stringBuilder.Append("<tr>");
                    foreach (var column in dataGrid.Columns)
                    {
                        var header = ((DataGridColumn)column).Header.ToString();
                        var propertyValue = item.GetType().GetProperty(((DataGridColumn)column).SortMemberPath)?.GetValue(item, null);

                        if (header == "تاريخ الإنتهاء" && propertyValue is DateTime date)
                        {
                            stringBuilder.Append($"<td>{date.ToString("yyyy-MM-dd")}</td>");
                        }
                        else
                        {
                            if (header == "متبقي" && propertyValue is int remainingDays)
                            {
                                stringBuilder.Append($"<td>{remainingDays} يوم</td>");
                            }
                            else
                            {
                                stringBuilder.Append($"<td>{propertyValue}</td>");
                            }
                        }
                    }

                    stringBuilder.Append("</tr>");
                }

                stringBuilder.Append("</table>");
            }
        }


        private void SetDataGridItemsSource(DataGrid dataGrid, ObservableCollection<Product> products)
        {
            // Setting the items source of the data grid to the provided collection of products
            dataGrid.ItemsSource = products;

            
            dataGrid.MouseDoubleClick += (sender, e) =>
            {
                var grid = (DataGrid)sender;
                var selectedProduct = (Product)grid.SelectedItem;

                if (selectedProduct != null)
                {
                    // this will remove a product temporarily if user doesnt work to delete it from json file when generating the products list
                    products.Remove(selectedProduct);

                    // Refresh table after changes
                    grid.Items.Refresh();
                }
            };
        }


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
         
            MessageBox.Show("يمكنك إزالة المنتجات بشكل مؤقت بالضغط المزدوج على المنتج", "مساعدة", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void Current_Exit(object sender, ExitEventArgs e)
        {
            
            if (File.Exists(currentFileName))
            {
                File.Delete(currentFileName);
            }
        }
    }
}