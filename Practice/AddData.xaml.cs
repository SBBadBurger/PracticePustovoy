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
using System.Windows.Shapes;


namespace Practice
{
    /// <summary>
    /// Логика взаимодействия для AddData.xaml
    /// </summary>
    public partial class AddData : Window
    {
        private payments _payments = new payments();
        users user;
        public AddData(users user)
        {
            InitializeComponent();
            this.user = user;
            DataContext = _payments;
            comboCategor.ItemsSource = Instances.db.categories.ToList();
        }

        private void Cancle(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void addData(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (comboCategor.SelectedItem == null)
                errors.AppendLine("Выберите категорию.");
            if (string.IsNullOrWhiteSpace(purOfPay.Text))
                errors.AppendLine("Введите назначение платежа.");
            if (qual.Text.Length < 0)
                errors.AppendLine("Укажите количество.");
            if (Convert.ToInt32(qual.Text) <= 0)
                errors.AppendLine("Количество должно быть больше нуля.");
            if (price.Text.Length < 0)
                errors.AppendLine("Укажите цену.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
            }
            else
            {
                products pr = new products { product_name = purOfPay.Text, FK_category_id = comboCategor.SelectedIndex, price = Convert.ToDecimal(price.Text) };
                Instances.db.products.Add(pr);
                Instances.db.SaveChanges();
                payments pd = new payments
                {
                    payment_key = comboCategor.SelectedItem.ToString().Substring(0, 1) + (Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id).Count() + 1).ToString() + DateTime.Now.Date.ToString(),
                    count = Convert.ToInt32(qual.Text),
                    sum = Convert.ToInt32(qual.Text) * Convert.ToInt32(price.Text),
                    FK_product_id = Instances.db.products.Count()+1,
                    FK_user_id = user.PK_users_id ,
                    date = DateTime.Now.Date,
                };
                
                Instances.db.payments.Add(pd);
            }

            
                Instances.db.SaveChanges();
                MessageBox.Show("Платёж добавлен.");
            


            
        }
    }
}
