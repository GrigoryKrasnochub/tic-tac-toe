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
        private bool isGameStarted=false; // Начинали ли игру хоть раз?
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
        Connection _connection;
        Thread _clientThread;


        public MainForm()
        {
            InitializeComponent();
            gr= CreateGraphics();
            Simbols = CreateGraphics();
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
            gr.Clear(this.BackColor);
            DrawMap(X, Y);
            stageCounter = 0;
            isGameStarted = true;
            isGameEnded = false;
            playGrounds = new int[X, Y];

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            FillMap();
            DrawMap(X, Y);
            if (isGameEnded)crossOutWinner(x1, y1, x2, y2);
        }

        private void DrawMap(int x, int y){
           // gr.Clear(this.BackColor);
             offsetX=50;
             offsetY=50;
             shift = 50;
             longX = x*shift;
             longY=y*shift;
            Pen p = new Pen(Color.Black, 2);
            

            for (int i = 0; i < x+1;i++ ) // Вертикали
            {
                gr.DrawLine(p, offsetX + shift * i, offsetY, offsetX + shift * i, offsetY + longY);
            }
            for (int i = 0; i < y + 1; i++) // Горизонтали
            {
                gr.DrawLine(p, offsetX, offsetY + shift * i, offsetX+longX, offsetY + shift * i);
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

            int xPos=(e.X - offsetX - (e.X - offsetX) % shift) / shift;
            int yPos = (e.Y - offsetY - (e.Y - offsetY) % shift) / shift;
           

            if (playGrounds[xPos, yPos] != 0)
            {
                return;
            }
            int test = playGrounds[xPos, yPos];
            playGrounds[xPos, yPos] = turn ? 1 : 2;

            FillMap();
            DrawMap(X, Y);
            stageCounter += 1;//Счетчик хода
            WinnerSearcher(playGrounds);
            
            turn = !turn;
            
        }

        private void FillMap(){
            for (int i = 0; i < longX / shift; i++)
            {
                for (int j = 0; j < longY / shift; j++)
                {
                    if (playGrounds[i, j] == 1)
                    {
                        DrawCross(i, j);
                    }
                    if (playGrounds[i, j] == 2)
                    {
                        DrawCircle(i, j);
                    }
                }

            }
        }

        private void DrawCross(int i, int j) {
            Pen p = new Pen(Color.Red, 2);
            Simbols.DrawLine(p, offsetX + shift * i, offsetY + shift * j, offsetX + shift * i+shift, offsetY + shift * j+shift);
            Simbols.DrawLine(p, offsetX + shift * i+shift, offsetY + shift * j, offsetX + shift * i , offsetY + shift * j +shift);
        }
        private void DrawCircle(int i, int j)
        {
            Pen p = new Pen(Color.Blue, 2);
            Simbols.DrawEllipse(p, offsetX + shift * i, offsetY + shift * j, shift, shift);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isGameStarted == true)
            {
                gr.Clear(this.BackColor);
                DrawMap(X, Y);
                playGrounds = new int[X, Y];
                stageCounter = 0;
                turn = true;
                isGameEnded = false;
            }
        }
        
        private void crossOutWinner(int x1, int y1, int x2, int y2)
        {
            Pen p = new Pen(Color.Green, 4);
            gr.DrawLine(p, offsetX + shift * x1 + shift/2, offsetY + shift * y1 + shift/2, offsetX + shift * x2 + shift/2, offsetY + shift * y2 + shift/2);
        }

        private void WinnerSearcher(int [,] field) // Ищет и выводит победителя
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
                        crossOutWinner(ii, jj, i, j);
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
                    if (field[i, j] == val && field.GetLength(1)-j>=W && field.GetLength(0) - i>=W)
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
                            y1 =j;
                            x2 = i+W-1;
                            y2 = j+W-1;
                            crossOutWinner(i, j, i + W - 1, j + W - 1);
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
                // TODO проверить поворот координат точек ii jj при z = 1
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
                        crossOutWinner(jj, ii, j, i);
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
                    if (field[i, j] == val &&  i-W+1 >= 0 &&  j+W<= field.GetLength(1))
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
                            x2 = i-W+1;
                            y2 = j+W-1;
                            crossOutWinner(i, j, i - W + 1, j + W - 1);
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
            chatTextBox.Invoke((MethodInvoker)(() => chatTextBox.AppendText("Соперник: " + message + "\n")));
        }

        public void ShowRequestMessage()
        {
            DialogResult dialogResult = MessageBox.Show("Принять запрос на игру?", "Запрос на подключение", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // TODO
            }
            else if (dialogResult == DialogResult.No)
            {
                // TODO
            }
        }

        private void startServerButton_Click(object sender, EventArgs e)
        {
            // запуск сервера
            _connection = new Connection(_serverPort);
            _connection.Chatted += WriteMessage;
            _connection.Requested += ShowRequestMessage;
            _clientThread = new Thread(new ThreadStart(_connection.StartServer));
            _clientThread.Start();
            isServer = true;

        }

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

            _connection = new Connection(_serverPort, ip);
            _connection.Chatted += WriteMessage;
            Thread clientThread = new Thread(new ThreadStart(_connection.StartClient));
            clientThread.Start();
            isClient = true;
        }

        private void SendTextButton_Click(object sender, EventArgs e)
        {
            if (sendMessageTextBox.Text == "") return;
            string message = "Я: " + sendMessageTextBox.Text + "\n";
            chatTextBox.AppendText(message);

            // TODO в новом потоке? 
            _connection.SendMessage(sendMessageTextBox.Text);

            sendMessageTextBox.Text = "";
        }

        private void sendMessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendTextButton_Click(this, new EventArgs());
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connection != null) _connection.Terminate();
            if (_clientThread != null) _clientThread.Abort();
        }
        /*
        TODO:
         - счет
         //- закрытие треда сервера и клиента, когда закрываешь программу
         //- сделать нормальное разделение на Сервер и Клиент. Либо объединить это всё в один класс
         - запрещать несколько подключений на сервере
         - хранить значения isClient и isServer в классе Connection
         - сделать обработку отсутствия подключения (при отправке в чате например)
         - сделать какой-то лэйбл-индикатор: запущен ли сервер, есть ли подключение
        */
    }
}
