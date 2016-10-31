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

namespace USB_Wizard
{
    /// <summary>
    /// passworddlg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class passworddlg : Window
    {
        public passworddlg(string question)
        {
            InitializeComponent();
            lblQuestion.Content = question;
            //MessageBox.Show(Convert.ToString(lblQuestion.Content));

        }
        public void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            String str;
            str = Convert.ToString(txtAnswer.Password);

            MessageBox.Show(str);

        }

        public void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();

        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void btnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
        /*public string Answer
{
   get { return txtAnswer; }
}*/

    }
}
