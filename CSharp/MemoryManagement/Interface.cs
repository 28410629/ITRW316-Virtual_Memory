using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryManagement
{
    public partial class FormMain : Form
    {
        private int _physicalCurrentEnd = 0;
        private int _storageCurrentEnd = 0;
        private double _frameSize = 18;
        private double _pageSize = 40;
        private const int blockLimit = 27;
        private bool _programRandomSize = false;
        private Random random = new Random();
        private Button[] memoryPhysical;
        private Button[] memoryStorage;
        private int programName = 65;
        private List<string[]> programInformation = new List<string[]>();

        public FormMain()
        {
            InitializeComponent();
            setup();
        }

        private void checkBoxProgramRandomSize_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxProgramRandomSize.Checked)
            {
                comboBoxProgramSelectedSize.Enabled = false;
                _programRandomSize = true;
            }
            else
            {
                comboBoxProgramSelectedSize.Enabled = true;
                _programRandomSize = false;
            }
        }

        private void setup()
        {
            comboBoxProgramSelectedSize.SelectedIndex = 0;
            comboBoxSelectedAlgorithm.SelectedIndex = 0;
            memoryPhysical = new Button[] { buttonP1, buttonP2, buttonP3, buttonP4, buttonP5, buttonP6, buttonP7,
                                             buttonP8, buttonP9, buttonP10, buttonP11, buttonP12, buttonP13, buttonP14,
                                             buttonP15, buttonP16, buttonP17, buttonP18, buttonP19, buttonP20, buttonP21,
                                             buttonP22, buttonP23, buttonP24, buttonP25, buttonP26, buttonP27, buttonP28 };

            memoryStorage = new Button[] { buttonS1, buttonS2, buttonS3, buttonS4, buttonS5, buttonS6, buttonS7,
                                             buttonS8, buttonS9, buttonS10, buttonS11, buttonS12, buttonS13, buttonS14,
                                             buttonS15, buttonS16, buttonS17, buttonS18, buttonS19, buttonS20, buttonS21,
                                             buttonS22, buttonS23, buttonS24, buttonS25, buttonS26, buttonS27, buttonS28 };
        }
        
        private void addMemory()
        {
            int ProgramSize = 0;
            if (_programRandomSize)
            {
                ProgramSize = random.Next(1, 257);
            }
            else
            {
                ProgramSize = (int) Math.Pow(2,(comboBoxProgramSelectedSize.SelectedIndex + 1));
            }
            createMemory((char)programName + " : " + ProgramSize, ProgramSize, memoryPhysical, _frameSize);
        }

        private void createMemory(string label, int ProgramSize, Button[] memory, double blockSize)
        {
            int start = _physicalCurrentEnd;
            int range = ((int)Math.Ceiling(ProgramSize / blockSize)) + _physicalCurrentEnd;
            if (range > blockLimit)
            {
                MessageBox.Show("No space left!");
            }
            else
            {
                string[] arr = new string[] { (char)programName + "", ProgramSize + "", (ProgramSize / blockSize) + "", Math.Ceiling(ProgramSize / blockSize) + "", start + "", range + "" };
                programInformation.Add(arr);
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                for (int i = _physicalCurrentEnd; i < range; i++)
                {
                    memory[i].Text = label;
                    memory[i].ForeColor = Color.White;
                    memory[i].BackColor = color;
                }
                _physicalCurrentEnd = ((int)Math.Ceiling(ProgramSize / blockSize)) + _physicalCurrentEnd;
                programName++;
                addInfo(arr);
            }
        }

        private void addInfo(string[] program)
        {
            textBoxInfo.AppendText("Program " + program[0] + " :\t" + program[1] + " bytes.");
            textBoxInfo.AppendText(Environment.NewLine);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Random must be on...\nWould you like to turn it on?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                checkBoxProgramRandomSize.Checked = true;
            }

            while(true)
            {
                addMemory();
            }
        }
    }
}
