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
using System.ComponentModel;
using System.Threading;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System.Windows.Threading;
using System.Configuration;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow:Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        bool kiemtra = false;
        int hang, cot;
        string name;
        Ellipse elip = null;
        bool check = false;
        int[,] banco = new int[12, 12];
        int dem = 0;
        bool trung = false;
        int type;
        int hang1, cot1;
        bool turn;
        string _name;
        int _row, _col;
        string _mes;
        int player;
        int luot;
        Quobject.SocketIoClientDotNet.Client.Socket socket;
        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += timvitridanh;
            worker.RunWorkerCompleted += maydanh;
        }
        void InvokeOrExecute(Action action)
        {
            var d = Application.Current.Dispatcher;
            if (d.CheckAccess())
                action();
            else
                d.BeginInvoke(DispatcherPriority.Normal, action);
        }
        void KetNoi(string name)
        {
            //Việc cần làm: Kết nối tới server, thành công đăng kí danh tính MyNameIs, và ConnectToOtherPlayer
            //Đồng thời khởi tạo cái socket.On() để bắt tương ứng các nội dung
            //Bắt ChatMessage
            //Bắt EndGame
            //Bắt NextStepIs
            //Thất bại(Không kết tới server thành công) xuất thông báo và return
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                MessageBox.Show("Connected", "Thông báo", MessageBoxButton.OK);
                //lbmessage.Items.Add("Connected");
            });
            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                MessageBox.Show(data.ToString());
                //lbmessage.Items.Add(data.ToString());
            });

            socket.On("ChatMessage", (data) =>
            {
                string s = "You are the first player!";
                turn = data.ToString().Contains(s);
                if(type==4)
                {
                    if (turn == true)
                    {
                        InvokeOrExecute(() => Guitoado(1, 1));
                    }
                }
                MessageBox.Show(data.ToString());
                //lbmessage.Items.Add(data.ToString());
                if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                {
                    socket.Emit("MyNameIs", name);
                    socket.Emit("ConnectToOtherPlayer");
                    _name = name;
                    
                }
                else
                {
                    if (((Newtonsoft.Json.Linq.JObject)data)["from"] == null)
                    {
                        _name = "Server";
                    }
                    else
                    {
                        _name = ((Newtonsoft.Json.Linq.JObject)data)["from"].ToString();
                    }
                    _mes = ((Newtonsoft.Json.Linq.JObject)data)["message"].ToString();
                    
                    InvokeOrExecute(() => addMess(_name, _mes));
                }
            });
            socket.On("NextStepIs", (data) =>
            {
                var a = JObject.Parse(data.ToString());
                player = (int)a["player"];
                _row = (int)a["row"];
                _col = (int)a["col"];
                if (type == 3)
                {
                    InvokeOrExecute((Action)(() => velenbanco(_row, _col, player + 1)));
                }
                else
                {
                    InvokeOrExecute((Action)(() => velenbanco(_row, _col, player + 1)));
                    if (player == 1 )
                    {
                        InvokeOrExecute((Action)(() => timvitridanh2(_row, _col)));
                    }
                }
            });
            socket.On("EndGame", (data) =>
            {
                MessageBox.Show(data.ToString());
                //lbmessage.Items.Add(data.ToString());
                //ở đây có 1 câu cần đưa vào khung chat nak
            });

        }
        void Guitoado(int hang,int cot)
        {
            socket.Emit("MyStepIs", JObject.FromObject(new { row = hang, col = cot }));
        }
        void GoiTinNhan(string mes, string name)
        {
            socket.Emit("ChatMessage", mes);
            socket.Emit("message:" + mes, "from:" + name);
        } 
        void DoiTen(string name)
        {
            socket.Emit("MyNameIs", name);
            socket.Emit("message:", _name + "is now called" + name);
            _name = name;
        }

        void velenbanco(int hang, int cot, int player)
        {
            
                    if (player == 1)
                    {
                        banco[hang, cot] = 1;
                        drawelip(hang, cot, Brushes.Red);
                    }
                    else
                    {
                        banco[hang, cot] = 2;
                        drawelip(hang, cot, Brushes.Blue);
                    }
           
        }

 
        private void drawelip(int hang,int cot, Brush a)
        {
            double top, left;
            top = hang * (Board.ActualWidth / 12);
            left = cot * (Board.ActualHeight / 12);
            elip = new Ellipse();
            elip.Width = elip.Height = Board.ActualWidth / 12;
            elip.SetValue(Canvas.TopProperty, top);
            elip.SetValue(Canvas.LeftProperty, left);
            elip.Fill = a;
            Board.Children.Add(elip);
        }
        
        bool kiemtraphuongngang(int hang, int cot)
        {
            while (cot<11&&banco[hang, cot] == banco[hang, cot + 1])
            {
                dem++;
                cot++;
            };
            cot=cot-dem;
            while (cot>0&&banco[hang, cot] == banco[hang, cot - 1])
                {
                    dem++;
                    cot--;
                };
            if(dem>=4)
            {
                return true;
            }
            else
            {
                dem = 0;
                return false;
            }
        }
        bool kiemtrahangdoc(int hang, int cot)
        {
            while (hang < 11 && banco[hang, cot] == banco[hang+1, cot])
            {
                dem++;
                hang++;
            };
            hang = hang - dem;
            while (hang > 0 && banco[hang, cot] == banco[hang-1, cot])
            {
                dem++;
                hang--;
            };
            if (dem >= 4)
            {
                return true;
            }
            else
            {
                dem = 0;
                return false;
            }
        }
        bool kiemtratheoduongcheothunhat(int hang, int cot)
        {
            while(hang<11&&cot<11&&banco[hang,cot]==banco[hang+1,cot+1])
            {
                dem++;
                hang++;
                cot++;
            }
            hang = hang - dem;
            cot = cot - dem;
            while(hang>0&&cot>0&&banco[hang,cot]==banco[hang-1,cot-1])
            {
                dem++;
                hang--;
                cot--;
            }
            if (dem >= 4)
            {
                return true;
            }
            else
            {
                dem = 0;
                return false;
            }
        }
        bool kiemtratheoduongcheothuhai(int hang, int cot)
        {
            while (hang < 11 && cot >0 && banco[hang, cot] == banco[hang + 1, cot - 1])
            {
                dem++;
                hang++;
                cot--;
            }
            hang = hang - dem;
            cot = cot + dem;
            while (hang > 0 && cot < 11 && banco[hang, cot] == banco[hang - 1, cot +1 ])
            {
                dem++;
                hang--;
                cot++;
            }
            if (dem >= 4)
            {
                return true;
            }
            else
            {
                dem = 0;
                return false;
            }
        }
        int hangAI, cotAI;
        private void maydanh(object sender, RunWorkerCompletedEventArgs e)
        {
            drawelip(hangAI, cotAI, Brushes.Blue);
            banco[hangAI, cotAI] = 2;
            kiemtra = false;
        }
        private void timvitridanh(object sender, DoWorkEventArgs e)
        {               
            int i, j;
            if (cot > 0&&cot<11 && hang > 0&&hang<11)
            {
                for (i = hang - 1; i <= hang + 1; i++)
                {
                    for (j = cot - 1; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }

                    }

                }
            }
            else if(cot==0 && hang==0) 
            {
                for (i = hang ; i <= hang + 1; i++)
                {
                    for (j = cot; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                           
                    }
                }
            }
            else if(cot==0&&hang==11)
            {
                for (i = hang-1; i <= hang; i++)
                {
                    for (j = cot; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                            
                    }

                }
            }
            else if(cot==11&&hang==0)
            {
                for (i = hang; i <= hang + 1; i++)
                {
                    for (j = cot-1; j <= cot; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }  
                    }
                   
                }
            }
            else if(cot==11&&hang==11)
            {
                for (i = hang-1; i <= hang; i++)
                {
                    for (j = cot - 1; j <= cot; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                        
                    }
                }
            }
            else if(cot==0&&hang>0&&hang<11)
            {
                for (i = hang - 1; i <= hang+1; i++)
                {
                    for (j = cot; j <= cot+1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                    }
                }
            }
            else if(cot==11&&hang>0&&hang<11)
            {
                for (j = cot - 1; j <= cot; j++)
                {
                    for (i = hang - 1; i <= hang + 1; i++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                    }
                }
            }
            else if(hang==0&&cot>0&&cot<11)
            {
                for (i = hang; i <= hang + 1; i++)
                {
                    for (j = cot - 1; j <= cot+1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                            
                    }
                }
            }
            else if(hang==11&&cot>0&&cot<11)
            {
                for (i = hang-1; i <= hang; i++)
                {
                    for (j = cot - 1; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                            
                    }
                }
            }
            
           
        }

        private void timvitridanh2(int hang, int cot)
        {
            int i, j;
            if (cot > 0 && cot < 11 && hang > 0 && hang < 11)
            {
                for (i = hang - 1; i <= hang + 1; i++)
                {
                    for (j = cot - 1; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }

                    }

                }
            }
            else if (cot == 0 && hang == 0)
            {
                for (i = hang; i <= hang + 1; i++)
                {
                    for (j = cot; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }

                    }
                }
            }
            else if (cot == 0 && hang == 11)
            {
                for (i = hang - 1; i <= hang; i++)
                {
                    for (j = cot; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }

                    }

                }
            }
            else if (cot == 11 && hang == 0)
            {
                for (i = hang; i <= hang + 1; i++)
                {
                    for (j = cot - 1; j <= cot; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                    }

                }
            }
            else if (cot == 11 && hang == 11)
            {
                for (i = hang - 1; i <= hang; i++)
                {
                    for (j = cot - 1; j <= cot; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }

                    }
                }
            }
            else if (cot == 0 && hang > 0 && hang < 11)
            {
                for (i = hang - 1; i <= hang + 1; i++)
                {
                    for (j = cot; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                    }
                }
            }
            else if (cot == 11 && hang > 0 && hang < 11)
            {
                for (j = cot - 1; j <= cot; j++)
                {
                    for (i = hang - 1; i <= hang + 1; i++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }
                    }
                }
            }
            else if (hang == 0 && cot > 0 && cot < 11)
            {
                for (i = hang; i <= hang + 1; i++)
                {
                    for (j = cot - 1; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }

                    }
                }
            }
            else if (hang == 11 && cot > 0 && cot < 11)
            {
                for (i = hang - 1; i <= hang; i++)
                {
                    for (j = cot - 1; j <= cot + 1; j++)
                    {
                        if (banco[i, j] == 0)
                        {
                            hangAI = i;
                            cotAI = j;
                            break;
                        }

                    }
                }
            }
            if(type==4)
            {
                Guitoado(hangAI, cotAI);
            }
        }
       
        private void ketqua2nguoichoi(int hang, int cot)
        {
            string songchoi = banco[hang, cot].ToString();
            if (kiemtraphuongngang(hang, cot) == true)
            {
                MessageBox.Show("người chơi thứ "+ songchoi +" đã chiến thắng");
                System.Environment.Exit(0);
            }
            else if (kiemtrahangdoc(hang, cot) == true)
            {
                MessageBox.Show("người chơi thứ " + songchoi + " đã chiến thắng");
                System.Environment.Exit(0);
            }
            else if (kiemtratheoduongcheothunhat(hang, cot) == true)
            {
                MessageBox.Show("người chơi thứ " + songchoi + " đã chiến thắng");
                System.Environment.Exit(0);
            }
            else if (kiemtratheoduongcheothuhai(hang, cot) == true)
            {
                MessageBox.Show("người chơi thứ " + songchoi + " đã chiến thắng");
                System.Environment.Exit(0);
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (kiemtra == true)
            {
                MessageBox.Show("máy đang suy ngĩ, bạn chờ tí đã!!!");
                return;
            }
            Point point = e.GetPosition((IInputElement) sender);
            hang = ((int)point.Y / (int)(Board.ActualHeight / 12));
            cot = ((int)point.X / (int)(Board.ActualHeight / 12));


            string ketqua = "Bạn đã chọn vào hàng " + " " + hang.ToString() + " " + "cột " + " " + cot.ToString();
            if(banco[hang,cot]==0)
            {
                trung = false;
            }
            else
            {
                trung = true;
                MessageBox.Show("vị trí bạn chọn đã bị trùng");
                return;
            }
           if(type==1)
           {
               if (check == false && trung == false)
               {
                   banco[hang, cot] = 1;
                   drawelip(hang, cot, Brushes.Red);
                   ketqua2nguoichoi(hang, cot);
                   check = true;
               }
               else if (check == true && trung == false)
               {
                   banco[hang, cot] = 2;
                   drawelip(hang, cot, Brushes.Blue);
                   ketqua2nguoichoi(hang, cot);
                   check = false;
               }
           }
           else if(type==2)
           {
               if(trung==false)
               {
                   if (kiemtra==false)
                   {
                       banco[hang, cot] = 1;
                       drawelip(hang, cot, Brushes.Red);
                       ketqua2nguoichoi(hang, cot);
                       //Người đã đánh xong
                       kiemtra = true;
                       worker.RunWorkerAsync();
                       ketqua2nguoichoi(hang, cot);                       
                   }
                 
               }  
           
           }
           else if(type==3)
           {
               Guitoado(hang, cot);
           }
        }

        private void btnchange_Click(object sender, RoutedEventArgs e)
        {
            name = txtname.Text;
            switch(type)
            {
                case 1:
                case 2:
                    break;
                case 3:
                case 4:
                     DoiTen(name);
                    break;
            }
        }
        private void addMess(string _name, string _mess)
        {
            string message = "";
            message += _name;
            message += "\n";
            message += _mess;
            message += "\n";
            message += DateTime.Now.ToString();
            message += "\n";
            message += "..........................................................................................................";
            lbmessage.Items.Add(message);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                default:
                    
                case 1:
                case 2:
                    string message = "";
                    message += txtname.Text;
                    message += "\n";
                    message += txtmessage.Text;
                    message += "\n";
                    message += DateTime.Now.ToString();
                    message += "\n";
                    message += "..........................................................................................................";
                    lbmessage.Items.Add(message);
                    break;
                case 3:
                    string message1 = "";
                    message1 += name;
                    message1 += "\n";
                    message1 += txtmessage.Text;
                    message1 += "\n";
                    message1 += DateTime.Now.ToString();
                    message1 += "\n";
                    message1 += "..........................................................................................................";
                   // lbmessage.Items.Add(message1);
                    GoiTinNhan(txtmessage.Text, name);
                    break;
                case 4:
                    string message2 = "";
                    message2 += name;
                    message2 += "\n";
                    message2 += txtmessage.Text;
                    message2 += "\n";
                    message2 += DateTime.Now.ToString();
                    message2 += "\n";
                    message2 += "..........................................................................................................";
                   //lbmessage.Items.Add(message2);
                    GoiTinNhan(txtmessage.Text, name);
                    break;
            }     
            txtmessage.Text = "";
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
            for(int i=0;i<12;i++)
            {
                for(int j=0;j<12;j++)
                {
                    if(banco[i,j]==1)
                    {
                        drawelip(i, j, Brushes.Red);
                    }
                    else if(banco[i,j]==2)
                    {
                        drawelip(i, j, Brushes.Blue);
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            type = 1;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            type = 2;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
           
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            type = 3;
            string server = ConfigurationManager.AppSettings["Server"];
            socket = IO.Socket(server);
            //socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
            KetNoi(txtname.Text);
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            type = 4;
            string server = ConfigurationManager.AppSettings["Server"];
            socket = IO.Socket(server);
            //socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
            KetNoi(txtname.Text);
        }
    }
}
