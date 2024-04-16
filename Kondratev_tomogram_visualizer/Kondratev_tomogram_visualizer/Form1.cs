using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using System.IO;

namespace Kondratev_tomogram_visualizer
{
    public partial class Form1 : Form
    {
        bool loaded = false;
        bool needReload = false;
        int currentLayer = 0; // примерное значение для currentLayer
        Bin bin; // объявляем переменную bin в классе Form1
        View view; // объявляем переменную view в классе Form1
        int FrameCount = 0;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);
        private VisualizationMode currentVisualizationMode;
        int transferMin = 0;
        int transferMax = 2000;
        public Form1()
        {
            InitializeComponent();

            bin = new Bin(); // инициализируем объект bin
            view = new View(); // инициализируем объект view
        }
        public enum VisualizationMode
        {
            Quads,
            QuadsStrip,
            Texture
        }

        public void SetVisualizationMode(VisualizationMode mode)
        {
            currentVisualizationMode = mode;
            // Перерисовываем окно для отображения выбранного режима визуализации
            glControl1.Invalidate();
        }

        void displayFPS()
        {
            FrameCount++;
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = "FPS: " + FrameCount + " " + "Mode: " + currentVisualizationMode.ToString();
                FrameCount = 0;
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(Application_Idle);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                needReload = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                switch(currentVisualizationMode)
                {
                    case VisualizationMode.Quads: 
                        view.DrawQuads(currentLayer, transferMin, transferMax);
                        break;
                    case VisualizationMode.QuadsStrip: 
                        view.DrawQuadsStrip(currentLayer, transferMin, transferMax);
                        break;
                    case VisualizationMode.Texture:
                        if (needReload)
                        {
                            view.generateTextureImage(currentLayer, transferMin, transferMax);
                            view.Load2DTexture();
                            needReload = false;
                        }
                        view.DrawTexture();
                        break;
                }
                glControl1.SwapBuffers();
            }
        }


        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void quadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetVisualizationMode(VisualizationMode.Quads);
        }

        private void textureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetVisualizationMode(VisualizationMode.Texture);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        private void trackBar23_Scroll(object sender, EventArgs e)
        {
            transferMin = trackBar2.Value;
            transferMax = transferMin + trackBar3.Value;
            needReload = true;
        }

        private int GetValueForTransferFunction(int layer, int minValue, int maxValue)
        {
            // Здесь вы можете добавить дополнительную логику для расчета значения Transfer Function
            return layer; // В данном примере просто возвращается номер выбранного слоя
        }

        private Color TransferFunction(int transferFunctionValue)
        {
            if (transferFunctionValue < 50)
            {
                return Color.Blue;
            }
            else
            {
                return Color.Red;
            }
        }

        private void quadStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentVisualizationMode = VisualizationMode.QuadsStrip;
        }
    }
}
