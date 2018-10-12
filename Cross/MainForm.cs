using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;


namespace Cross
{
    public partial class MainForm : Form
    {
        /*
        Памятка.
            Дорогой друг, помни:
                Ничья определяется через счетчик ходов.
                В конце партии isGameEnded должно быть приравнено у true
                true это крестики
        */

        private int X = 0;
        private int Y = 0;
        private int W = 0;

        private readonly int _serverPort = 33377;
        Connection _connection;
        Thread _connectionThread;
        Drawer drawer;
        TheGame game;

        private readonly int _chatPort = 33388;
        UdpMulticastChat chat;
        Thread chatThread;

        public MainForm()
        {
            InitializeComponent();
            Console.WriteLine(ClientSize);
            chat = new UdpMulticastChat(_chatPort);
            chat.Chatted += DisplayMulticastMessage;
            chatThread = new Thread(new ThreadStart(chat.ReceiveMessages));
            chatThread.Start();
        }

        private void button1_Click(object sender, EventArgs e) //Новая
        {
            FormDialog form = new FormDialog();
            form.ShowDialog();
            if (!form.IsFormCorrect())
            {
                return;
            }
            X = form.GetX();
            Y = form.GetY();
            W = form.GetW();
            drawer = new Drawer(form.GetX(), form.GetY(), CreateGraphics(), CreateGraphics(), ClientSize.Height,ClientSize.Width,chatTextBox.ClientSize.Width);//Экземпляр рисовалки
            game = new TheGame(drawer,form.GetW(), form.GetX(), form.GetY());
            game.SetTurn(true);
            drawer.ClearMap(this.BackColor);
            drawer.DrawMap();
            game.SetIsGameStarted (true);
            game.SetIsGameEnded (false);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            if (drawer != null && game != null)
            {
                drawer.DrawMap();
                drawer.FillMap(game.getPlayGrounds());
                if (game.GetIsGameEnded()) drawer.crossOutWinner();
                UpdateScore();
            }

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (game == null)
            {
                return;
            }
            if (e.Button != MouseButtons.Left) //проверка на левую кнопку
            {
                return;
            }
            if (game.UserTurn(e.X, e.Y) && _connection != null)
            {
                UpdateScore();
                game.WhoIsUser="";
                _connection.SendMove(game.GetXpos(), game.GetYpos());
            }
            
        }

