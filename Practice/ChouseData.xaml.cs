using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ChouseData.xaml
    /// </summary>
    public partial class ChouseData : Window
    {
        users user;
        public ChouseData(users user)
        {
            InitializeComponent();
            if (user != null) this.user = user;
            datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id).ToList();
        }

        //добавление записи
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddData addData = new AddData(user);
            bool result = (bool)addData.ShowDialog();
            if (!result) datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id).ToList(); 
        }

        //удаление записей
        private void RemData(object sender, RoutedEventArgs e)
        {
            var dataFR = datagride.SelectedItems.Cast<payments>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить следующие {dataFR.Count()} элементов?", "Внимение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Instances.db.payments.RemoveRange(dataFR);
                    Instances.db.SaveChanges();
                    MessageBox.Show("Данные удалены.");

                    datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id).ToList(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

         //выбор начальной даты
        private void DateFrom_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (dateTo.SelectedDate != null)
                datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id && p.date >= dateFrom.SelectedDate && p.date <= dateTo.SelectedDate).ToList();
            else datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id && p.date >= dateFrom.SelectedDate).ToList();
        }

        //выбор конечной даты
        private void DateTo_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (dateFrom.SelectedDate != null)
                datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id && p.date >= dateFrom.SelectedDate && p.date <= dateTo.SelectedDate).ToList();
            else datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id && p.date >= dateTo.SelectedDate).ToList();
        }

        //выбор категории
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxCategory.SelectedItem != null)
            {
                datagride.ItemsSource = Instances.db.payments.Where(p => p.FK_user_id == user.PK_users_id&&p.products.FK_category_id==comboBoxCategory.SelectedIndex).ToList();
            }
        }
        private void MarkColors()
        {

        }

        //очистка списка
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            datagride.Items.Clear();
        }

        //вывод отчёта
        private void ButtonReport_Click(object sender, RoutedEventArgs e)
        {
            if (datagride.Items.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Output.pdf";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("Невозможно сохранить документ: " + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            PdfPTable pdfTable = new PdfPTable(datagride.Columns.Count);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            foreach (DataGridColumn column in datagride.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()));
                                pdfTable.AddCell(cell);
                            }

                            foreach (DataGridRow row in datagride.Items)
                            {

                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(cell.Value.ToString());
                                }
                            }
                           

                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                                PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();
                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                                stream.Close();
                            }

                            MessageBox.Show("Data Exported Successfully !!!", "Info");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }
    }
}
