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

        public Graphics gr;
        public Graphics Simbols;
        private int stageCounter; // Счетчик шагов
        private int X = 0;
        private int Y = 0;
        private int W = 0;

        private readonly int _serverPort = 33377;
        private bool isServer = false;
        private bool isClient = false;
        Connection _connection;
        Thread _connectionThread;
        Drawer drawer;
        TheGame game;

        public MainForm()
        {
            InitializeComponent();
            gr = CreateGraphics();
            Simbols = CreateGraphics();
            Console.WriteLine(ClientSize);
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
            drawer = new Drawer(form.GetX(), form.GetY(), gr, Simbols,ClientSize.Height,ClientSize.Width,chatTextBox.ClientSize.Width);//Экземпляр рисовалки
            game = new TheGame(drawer,form.GetW(), form.GetX(), form.GetY());
            game.SetTurn(true);
            gr.Clear(this.BackColor);
            drawer.DrawMap();
            stageCounter = 0;
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
            game.UserTurn(e.X,e.Y);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (game != null && game.GetIsGameStarted())
            {
                gr.Clear(this.BackColor);
                drawer.DrawMap();
                game.ResetPlayGrounds();
                stageCounter = 0;
                game.SetTurn (true);
                game.SetIsGameEnded(false);
            }
        }

        public void WriteMessage(string message)
        {
            chatTextBox.Invoke((MethodInvoker) (() => chatTextBox.AppendText("Соперник: " + message + "\n")));
        }

        public void ShowRequestMessage(string ip)
        {
            DialogResult dialogResult = MessageBox.Show("Принять запрос на игру от " + ip + "?", "Запрос на подключение", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                _connection.SendSettings(X, Y, W);
                stageCounter = 0;
                game.SetIsGameStarted (true);
                game.SetIsGameEnded(false);
                //playGrounds = new int[X, Y];
                game.SetTurn(false);
                game.setIsOnlineGame (true);
                game.setYourOnlineTurn (true);

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
            drawer = new Drawer(X, Y, gr, Simbols, ClientSize.Height, ClientSize.Width, chatTextBox.ClientSize.Width);
            game = new TheGame(drawer, w, x, y);
            gr.Clear(this.BackColor);
            drawer.DrawMap();
            stageCounter = 0;
            game.SetIsGameStarted(true);
            game.SetIsGameEnded(false);
            game.ResetPlayGrounds();
            game.SetTurn (false);
            game.setIsOnlineGame(true);
            game.setYourOnlineTurn(false);
        }

        public void DrawEnemyTurn(int x, int y)
        {
            // TODO
            game.ChangePlayGrounds(x, y);
            drawer.DrawMap();
            drawer.FillMap(game.getPlayGrounds());
            stageCounter += 1;
            game.WinnerSearcher();
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
            _connection.Chatted += WriteMessage;
            _connection.Requested += ShowRequestMessage;
            _connection.Moved += DrawEnemyTurn;
            _connectionThread = new Thread(new ThreadStart(_connection.StartServer));
            _connectionThread.Start();
            isServer = true;
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
            _connection.Chatted += WriteMessage;
            _connection.GotSettings += GotSettingsHandler;
            _connection.Moved += DrawEnemyTurn;
            _connectionThread = new Thread(new ThreadStart(ConnectServer));
            _connectionThread.Start();
            isClient = true; //?
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
        }
        /*
        TODO:
         - счет
         - хранить значения isClient и isServer в классе Connection
         - сделать какой-то лэйбл-индикатор: запущен ли сервер, есть ли подключение
         - зачеркивать не только первую попавшуюся комбинацию
         //- обработка отказа от игры на клиенте
         //- обрабатывать неудачные подключения (со стороны клиента)
         - изменение и создание поля после установки соединения
         - обрабатывать конкуренцию за mapGraphics и Simbols
        */
    }
}
