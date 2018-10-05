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
        public Drawer (int X,int Y,Graphics mapGraphics,Graphics Simbols)
        {
            this.X = X;
            this.Y = Y;
            longX = X * shift;
            longY = Y * shift;
            this.mapGraphics = mapGraphics;
            this.Simbols = Simbols;
        }
        private Graphics Simbols;
        private Graphics mapGraphics;

        //Настройки
        private int X = 0; // число клеток по горизонтали (ЗАДАЕТСЯ ПРОГРАММНО)
        private int Y = 0; //Число клеток по вертикали (ЗАДАЕТСЯ ПРОГРАММНО)
        private int offsetX = 50;//отступ
        private int offsetY = 50;//отступ 
        private int shift = 50;//размер клетки?
        private int longX;//Длина по Х (ВЫСЧИТЫВАЕТСЯ АВТОМАТИЧЕСКИ)
        private int longY;//Длина по У (ВЫСЧИТЫВАЕТСЯ АВТОМАТИЧЕСКИ)

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

        public void FillMap( int [,] playGrounds)
        {
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
            Pen p = new Pen(Color.Green, 4);
            mapGraphics.DrawLine(p, offsetX + shift * x1 + shift / 2, offsetY + shift * y1 + shift / 2, offsetX + shift * x2 + shift / 2, offsetY + shift * y2 + shift / 2);
        }
    }
}
