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
        private bool isGameStarted = false; // Начинали ли игру хоть раз?
        private bool isGameEnded = false; // Был ли отыгран раунд
        public Graphics gr;
        public Graphics Simbols;
        private int stageCounter; // Счетчик шагов
        private int offsetX = 50;
        private int offsetY = 50;
        private int shift = 50;
        private int X = 0;
        private int Y = 0;
        private int W = 0;
        private int x1 = 0;
        private int y1 = 0;
        private int x2 = 0;
        private int y2 = 0;
        private int longX = 0;
        private int longY = 0;
        private int[,] playGrounds;
        public bool turn = true;

        private readonly int _serverPort = 33377;
        private bool isServer = false;
        private bool isClient = false;
        private bool isOnlineGame = false;
        Connection _connection;
        Thread _connectionThread;
        Drawer drawer;

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
            turn = true;
            X = form.GetX();
            Y = form.GetY();
            W = form.GetW();
            drawer = new Drawer(form.GetX(), form.GetY(), gr, Simbols,ClientSize.Height,ClientSize.Width,chatTextBox.ClientSize.Width);//Экземпляр рисовалки
            //ACHТUNG КОСТЫЛИ КОСТЫЛИКИ
            shift = drawer.GetCellsSize();
            //ACHТUNG КОСТЫЛИ КОСТЫЛИКИ
            gr.Clear(this.BackColor);
            longX = X * shift;
            longY = Y * shift;
            drawer.DrawMap();
            stageCounter = 0;
            isGameStarted = true;
            isGameEnded = false;
            playGrounds = new int[X, Y];

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            if (drawer != null)
            {
                drawer.DrawMap();
                drawer.FillMap(playGrounds);
                if (isGameEnded) drawer.crossOutWinner(x1, y1, x2, y2);
            }

        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) //проверка на левую кнопку
            {
                return;
            }
            if (isGameEnded)
            {
                return;
            }
            if (isGameStarted != true)
            {
                return;
            }
            if (e.X < offsetX || e.X > offsetX + longX)
            {
                return;
            }
            if (e.Y < offsetY || e.Y > offsetY + longY)
            {
                return;
            }

            int xPos = (e.X - offsetX - (e.X - offsetX) % shift) / shift;
            int yPos = (e.Y - offsetY - (e.Y - offsetY) % shift) / shift;


            if (playGrounds[xPos, yPos] != 0)
            {
                return;
            }
            int test = playGrounds[xPos, yPos];
            playGrounds[xPos, yPos] = turn ? 1 : 2;

            drawer.FillMap(playGrounds);
            drawer.DrawMap();
            stageCounter += 1;//Счетчик хода
            WinnerSearcher(playGrounds);

            turn = !turn;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isGameStarted == true)
            {
                gr.Clear(this.BackColor);
                drawer.DrawMap();
                playGrounds = new int[X, Y];
                stageCounter = 0;
                turn = true;
                isGameEnded = false;
            }
        }

        //Определение победителя два метода вниз
        private void WinnerSearcher(int[,] field) // Ищет и выводит победителя
        {
            int val = turn ? 1 : 2; // Чей ход того и проверяем, нельзя выиграть в чужой ход
            for (int i = 0; i < field.GetLength(0); i++) // vertical
            {
                int countX = W;
                // TODO проверить поворот координат точек ii jj при z = 1
                int ii = -1;
                int jj = -1;
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == val)
                    {
                        countX -= 1;
                        ii = ii < 0 ? i : ii;
                        jj = jj < 0 ? j : jj;
                    }
                    else
                    {
                        countX = W;
                        ii = -1;
                        jj = -1;
                    }
                    if (countX == 0)
                    {
                        x1 = ii;
                        y1 = jj;
                        x2 = i;
                        y2 = j;
                        drawer.crossOutWinner(ii, jj, i, j);
                        MessageBox.Show(string.Format("Победа {0}", turn ? "Крестики" : "Нолики"));
                        isGameEnded = true;
                        return;
                    }

                }

            }

            for (int i = 0; i < field.GetLength(0); i++) // diagonally \
            {
                int countX = W;
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == val && field.GetLength(1) - j >= W && field.GetLength(0) - i >= W)
                    // определяет есть ли возможность найти выигрышную комбинацию, начиная с этой ячейки
                    {

                        for (int k = 0; k < W; k++)//если есть то проверем
                        {
                            if (field[i + k, j + k] == val)
                                countX -= 1;
                        }
                        if (countX == 0)
                        {
                            x1 = i;
                            y1 = j;
                            x2 = i + W - 1;
                            y2 = j + W - 1;
                            drawer.crossOutWinner(i, j, i + W - 1, j + W - 1);
                            MessageBox.Show(string.Format("Победа {0}", turn ? "Крестики" : "Нолики"));
                            isGameEnded = true;
                            return;
                        }
                        countX = W;
                    }
                }

            }

            for (int i = 0; i < field.GetLength(1); i++) // horizont
            {
                int countX = W;
                int ii = -1;
                int jj = -1;
                for (int j = 0; j < field.GetLength(0); j++)
                {
                    if (field[j, i] == val)
                    {
                        countX -= 1;
                        ii = ii < 0 ? i : ii;
                        jj = jj < 0 ? j : jj;
                    }
                    else
                    {
                        countX = W;
                        ii = -1;
                        jj = -1;
                    }
                    if (countX == 0)
                    {
                        x1 = jj;
                        y1 = ii;
                        x2 = j;
                        y2 = i;
                        drawer.crossOutWinner(jj, ii, j, i);
                        MessageBox.Show(string.Format("Победа {0}", turn ? "Крестики" : "Нолики"));
                        isGameEnded = true;
                        return;
                    }

                }

            }

            for (int i = 0; i < field.GetLength(0); i++) // diagonally /
            {
                int countX = W;
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == val && i - W + 1 >= 0 && j + W <= field.GetLength(1))
                    // определяет есть ли возможность найти выигрышную комбинацию, начиная с этой ячейки
                    {

                        for (int k = 0; k < W; k++) //если есть то проверем
                        {
                            if (field[i - k, j + k] == val)
                                countX -= 1;
                        }
                        if (countX == 0)
                        {
                            x1 = i;
                            y1 = j;
                            x2 = i - W + 1;
                            y2 = j + W - 1;
                            drawer.crossOutWinner(i, j, i - W + 1, j + W - 1);
                            MessageBox.Show(string.Format("Победа {0}", turn ? "Крестики" : "Нолики"));
                            isGameEnded = true;
                            return;
                        }
                        countX = W;
                    }
                }
            }

            // Ничья
            if (isGameEnded == false)
            {
                if (stageCounter == X * Y)
                {
                    MessageBox.Show("Game Over");
                    isGameEnded = true;
                }
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
                turn = false;
                isOnlineGame = true;
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
            gr.Clear(this.BackColor);
            drawer.DrawMap();
            stageCounter = 0;
            isGameStarted = true;
            isGameEnded = false;
            playGrounds = new int[X, Y];
            turn = false;
            isOnlineGame = true;
        }

        public void DrawEnemyTurn(int x, int y)
        {
            // TODO

            playGrounds[x, y] = turn ? 1 : 2;
            drawer.DrawMap();
            drawer.FillMap(playGrounds);
            stageCounter += 1;
            WinnerSearcher(playGrounds);
            turn = !turn;

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
            isOnlineGame = true;
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
            //_connection.Moved += DrawEnemyTurn;
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
