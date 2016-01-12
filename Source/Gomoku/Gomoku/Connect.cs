using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System.Windows;

namespace Gomoku
{
    class Connect:MainWindow
    {
        
        public static bool turn;
        public static string _name;
        public static int _row, _col;
        public static string _mes;
        public static void KetNoi(Quobject.SocketIoClientDotNet.Client.Socket socket, string name)
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
            });
            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                    MessageBox.Show(data.ToString());
            });

            socket.On("ChatMessage", (data) =>
            {
                string s = "You are the first player!";
                turn = data.ToString().Contains(s);
                MessageBox.Show(data.ToString());
                if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                {
                    socket.Emit("MyNameIs", name);
                    socket.Emit("ConnectToOtherPlayer");
                    _name = name;
                }
                else if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() != "Welcome!")
                {
                    _mes = data.ToString();
                }
            });
            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                MessageBox.Show(data.ToString());
            });
            socket.On("NextStepIs", (data) =>
            {
                var a = JObject.Parse(data.ToString());

                _row = (int)a["row"];
                _col = (int)a["col"];

            });
            socket.On("EndGame", (data) =>
            {
                MessageBox.Show(data.ToString());
            });

        }
        public static void GuiToaDo(Quobject.SocketIoClientDotNet.Client.Socket socket, int row, int col)
        {
            //Thực hiện gửi vị trí đánh cho server đầu vào là hàng cột,
            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                MessageBox.Show(data.ToString());
            });
            socket.Emit("MyStepIs", JObject.FromObject(new { row = row, col = col }));
        }
        public static void DoiTen(Quobject.SocketIoClientDotNet.Client.Socket socket, string name)
        {
            socket.Emit("MyNameIs", name);
            socket.Emit("message:", _name + "is now called" + name);
            _name = name;
        }
        public static void GoiTinNhan(Quobject.SocketIoClientDotNet.Client.Socket socket, string mes, string name)
        {
            socket.Emit("ChatMessage", mes);
            socket.Emit("message:" + mes, "from:" + name);//dòng này làm gìcó? có mà
        } 
    }
}
