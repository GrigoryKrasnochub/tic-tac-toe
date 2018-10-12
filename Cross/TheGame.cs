using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cross
{
    class TheGame
    {
        public TheGame (Drawer drawer, int W, int X, int Y)
        {
            this.drawer = drawer;
            offsetX = drawer.GetOffsetX();
            offsetY = drawer.GetOffsetY();
            shift = drawer.GetShift();
            this.W = W;
            this.X = X;
            this.Y = Y;
            playGrounds = new int[X, Y];
        }

        private int stageCounter = 0; 
        private Drawer drawer;
        private int offsetX;
        private int offsetY;
        private int shift;
        private int W;
        private int X;
        private int Y;
        private int xPos;
        private int yPos;
        private bool isOnlineGame = false;
        private bool yourOnlineTurn;
        private bool turn;
        private int[,] playGrounds;
        private bool isGameStarted = false; // Начинали ли игру хоть раз?
        private bool isGameEnded = false; // Был ли отыгран раунд
        
        
        public bool UserTurn(int eX, int eY)
        {
            /*
             Возвращает true, если ход был совершен, иначе false
            */
            if (isOnlineGame && yourOnlineTurn != turn )
            {
                return false;
            }
            if (isGameEnded)
            {
                return false;
            }
            if (isGameStarted != true)
            {
                return false;
            }
            if (eX < offsetX || eX > offsetX + drawer.GetLongX())
            {
                return false;
            }
            if (eY < offsetY || eY > offsetY + drawer.GetLongY())
            {
                return false;
            }

            xPos = (eX - offsetX - (eX - offsetX) % shift) / shift;
            yPos = (eY - offsetY - (eY - offsetY) % shift) / shift;


            if (playGrounds[xPos, yPos] != 0)
            {
                return false;
            }

            int test = playGrounds[xPos, yPos];
            playGrounds[xPos, yPos] = turn ? 1 : 2;

            drawer.FillMap(playGrounds);
            drawer.DrawMap();
            stageCounter += 1; //Счетчик хода

            SearchWinner();
            turn = !turn;
            return true;
        }

        // Определение победителя два метода вниз
        // Ищет и выводит победителя
        public void SearchWinner()
        {
            int[,] field = playGrounds;
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

        public void SetTurn(bool turn)
        {
            this.turn = turn;
        }

        public void setIsOnlineGame(bool value)
        {
            isOnlineGame = value;
        }

        public void setYourOnlineTurn(bool value)
        {
            yourOnlineTurn = value;
        }

        public void SetIsGameStarted(bool isGamestr)
        {
            isGameStarted = isGamestr;
        }

        public void SetIsGameEnded(bool isGameend)
        {
            isGameEnded = isGameend;
        }

        public bool GetIsGameStarted()
        {
            return isGameStarted;
        }

        public bool GetIsGameEnded()
        {
            return isGameEnded;
        }

        public bool GetTurn()
        {
            return turn;
        }

        public void ChangePlayGrounds(int x, int y)
        {
            playGrounds[x, y] = turn ? 1 : 2;
        }

        public int[,] getPlayGrounds()
        {
            return playGrounds;
        }

        public void ResetPlayGrounds()
        {
            playGrounds = new int[X, Y];
        }

        public int GetXpos ()
        {
            return xPos;
        }

        public int GetYpos()
        {
            return yPos;
        }
    }
}
