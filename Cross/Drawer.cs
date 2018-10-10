using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Cross
{
    class Drawer
    {
        //Экземпляр создается при начале новой игры, там же передаются настройки
        public Drawer(int X, int Y, Graphics mapGraphics, Graphics Simbols, int userWindowHeight, int userWindowWidth, int rightOffset)
        {
            this.userWindowHeight = userWindowHeight;
            this.userWindowWidth = userWindowWidth;
            this.X = X;
            this.Y = Y;
            offsetXdown = rightOffset+50;
            shift = GetCellsSize();
            longX = X * shift;
            longY = Y * shift;
            this.mapGraphics = mapGraphics;
            this.Simbols = Simbols;
        }
        private Graphics Simbols;
        private Graphics mapGraphics;
        private int userWindowHeight;
        private int userWindowWidth;
        private int x1;
        private int x2;
        private int y1;
        private int y2;

        //Настройки
        //Стоит помнить, что отступы справа и снизу номинально ими не являются, являсь лишь поправкой размера поля на данную величину
        private int X ; // число клеток по горизонтали (ЗАДАЕТСЯ ПРОГРАММНО)
        private int Y ; //Число клеток по вертикали (ЗАДАЕТСЯ ПРОГРАММНО)
        private int offsetX = 50; //отступ
        private int offsetXdown = 500; //отступ предполагается отступ справа
        private int offsetY = 50; //отступ 
        private int offsetYdown = 50; //отступ 
        private int shift; //размер клетки?
        private int longX; //Длина по Х (ВЫСЧИТЫВАЕТСЯ АВТОМАТИЧЕСКИ)
        private int longY; //Длина по У (ВЫСЧИТЫВАЕТСЯ АВТОМАТИЧЕСКИ)

        public int GetLongX()
        {
            return longX;
        }

        public int GetLongY()
        {
            return longY;
        }

        public int GetShift()
        {
            return shift;
        }
        
        public int GetOffsetX()
        {
            return offsetX;
        }

        public int GetOffsetY()
        {
            return offsetY;
        }

        public void DrawMap()
        {
            // gr.Clear(this.BackColor);
            Pen p = new Pen(Color.Black, 2);


            for (int i = 0; i < X + 1; i++) // Вертикали
            {
                mapGraphics.DrawLine(p, offsetX + shift * i, offsetY, offsetX + shift * i, offsetY + longY);
            }
            for (int i = 0; i < Y + 1; i++) // Горизонтали
            {
                mapGraphics.DrawLine(p, offsetX, offsetY + shift * i, offsetX + longX, offsetY + shift * i);
            }

            mapGraphics.Flush();
        }

        public void FillMap(int[,] playGrounds)
        {
            for (int i = 0; i < playGrounds.GetLength(0); i++)
            {
                for (int j = 0; j < playGrounds.GetLength(1); j++)
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

        private void DrawCross(int i, int j)
        {
            Pen p = new Pen(Color.Red, 2);
            Simbols.DrawLine(p, offsetX + shift * i, offsetY + shift * j, offsetX + shift * i + shift, offsetY + shift * j + shift);
            Simbols.DrawLine(p, offsetX + shift * i + shift, offsetY + shift * j, offsetX + shift * i, offsetY + shift * j + shift);
        }
        private void DrawCircle(int i, int j)
        {
            Pen p = new Pen(Color.Blue, 2);
            Simbols.DrawEllipse(p, offsetX + shift * i, offsetY + shift * j, shift, shift);
        }

        public void crossOutWinner(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            Pen p = new Pen(Color.Green, 4);
            mapGraphics.DrawLine(p, offsetX + shift * x1 + shift / 2, offsetY + shift * y1 + shift / 2, offsetX + shift * x2 + shift / 2, offsetY + shift * y2 + shift / 2);
        }

        public void crossOutWinner()
        {
            Pen p = new Pen(Color.Green, 4);
            mapGraphics.DrawLine(p, offsetX + shift * x1 + shift / 2, offsetY + shift * y1 + shift / 2, offsetX + shift * x2 + shift / 2, offsetY + shift * y2 + shift / 2);
        }

        public int GetCellsSize()
        {
            int ratioX;
            int ratioY;
            ratioX = (userWindowWidth - offsetX - offsetXdown) / X;
            ratioY = (userWindowHeight - offsetY - offsetYdown) / Y;
            return (ratioX < ratioY ? ratioX : ratioY) - (ratioX < ratioY ? ratioX : ratioY) % 2;
        }
    }
}
