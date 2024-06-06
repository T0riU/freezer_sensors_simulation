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
using System.Windows.Forms.VisualStyles;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RGR
{
    public partial class Form1 : Form
    {
        Dictionary<int, SensorType> typesNum = new Dictionary<int, SensorType>(){
            {0, SensorType.TEMPERATURE},
            {1, SensorType.CONCENTRATION},
            {2, SensorType.CURRENTDENSITY}
        };
        Dictionary<SensorType, int> NumSensors = new Dictionary<SensorType, int>() {
            { SensorType.TEMPERATURE, 14},
            { SensorType.CONCENTRATION, 8},
            { SensorType.CURRENTDENSITY, 4}
        };
        Dictionary<SensorType, Dictionary<int, double>> sensors = new Dictionary<SensorType, Dictionary<int, double>>();
        int currentNum;
        SensorType currentType;

        const double MIN_SENSOR_VAL = 0.0;
        public String Warning = "0000";
        public int Warning_Id = 0;
        public double Density = 5.0;
        
        public Form1()
        {

            InitializeComponent();
            numericUpDown1.Minimum = 1;
            comboBox1_SelectedIndexChanged(null, null);
            foreach (SensorType sensorType in Enum.GetValues(typeof(SensorType)))
            {
                if (sensorType.Equals(SensorType.NONE)) continue;
                comboBox1.Items.Add(sensorType.ToString());
                int num = NumSensors.TryGetValue(sensorType, out num) ? num : -1;
                for (int i = 1; i <= num; i++) { setSensorVal(sensorType, i, Const.DF_VAL_SENSOR); }
            }

            comboBox1.SelectedIndex = 0;
            comboBox1_SelectedIndexChanged(null, null);
        }
        public void setSensorVal(SensorType sensorType, int numOfSensor, double val)
        {
            Dictionary<int, double> sensorValueByNum = sensors.TryGetValue(sensorType, out sensorValueByNum) ? sensorValueByNum : new Dictionary<int, double>();
            sensorValueByNum[numOfSensor] = val;
            sensors[sensorType] = sensorValueByNum;
        }
        private double getSensorVal(SensorType sensorType, int numSensor)
        {
            Dictionary<int, double> sensorVal = sensors.TryGetValue(sensorType, out sensorVal) ? sensorVal : new Dictionary<int, double>();
            double val = sensorVal.TryGetValue(numSensor, out val) ? val : 0;
            return val;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentType = typesNum.TryGetValue(comboBox1.SelectedIndex, out currentType) ? currentType : SensorType.NONE;
            int maxNum = NumSensors.TryGetValue(currentType, out maxNum) ? maxNum : -1;
            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = maxNum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Use choose "+ currentType.ToString()+" №"+currentNum);
            SetValue newForm = new SetValue(currentType, currentNum, getSensorVal(currentType, currentNum), this);
            newForm.Show();
            newForm.FormClosing += setText;
            
        }
        public void setText(object sender, EventArgs e)
        {
            List<String> Text = new List<String>();
            Text.Add("In: " + Convert.ToString(currentNum + getSensorAdditionalNumIn(currentType), 2));
            Text.AddRange(getText().ToList());
            richTextBox1.Lines = Text.ToArray();
        }
        private int getSensorAdditionalNumIn(SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.TEMPERATURE:
                    return 0;
                case SensorType.CONCENTRATION:
                    return (NumSensors[SensorType.TEMPERATURE]);
                case SensorType.CURRENTDENSITY:
                    return ((NumSensors[SensorType.TEMPERATURE] + NumSensors[SensorType.CONCENTRATION]));
                default:
                    return 0;
            }
        }
        private int getSensorAdditionalNumOut(SensorType sensorType, bool Cold=true)
        {
            switch (sensorType)
            {
                case SensorType.TEMPERATURE:
                    return Cold ? 32 + NumSensors[SensorType.TEMPERATURE] : 32;
                case SensorType.CONCENTRATION:
                    return Cold ? (int)Math.Pow(2, 6) + (NumSensors[SensorType.TEMPERATURE] * 2 + NumSensors[SensorType.CONCENTRATION]) : (int)Math.Pow(2, 6) + (NumSensors[SensorType.TEMPERATURE]*2);
                case SensorType.CURRENTDENSITY:
                    return (int)Math.Pow(2, 12);
                default:
                    return 0;
            }
        }
        public List<string> getText()
        {
            List<string> status = new List<string>();
            foreach (SensorType sensorType in Enum.GetValues(typeof(SensorType)))
            {
                if (sensorType.Equals(SensorType.NONE))
                    continue;

                int maxNum = NumSensors.TryGetValue(sensorType, out maxNum) ? maxNum : -1;

                for (int i = 1; i <= maxNum; i++)
                {
                    double value = getSensorVal(sensorType, i);
                    if ((value < Const.MIN_CR_SENSOR_VAL || value > Const.MAX_CR_SENSOR_VAL)&& sensorType != SensorType.CURRENTDENSITY)
                    {
                        status.Add("Reason: sensor №" + (i + getSensorAdditionalNumIn(sensorType)) + ";");
                        status.Add("Action: EMERGENCY STATE;");
                        status.Add("");
                    }
                    else {
                        string Text = "";
                        bool dotext = false;
                        bool Cold = true;
                        if (value >= Const.MIN_CR_SENSOR_VAL && value <= Const.MIN_N_SENSOR_VAL)
                        {
                            if (sensorType == SensorType.TEMPERATURE) Text = "The heater is on";
                            else if (sensorType == SensorType.CONCENTRATION) Text = "The concentrate feed is on";
                            else if (sensorType == SensorType.CURRENTDENSITY) Text = "The voltage is reduced; Discred:"+ Math.Round(Math.Abs(9.25925 * value*10 - 1000)/200,2);
                            dotext = true;
                            Cold = false;
                        }
                        else if (value >= Const.MAX_N_SENSOR_VAL && value <= Const.MAX_CR_SENSOR_VAL)
                        {
                            if(sensorType== SensorType.TEMPERATURE) Text = "The refrigerator is on";
                            else if(sensorType == SensorType.CONCENTRATION) Text = "The water feed is on";
                            else if(sensorType == SensorType.CURRENTDENSITY) Text = "The voltage is incresed; Discred:"+ Math.Round(Math.Abs(9.25925 * value*10 - 1111)/200,2);
                            dotext = true;
                        }
                        else if(sensorType == SensorType.CURRENTDENSITY){ 
                            if (value > Const.MAX_N_SENSOR_VAL) { Text = "The voltage is incresed; Discred:" + Math.Round(Math.Abs(9.25925 * value * 10 - 1111)/200,2); dotext = true; } 
                            else if (value < Const.MIN_N_SENSOR_VAL) { Text = "The voltage is decresed; Discred:" + Math.Round(Math.Abs(9.25925 * value * 10 - 1000)/200,2); dotext = true; }  
                        }
                        if (dotext)
                        {
                            int numDevice = i + getSensorAdditionalNumOut(sensorType, Cold);
                            Warning = Convert.ToString(numDevice, 2);
                            status.Add("Reason: sensor №" + (i + getSensorAdditionalNumIn(sensorType)) + ";");
                            status.Add("Action: " + Text + ";");
                            status.Add("Executive device: №" + numDevice + ";");
                            status.Add("RES2 code: " + Warning + ";");
                            status.Add("");
                        }
                    }
                }
            }
            return status;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            currentNum = (int)numericUpDown1.Value;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
//dfd Reason: sensor №1;
//Action: The refrigerator is on;
//Executive device: №47;
//RES2 code: 101111;

//Reason: sensor №16;
//Action: The concentrate feed is on;
//Executive device: №62;
//RES2 code: 111110;



