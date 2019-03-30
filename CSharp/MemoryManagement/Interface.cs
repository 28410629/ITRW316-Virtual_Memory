using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryManagement
{
    public partial class FormMain : Form
    {
        private double _frameSize = 18;
        private double _pageSize = 40;
        private const int blockLimit = 27;
        private bool _programRandomSize = false;
        private Random random = new Random();
        private Button[] memoryPhysical;
        private Button[] memoryStorage;
        private int programName = 65;
        private List<double[]> programInformation = new List<double[]>();
        private List<double[]> infoSwap = new List<double[]>();
        private int _sleepTime = 250;
        private bool isSimulation;
        private bool AddProgram = true; //reset required

        // CONSTRUCTOR

        public FormMain()
        {
            InitializeComponent();
            setup();
        }

        // EVENTS

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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            checkBoxProgramRandomSize.Checked = true;
            buttonStart.Enabled = false;
            buttonProgramAdd.Enabled = false;
            Thread thread = new Thread(new ThreadStart(addProgramThread));
            thread.Start();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormMain main = new FormMain();
            main.ShowDialog();
            this.Close();
        }

        // GUI METHODS

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

            for (int i = 0; i < memoryPhysical.Length; i++)
            {
                resetBlock(memoryPhysical[i]);
                resetBlock(memoryStorage[i]);
            }
        }

        private void resetBlock(Button button)
        {
            button.Text = "null";
            button.ForeColor = Color.Black;
            button.BackColor = Color.LightGray;
        }

        // ASSISTIVE ALGORITHM METHODS

        private void displayMessage(string text)
        {
            this.Invoke((MethodInvoker)delegate
            {
                textBoxInfo.AppendText(text);
                textBoxInfo.AppendText(Environment.NewLine);
            });
        }


        private Color getColor(int name)
        {
            for (int i = 0; i < memoryPhysical.Length; i++)
            {
                if (memoryPhysical[i].Text.Contains((char)name))
                {
                    return memoryPhysical[i].BackColor;
                }
            }
            for (int i = 0; i < memoryStorage.Length; i++)
            {
                if (memoryStorage[i].Text.Contains((char)name))
                {
                    return memoryStorage[i].BackColor;
                }
            }
            return Color.Transparent;
        }

        private int scanSpace(int blocksRequired, Button[] memory) // simple memory allocation with no compression
        {
            int startPosition = -1; // error code if page fault
            int count = 0;
            int index = 0;
            for (int i = 0; i < memory.Length; i++)
            {
                if (memory[i].Text == "null")
                {
                    count++;
                    if (count == 1)
                    {
                        index = i; // save start
                    }
                }
                else
                {
                    if (count >= blocksRequired)
                    {
                        startPosition = index;
                        return startPosition;
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }
            if (count >= blocksRequired)
            {
                startPosition = index;
            }
            return startPosition;
        }

        // MAIN METHOD, ADD PROGRAM TO MEMORY

        private void addProgramThread()
        {
            if (AddProgram)
            {
                int pSize = 0; // determine program details
                if (_programRandomSize)
                {
                    pSize = random.Next(1, 257);
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        pSize = (int)Math.Pow(2, (comboBoxProgramSelectedSize.SelectedIndex + 1));
                    });
                }
                int pName = programName; // new program name
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)); // new program colour
                int pBlocks = (int)Math.Ceiling(pSize / _frameSize); // amount of blocks to allocate to the program

                int start = scanSpace(pBlocks, memoryPhysical); // scan for space
                bool couldHelp = true; // should find better method

                while (start == -1 && couldHelp) // if no space, move to swap according to paging algorithm
                {
                    couldHelp = FIFO();
                    start = scanSpace(pBlocks, memoryPhysical);
                }

                if (start != -1) // add program to ram
                {
                    int end = start + pBlocks;

                    for (int i = start; i < end; i++)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            memoryPhysical[i].Text = (char)pName + " : " + pSize;
                            memoryPhysical[i].ForeColor = Color.White;
                            memoryPhysical[i].BackColor = color;

                        });
                        Thread.Sleep(_sleepTime);
                    }
                    displayMessage("Program " + (char)pName + " is added to ram.");

                    programName++;
                    if (programName == 91)
                    {
                        AddProgram = false;
                    }

                    // add details to list
                    programInformation.Add(new double[] { pName, pSize, start, end });
                }
                else
                {
                    displayMessage("Program " + (char)pName + " could not be added.");
                }

                this.Invoke((MethodInvoker)delegate
                {
                    buttonStart.Enabled = true;
                    buttonStart.PerformClick(); // this will be used to make a simulation
                });
            }
            else
            {
                if (MessageBox.Show("Reset required. Press 'Yes' to reset.", "Simulation End", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    buttonReset.PerformClick();
                } 
            }
        }

        // ALGORITHMS

        private bool FIFO()
        {
            if (programInformation.Count > 0)
            {
                // determine program to be moved to swap
                double programSize = programInformation[0][1];
                int blockRequired = (int)Math.Ceiling(programInformation[0][1] / _pageSize);

                // where it should be located
                int start = scanSpace(blockRequired, memoryStorage);
                while (start == -1)
                {
                    // delete program from swap if no space
                    for (int i = (int)infoSwap[0][2]; i < (int)infoSwap[0][3]; i++)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            memoryStorage[i].Text = "null";
                            memoryStorage[i].ForeColor = Color.Black;
                            memoryStorage[i].BackColor = Color.LightGray;
                        });
                        Thread.Sleep(_sleepTime);
                    }
                    displayMessage("Program " + (char)infoSwap[0][0] + " is dropped.");
                    infoSwap.RemoveAt(0); // remove program from swap information
                    start = scanSpace(blockRequired, memoryStorage); // re-evaluate
                }

                // determine color of program moved to swap
                Color color = Color.Transparent;
                for (int i = 0; i < memoryPhysical.Length; i++)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (memoryPhysical[i].Text.Contains((char)programInformation[0][0]))
                        {
                            color = memoryPhysical[i].BackColor;
                        }
                    });
                }

                // remove from ram
                for (int i = (int)programInformation[0][2]; i < (int)programInformation[0][3]; i++)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        memoryPhysical[i].Text = "null";
                        memoryPhysical[i].ForeColor = Color.Black;
                        memoryPhysical[i].BackColor = Color.LightGray;

                    });
                    Thread.Sleep(_sleepTime);
                }

                // allocate to swap
                int end = start + blockRequired;

                for (int i = start; i < end; i++)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        memoryStorage[i].Text = (char)programInformation[0][0] + " : " + programInformation[0][1];
                        memoryStorage[i].ForeColor = Color.White;
                        memoryStorage[i].BackColor = color;
                    });
                    Thread.Sleep(_sleepTime);
                }

                displayMessage("Program " + (char)programInformation[0][0] + " is moved to swap.");

                programInformation[0][2] = start;
                programInformation[0][3] = end;
                infoSwap.Add(programInformation[0]);
                programInformation.RemoveAt(0);

                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
