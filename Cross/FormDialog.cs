using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cross
{
    public partial class FormDialog : Form
    {
        private int X = 3;
        private int Y = 3;
        private int W = 3;
        public FormDialog()
        {
            InitializeComponent();
        }

        public bool IsFormCorrect()
        {
            GetX();
            GetY();
            GetW();

            if ((W > X && W > Y) || X < 2 || Y < 2 || X > 12 || Y > 12) return false;
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsFormCorrect()) MessageBox.Show("Данные некорректны");
            else
            {
                this.Close();
            }
        }

        public int GetX()
        {
            try
            {
                X = int.Parse(comboBox1.Text);
                return X;
            }
            catch (Exception)
            {
                MessageBox.Show("Твое значение наша проблема! Нет! Мы про Х, подумай об этом!");
                X = 3;
                return X;
            }
        }
        public int GetY()
        {
            try
            {
                Y = int.Parse(comboBox2.Text);
                return Y;
            }
            catch (Exception)
            {
                MessageBox.Show("Твое значение наша проблема! Нет! Мы про Y, подумай об этом!");
                Y = 3;
                return Y;
            }
        }
        public int GetW()
        {
            try
            {
                W = int.Parse(comboBox3.Text);
                return W;
            }
            catch (Exception)
            {
                MessageBox.Show("Твое значение наша проблема! Нет! Мы про W, подумай об этом!");
                W = 3;
                return W;
            }
        }
    }
}
