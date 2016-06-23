using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matrice
{
    public partial class Map : Form
    {
        private int length;
        private string[,] path;
        private int[,] way;
        private string result;

        public Map()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBoxFileContent != null)
                ResetApp();

            OpenFileDialog opfd = new OpenFileDialog();
            opfd.Filter = "Text document|*.txt";
            opfd.Title = "Choose file to open";
            opfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            if (opfd.ShowDialog() == DialogResult.OK)
                Charger(opfd.FileName);
        }

        private void ResetApp()
        {
            richTextBoxFileContent.Clear();
            richTextBoxResult.Clear();
            result = String.Empty;
        }

        private string SwitchPoint(int n)
        {
            string s;
            int value = 97;
            value += n;
            s = Convert.ToChar(value).ToString();
            
            return s;
        }

        private void Charger(string fileName)
        {
            try
            {
                string text = System.IO.File.ReadAllText(fileName);
                richTextBoxFileContent.Text = text;
                labelFileName.Text = fileName;

                StreamReader sr = new StreamReader(fileName);

                length = int.Parse(sr.ReadLine()); // get Matrice length

                path = new string[length, length];
                way = new int[100,100];
                string s = sr.ReadLine();

                int i = 0;
                #region get value
                while (s != null)
                {
                    string[] b = s.Split(' ');
                    for (int j = 0; j < length; j++)
                        path[i, j] = b[j];

                    i++;
                    s = sr.ReadLine();
                }
                #endregion
                sr.Close();

                Floyd();
                Display();
            }

            #region error open file
            catch (FormatException ex)
            {
                MessageBox.Show("File " + fileName + " unusual, something incorrect!\nPlease try again!", "Error");
            }
            catch (Exception other)
            {
                MessageBox.Show("File " + fileName + " unusual!\nPlease try again!", "Other error");
            }
            #endregion
        }

        private void Floyd()
        {
            #region init result
            for (int i = 0; i < length;i++ )
            {
                for(int j=0;j<length;j++)
                {
                    way[i, j] = i;
                }
            }
            #endregion

            #region floyd
            for (int k = 0; k < length; k++)
                for (int i = 0; i < length; i++)
                    for (int j = 0; j < length; j++)
                    {
                        int a, b, c;
                        
                        if (Int32.TryParse(path[i, j], out a))
                        {
                            if (Int32.TryParse(path[i, k], out b) && Int32.TryParse(path[k, j], out c))
                                if (a > b + c)
                                    SetValue(b, c, i, j, k);
                        }

                        else if (Int32.TryParse(path[i, k], out b) && Int32.TryParse(path[k, j], out c))
                                SetValue(b, c, i, j, k);
                    }
            #endregion
        }

        private void PrintWay(int x, int y)
        {
            int r;
            if (way[x,y] == x)
            {
                result += "->" + SwitchPoint(y);
                return;
            }
            else
            {
                r = way[x,y];
                PrintWay(x, r);
                PrintWay(r, y);
            }
        }


        private void SetValue(int b, int c, int i, int j, int k)
        {
                path[i, j] = (b + c).ToString();
                way[i,j] = k;
        }

        private void Display()
        {
            int i, j, a;

            richTextBoxFileContent.Text += "\r\n\n"; 
            for (i = 0; i < length; i++)
            {
                for (j = 0; j < length; j++)
                {
                    richTextBoxFileContent.Text += path[i, j] + "  "; 
                }
                richTextBoxFileContent.Text += "\r\n"; 
            }
            for (i = 0; i < length; i++)
                for (j = 0; j < length; j++)
                    if (i != j && Int32.TryParse(path[i,j], out a))
                    {
                        result += SwitchPoint(i);
                        PrintWay(i, j);
                        result +=  ": " + path[i, j] + "\r\n";
                    }

            richTextBoxResult.Text = result;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllText(saveDialog.FileName, result);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("It's an project to find the way between 2 nodes.\n\nProfessor HUYNH Tuong Nguyen\n\nStudent PHAN Anh Thu\nLINF14 - PUFHCM", "About", MessageBoxButtons.OK);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to quit?", "Quit", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                Application.Exit();
        }
    }
}
