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
        private double _pageSize = 18;
        private double _frameSizeTotal = 28 * 18;
        private double _pageSizeTotal = 56 * 18;
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
        private int countUsedP = 0;
        private int countUsedS = 0;
        private double _num;
        private int _storedName = 0; // stored variable references position in arrays
        private int _storedPage = 1;
        private int _storedSize = 2;
        private int _storedStart = 3;
        private int _storedStop = 4;

        // CONSTRUCTOR

        public FormMain()
        {
            InitializeComponent();
            setup();
        }

        // EVENTS

        private void textBoxFrameSize_TextChanged(object sender, EventArgs e)
        {
            
            if (!double.TryParse(textBoxFrameSize.Text, out _num))
            {
                MessageBox.Show("Please enter valid value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                pageSize(_num);
            }
        }

        private void textBoxPageSize_TextChanged(object sender, EventArgs e)
        {
            if (!double.TryParse(textBoxPageSize.Text, out _num))
            {
                MessageBox.Show("Please enter valid value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                pageSize(_num);
            }
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

        private void pageSize(double num)
        {
            textBoxFrameSize.Text = "" + num;
            textBoxPageSize.Text = "" + num;
            _frameSizeTotal = 28 * num;
            _pageSizeTotal = 56 * num;
            _frameSize = num;
            _pageSize = num;
            labelMemoryUsedP.Text = countUsedP + " / " + _frameSizeTotal + " bytes";
            labelMemoryUsedS.Text = countUsedS + " / " + _pageSizeTotal + " bytes";
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
                                             buttonS22, buttonS23, buttonS24, buttonS25, buttonS26, buttonS27, buttonS28,
                                             buttonS29, buttonS30, buttonS31, buttonS32, buttonS33, buttonS34, buttonS35,
                                             buttonS36, buttonS37, buttonS38, buttonS39, buttonS40, buttonS41, buttonS42,
                                             buttonS43, buttonS44, buttonS45, buttonS46, buttonS47, buttonS48, buttonS49,
                                             buttonS50, buttonS51, buttonS52, buttonS53, buttonS54, buttonS55, buttonS56};

            for (int i = 0; i < memoryPhysical.Length; i++)
            {
                resetBlock(memoryPhysical[i]);
            }
            for (int i = 0; i < memoryStorage.Length; i++)
            {
                resetBlock(memoryStorage[i]);
            }

            textBoxFrameSize.Text = "" + 18;
            textBoxPageSize.Text = "" + 18;

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
                countUsedP = 0;
                countUsedS = 0;

                for (int i = 0; i < infoSwap.Count; i++)
                {
                    countUsedS += (int)infoSwap[i][_storedSize]; // count sizes of swap
                }

                for (int i = 0; i < programInformation.Count; i++)
                {
                    countUsedP += (int)programInformation[i][1]; // count sizes of physical
                }

                this.Invoke((MethodInvoker)delegate
                {
                    labelMemoryUsedP.Text = countUsedP + " / " + _frameSizeTotal + " bytes";
                    labelMemoryUsedS.Text = countUsedS + " / " + _pageSizeTotal + " bytes";
                });

                int pSize = 0; // determine program details
                if (_programRandomSize)
                {
                    pSize = random.Next(1, 129);
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        pSize = (int)Math.Pow(2, (comboBoxProgramSelectedSize.SelectedIndex + 1));
                    });
                }
                string pName = ""; // new program name
                Color color = Color.FromArgb(random.Next(14,129), random.Next(14, 129), random.Next(14, 129)); // new program colour
                int pBlocks = (int)Math.Ceiling(pSize / _frameSize); // amount of blocks to allocate to the program


                for (int j = 0; j < pBlocks; j++)
                {
                    pName = (char)programName + "-" + j;
                    int start = scanSpace(1, memoryPhysical); // scan for space
                    bool couldHelp = true; // should find better method

                    while (start == -1 && couldHelp) // if no space, move to swap according to paging algorithm
                    {
                        couldHelp = FIFO();
                        start = scanSpace(1, memoryPhysical);
                    }

                    if (start != -1) // add program to ram
                    {
                        int end = start + 1;


                        for (int i = start; i < end; i++)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                memoryPhysical[i].Text = pName + " : " + pSize;
                                memoryPhysical[i].ForeColor = Color.White;
                                memoryPhysical[i].BackColor = color;
                            });
                            Thread.Sleep(_sleepTime);
                        }

                        // add details to list
                        programInformation.Add(new double[] { programName, j, pSize, start, end }); // position
                    }
                    else
                    {
                        displayMessage("Program " + pName + " could not be added.");
                    }
                }

                displayMessage("Program " + pName + " is added to ram.");

                programName++;
                if (programName == 91)
                {
                    AddProgram = false;
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
                    this.Invoke((MethodInvoker)delegate
                    {
                        buttonReset.PerformClick();
                    });
                } 
            }
        }

        private void removeProgram(int start, int end, Button[] memory)
        {
            for (int i = start; i < end; i++)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    memory[i].Text = "null";
                    memory[i].ForeColor = Color.Black;
                    memory[i].BackColor = Color.LightGray;
                });
                Thread.Sleep(_sleepTime);
            }
        }

        // ALGORITHMS

        private bool FIFO()
        {
            if (programInformation.Count > 0)
            {
                // determine program to be moved to swap
                double programSize = programInformation[0][_storedSize];
                int blockRequired = (int)Math.Ceiling(programInformation[0][_storedSize] / _pageSize);

                // where it should be located
                for (int i = 0; i < blockRequired; i++)
                {
                    int start = scanSpace(1, memoryStorage);
                    while (start == -1)
                    {
                        removeProgram((int)infoSwap[0][_storedStart], (int)infoSwap[0][_storedStop], memoryStorage); // drop program from swap if no space
                        displayMessage("Program " + (char)infoSwap[0][_storedName] + "-" + infoSwap[0][_storedPage] + " is dropped.");
                        infoSwap.RemoveAt(0); // remove program from swap information
                        start = scanSpace(1, memoryStorage); // re-evaluate
                    }

                    // determine color of program moved to swap
                    Color color = Color.Transparent;
                    for (int j = 0; j < memoryPhysical.Length; j++)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (memoryPhysical[j].Text.Contains((char)programInformation[0][_storedName] + "-" + programInformation[0][_storedPage]))
                            {
                                color = memoryPhysical[j].BackColor;
                            }
                        });
                    }

                    removeProgram((int)programInformation[0][_storedStart], (int)programInformation[0][_storedStop], memoryPhysical); // remove from ram

                    // allocate to swap
                    int end = start + 1;

                    for (int k = start; k < end; k++)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            memoryStorage[k].Text = (char)programInformation[0][_storedName] + "-" + programInformation[0][_storedPage] + " : " + programInformation[0][_storedSize];
                            memoryStorage[k].ForeColor = Color.White;
                            memoryStorage[k].BackColor = color;
                        });
                        Thread.Sleep(_sleepTime);
                    }

                    displayMessage("Program " + (char)programInformation[0][_storedName] + "-" + programInformation[0][_storedPage] + " is moved to swap.");

                    programInformation[0][_storedStart] = start;
                    programInformation[0][_storedStop] = end;
                    infoSwap.Add(programInformation[0]);
                    programInformation.RemoveAt(0);
                }
                

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