        private void ResetGameAndMap()
        {
            if (game != null && game.GetIsGameStarted())
            {
                drawer.ClearMap(this.BackColor);
                drawer.DrawMap();
                game.ResetGame();
                if (game.getIsOnlineGame())
                {
                    game.ResetUser();
                    game.setYourOnlineTurn(!game.GetYourOnlineTurn());
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (game != null && game.GetIsGameStarted() && game.getIsOnlineGame())
            {
                if (game.GetIsGameEnded())
                {
                    ResetGameAndMap();
                    _connection.SendResetGameEvent();
                    return;
                }
            }
            else if (game != null && game.GetIsGameStarted())
            {
                ResetGameAndMap();
            }
        }

        public void DisplayPrivateMessage(string message)
        {
            chatTextBox.Invoke((MethodInvoker) (() => chatTextBox.AppendText("Соперник: " + message + "\n")));
        }

        public void ShowRequestMessage(string ip)
        {
            DialogResult dialogResult = MessageBox.Show("Принять запрос на игру от " + ip + "?", "Запрос на подключение", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                _connection.SendSettings(X, Y, W);
                game.SetIsGameStarted (true);
                game.SetIsGameEnded(false);
                //playGrounds = new int[X, Y];
                game.SetTurn(true);
                game.setIsOnlineGame (true);
                game.setYourOnlineTurn(false);

            }
            else if (dialogResult == DialogResult.No)
            {
                new Thread(new ThreadStart(() => startServerButton_Click(this, new EventArgs()))).Start();
                //startServerButton_Click(this, new EventArgs());
            }

        }

        public void GotSettingsHandler(int x, int y, int w)
        {
            X = x;
            Y = y;
            W = w;
            drawer = new Drawer(X, Y, CreateGraphics(), CreateGraphics(), ClientSize.Height, ClientSize.Width, chatTextBox.ClientSize.Width);
            game = new TheGame(drawer, w, x, y);
            drawer.ClearMap(this.BackColor);
            drawer.DrawMap();
            game.SetIsGameStarted(true);
            game.SetIsGameEnded(false);
            game.ResetPlayGrounds();
            game.SetTurn(true);
            game.setIsOnlineGame(true);
            game.setYourOnlineTurn(true);
        }

        public void DrawEnemyTurn(int x, int y)
        {
            game.ChangePlayGrounds(x, y);
            drawer.DrawMap();
            drawer.FillMap(game.getPlayGrounds());
            game.SearchWinner();
            game.SetTurn(!game.GetTurn());
        }

        //Сервер
        private void startServerButton_Click(object sender, EventArgs e)
        {
            if (_connection != null)
            {
                _connection.Terminate();
                startServerButton.Invoke((MethodInvoker)(() => startServerButton.Text = "Запустить сервер"));
                if (_connectionThread != null) _connectionThread.Abort();
                _connection = null;
                _connectionThread = null;
                return;
            }
            if (_connection == null && (X == 0 || Y == 0 || W == 0))
            {
                MessageBox.Show("Задайте настройки игры, прежде чем запустить сервер");
                return;
            }
            // запуск сервера
            _connection = new Connection(_serverPort);
            _connection.Chatted += DisplayPrivateMessage;
            _connection.Requested += ShowRequestMessage;
            _connection.Moved += DrawEnemyTurn;
            _connection.ResetSignal += ResetGameAndMap;
            _connectionThread = new Thread(new ThreadStart(_connection.StartServer));
            _connectionThread.Start();
            game.setIsOnlineGame(true);

            startServerButton.Text = "Остановить сервер";
        }

        //Клиент
        private void connectToServerButton_Click(object sender, EventArgs e)
        {
            // подключение к серверу
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            string ip = textBox1.Text.Trim();
            Match m = Regex.Match(ip, pattern);
            if (!m.Success)
            {
                MessageBox.Show("Введен некорректный IP адрес");
                return;
            }
            ip = m.Value;

            if (_connection != null)
            {
                _connection.Terminate();
                if (_connectionThread != null) _connectionThread.Abort();
                _connection = null;
                _connectionThread = null;
            }

            _connection = new Connection(_serverPort, ip);
            _connection.Chatted += DisplayPrivateMessage;
            _connection.GotSettings += GotSettingsHandler;
            _connection.Moved += DrawEnemyTurn;
            _connection.ResetSignal += ResetGameAndMap;
            _connectionThread = new Thread(new ThreadStart(ConnectServer));
            _connectionThread.Start();
        }

        private void ConnectServer()
        {
            try
            {
                _connection.StartClient();
            }
            catch (SocketException)
            {
                MessageBox.Show("Не удалось подключиться");
                
            }
        }

        //Отправляем сообщение
        private void SendTextButton_Click(object sender, EventArgs e)
        {
            if (_connection == null) return;
            if (sendMessageTextBox.Text == "") return;
            string message = "Я: " + sendMessageTextBox.Text + "\n";
            chatTextBox.AppendText(message);

            // TODO в новом потоке? 
            _connection.SendMessage(sendMessageTextBox.Text);

            sendMessageTextBox.Text = "";
        }

        //Отправляем сообщение клавишей Enter
        private void sendMessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendTextButton_Click(this, new EventArgs());
            }
        }

        private void SetScore()
        {
            // TODO

            //ScoreLabel.Text = ""
        }

        //Закрываем поток
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connection != null) _connection.Terminate();
            if (_connectionThread != null) _connectionThread.Abort();
            if (chat != null) chat.Terminate();
            if (chatThread != null) chatThread.Abort();
        }

        private void sendMessageTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendTextButton2_Click(this, new EventArgs());
            }
        }

        private void SendTextButton2_Click(object sender, EventArgs e)
        {
            if (chat == null) return;
            if (textBox2.Text == "") return;
            string message = "Я: " + textBox2.Text + "\n";
            richTextBox1.AppendText(message);

            chat.SendMessage(textBox2.Text);

            textBox2.Text = "";
        }

        public void DisplayMulticastMessage(string ip, string message)
        {
            chatTextBox.Invoke((MethodInvoker)(() => richTextBox1.AppendText(ip + ": " + message + "\n")));
        }

        public void UpdateScore()
        {
            ScoreLabel.Text = game.GetGameScore();
        }
/*
TODO:
//- пофиксить, что первый ход клиент делает с нолика
- счет
- сделать какой-то лэйбл-индикатор: запущен ли сервер, есть ли подключение, с кем игра, чей ход
- зачеркивать не только первую попавшуюся комбинацию
- изменение и создание поля после установки соединения
- обрабатывать конкуренцию за mapGraphics и Simbols
*/
    }
}
