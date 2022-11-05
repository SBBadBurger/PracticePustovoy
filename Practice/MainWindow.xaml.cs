using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Practice
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string login;
        string pass;
        int tryes;
        int currentTime;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();

        }
        //Обработка нажатия кнопки войти
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            login = loginText.Text;
            pass = ToSha256(loginText.Text);
            ChouseData chouseData;
            //Поиск пользователя в базе данных
            try {
                    var user = Instances.db.users.Where(p => p.login == login && p.password == pass).FirstOrDefault();
                
            if (user != null && loginText.Text == passwordText.Text)
                {
                chouseData = new ChouseData(user);
                chouseData.Show();
                this.Close();
            }
                else
                {
                    //Обработка количества попыток
                    if (tryes < 3)
                    {
                        tryes++;
                        MessageBox.Show("Неверный логин или пароль \n Попыток: "+(3-tryes));
                        Capcha capchaWindow = new Capcha();
                        capchaWindow.Show();
                        this.Visibility=Visibility.Collapsed;
                    }
                    else
                    {
                        //Таймер при максимальном количестве неудачных попыток
                        currentTime = 0;
                        MessageBox.Show("Недопустимое количество попыток. \n Необходимо подождать 30 секунд");
                        this.IsEnabled = false;
                        textLock.Visibility=Visibility.Visible;
                        timer.Tick += new EventHandler(timerTick);
                        timer.Interval = new TimeSpan(0, 0, 1000);
                        timer.Start();
                        tryes = 0;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Такого пользователя не существует");
            }
        }
        // Обработка тика таймера
        private void timerTick(object sender, EventArgs e)
        {
            if (currentTime <= 30)
            {
                currentTime++;
                textLock.Text = "Необходимо подождать: " + currentTime;
            }
            else
            {
                textLock.Visibility = Visibility.Collapsed;
                this.IsEnabled = true;
                timer.Stop();
            }
        }
        //Кодировка в Sha256
        private string ToSha256(string x)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] source = Encoding.UTF8.GetBytes(x);
                byte[] hash = sha256Hash.ComputeHash(source);
                x = BitConverter.ToString(hash).Replace("-", String.Empty);
            }
            return x;
        }
    }
}
