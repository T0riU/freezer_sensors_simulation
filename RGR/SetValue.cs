using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RGR
{
    public partial class SetValue : Form
    {
        int currentNum;
        SensorType currentType;
        string text = "";
        Form1 FormLink;
        int wid = 0;
        public SetValue(SensorType currentType,int currentNum,double value, Form1 temp)
        {
            InitializeComponent();
            this.Text = currentType.ToString()+" №"+ currentNum;
            this.currentType = currentType;
            this.currentNum = currentNum;
            trackBar1.Minimum = (int)(Const.MIN_SENSOR_VAL * 10);
            trackBar1.Maximum = (int)(Const.MAX_SENSOR_VAL * 10);
            trackBar1.Value = (int)(value * 10);
            label1.Text = value.ToString();
            FormLink = temp;
            label2.Text = "Normal " + currentType.ToString();
            trackBar1_Scroll(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FormLink.Warning = text;
            //FormLink.Warning_Id = wid;
            FormLink.setSensorVal(currentType, currentNum, ((double)trackBar1.Value / 10.0));
            this.Close();
        }

        private void SetValue_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = ((double)trackBar1.Value/10.0).ToString() + "B";
            if (trackBar1.Value <= 66 && trackBar1.Value >= 54) { label1.ForeColor = Color.Green; text = "Normal " + currentType.ToString(); wid = 0; }
            else if (trackBar1.Value < 54) { label1.ForeColor = Color.Red; text = "WARNING: decreased " + currentType.ToString();  wid = 1; }
            else if (trackBar1.Value > 66) { label1.ForeColor = Color.Red; text = "WARNING: increased " + currentType.ToString(); wid = 2; }
            else if ((trackBar1.Value > 84 || trackBar1.Value < 36)&&SensorType.CURRENTDENSITY!= currentType) { label1.ForeColor = Color.Red; text = "Critical Error"; wid = 3; }
            label2.Text = text;
        }
    }
}
