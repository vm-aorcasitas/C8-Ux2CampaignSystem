using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GanoExcel.Web.Base;

namespace WebBaseFoo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            User user = Security.AuthenticateUser(textBox1.Text, textBox2.Text, Utilities.GetConnectionString());
            if (user == null)
                MessageBox.Show("Bad Login");
            else
                MessageBox.Show(user.Id.ToString());
        }

                   

        private void button2_Click(object sender, EventArgs e)
        {   
            DateTime callDate = DateTime.Parse(textBox4.Text).Date + DateTime.Parse(textBox3.Text).TimeOfDay;
            MessageBox.Show(callDate.ToString());
        }

    }
}

