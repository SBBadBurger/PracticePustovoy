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
    /// Логика взаимодействия для Capcha.xaml
    /// </summary>
    public partial class Capcha : Window
    {
        public Capcha()
        {
            InitializeComponent();
            GenerateCapcha();

        }

        private void EnterCapcha_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (enterCapcha.Text.Count() == 10)
            {
                if (enterCapcha.Text == textCapcha.Text)
                {
                    enterCapcha.IsEnabled = false;
                    MainWindow main = new MainWindow();
                    main.Visibility=Visibility.Visible;
                    this.Close();
                }
                else
                {
                    textCapcha.Text = "";
                    enterCapcha.Text = "";
                    GenerateCapcha();
                }
                
            }
            
        }

        //генерация капчи
        private void GenerateCapcha()
        {
            enterCapcha.Text = "";
            textCapcha.Text = "";
            string allowchar = "";
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char[] a = { ',' };
            string[] ar = allowchar.Split(a);
            string pwd = "";
            string temp = "";
            Random r = new Random();

            for (int i = 0; i < 10; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];
                pwd += temp;
            }
            textCapcha.Text = pwd;
        }
    }
}
