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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Microsoft.Win32;
using gomoku.ViewModels;
namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public int x { get; set; } //Tọa độ dòng
        public int y { get; set; } //Tọa độ cột
        MainWindowViewModel ViewModel;
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();

        }
        int cot, hang;
        bool check = false;
        string name;
        Rectangle rect;
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e, int cot, int hang)
        {
            Point point = e.GetPosition((IInputElement) sender);
            cot = ((int)point.X / (int)(Board.ActualHeight / 12));
            hang = ((int)point.Y / (int)(Board.ActualHeight / 12));
            string ketqua = "Bạn đã chọn vào hàng " + " " + hang.ToString() + " " + "cột " + " " +cot.ToString();
            MessageBox.Show(ketqua, "Thông báo",MessageBoxButton.OK);
            //DialogResult DR = MessageBox.Show(ketqua, "Thông báo", MessageBoxButton.OK);
   
            check = true;
            //double top, left;
            //top = cot * (Board.ActualWidth / 12);
            //left = hang * (Board.ActualWidth / 12);
            //Ellipse elip = new Ellipse();
            //elip.Width = elip.Height = Board.ActualWidth / 12;
            //elip.SetValue(Canvas.TopProperty, top);
            //elip.SetValue(Canvas.LeftProperty, left);
            //elip.Fill = Brushes.Red;
            //Board.Children.Add(elip);
            
        }
        private void btnchange_Click(object sender, RoutedEventArgs e)
        {
            name = txtname.Text;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string message = "";
            message += name;
            message += "\n";
            message += txtmessage.Text;
            message += "\n";
            message += DateTime.Now.ToString();
            message += "\n";
            message += "..........................................................................................................";
            lbmessage.Items.Add(message);
            txtmessage.Text = "";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
        }

        private void Board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Board.Children.Clear();
            double wi = Board.ActualHeight / 12;
            for (int x=0;x<12;x++)
            {
                for (int y=0;y<12;y++)
                {
                    double top, left;
                    top = y * wi;
                    left = x * wi;
                    Rectangle rect = new Rectangle();
                    rect.Width = rect.Height = wi;
                    rect.SetValue(Canvas.TopProperty, top);
                    rect.SetValue(Canvas.LeftProperty, left);
                    rect.Fill = Brushes.White;
                    if ((x+y)%2==0)
                    {
                        rect.Fill = Brushes.Gray;
                    }
                    Board.Children.Add(rect);
                }
            }
        }

       
    }
}
