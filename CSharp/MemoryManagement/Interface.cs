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
        object testO;

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

        private void addMemory()
        {
            int ProgramSize = 0;
            if (_programRandomSize)
            {
                ProgramSize = random.Next(1, 257);
            }
            else
            {
                ProgramSize = (int)Math.Pow(2, (comboBoxProgramSelectedSize.SelectedIndex + 1));
            }
            //MessageBox.Show("New program is, " + (char)programName + " with a size of " + ProgramSize);
            createMemory(ProgramSize, memoryPhysical, _frameSize);
            updateProgressbar(progressBarPhysical, memoryPhysical);
            updateProgressbar(progressBarSwap, memoryStorage);
        }

        

        private void createMemory(int ProgramSize, Button[] memory, double blockSize)
        {
            int start = scanSpace((int)Math.Ceiling(ProgramSize / blockSize), memoryPhysical);
            bool couldHelp = true;
            while (start == -1 && couldHelp)
            {
                couldHelp = FIFO();
                start = scanSpace((int)Math.Ceiling(ProgramSize / blockSize), memoryPhysical);
            }

            if (start != -1)
            {
                double[] arr = new double[] { programName, ProgramSize, 1 };
                programInformation.Add(arr);
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                int end = start + (int)Math.Ceiling(ProgramSize / blockSize);

                for (int i = start; i < end; i++)
                {
                    memory[i].Text = (char)programName + " : " + ProgramSize;
                    memory[i].ForeColor = Color.White;
                    memory[i].BackColor = color;
                }

                Console.WriteLine("[  OK   ] Program " + (char)programName + " is added to ram.");
                programName++;
                addInfo(arr);
            }
            else
            {
                Console.WriteLine("[ ERROR ] Program " + (char)programName + " could not be added since their is no space in ram or programs that can be moved to swap.");
                MessageBox.Show("ERROR!");
            }
        }

        private bool FIFO()
        {
            if (programInformation.Count > 0)
            {

                double programSize = programInformation[0][1];
                int blockRequired = (int)Math.Ceiling(programInformation[0][1] / _pageSize);

                int start = scanSpace(blockRequired, memoryStorage);
                while (start == -1)
                {
                    removeFirstProgram(memoryStorage, infoSwap);
                    Console.WriteLine("[  OK   ] Program " + (char)infoSwap[0][0] + " is dropped.");
                    infoSwap.RemoveAt(0);
                    start = scanSpace(blockRequired, memoryStorage);
                    MessageBox.Show("Drop occured! Because " + blockRequired + " blocks is needed. Start : " + start);
                }

                Color color = getColor((int)programInformation[0][0]);
                int end = start + blockRequired;

                for (int i = start; i < end; i++)
                {
                    memoryStorage[i].Text = (char)programInformation[0][0] + " : " + programInformation[0][1];
                    memoryStorage[i].ForeColor = Color.White;
                    memoryStorage[i].BackColor = color;
                }
                for (int i = 0; i < memoryPhysical.Length; i++)
                {
                    if (memoryPhysical[i].Text.Contains((char)programInformation[0][0]))
                    {
                        memoryPhysical[i].Text = "null";
                        memoryPhysical[i].ForeColor = Color.Black;
                        memoryPhysical[i].BackColor = Color.LightGray;
                    }
                }

                Console.WriteLine("[  OK   ] Program " + (char)programInformation[0][0] + " is moved to swap.");

                infoSwap.Add(programInformation[0]);
                programInformation.RemoveAt(0);

                return true;
            }
            else
            {
                return false;
            }
        }

        private void updateProgressbar(ProgressBar bar, Button[] memory)
        {
            int count = 0;
            for (int i = 0; i < memory.Length; i++)
            {
                if (!memory[i].Text.Contains("null"))
                {
                    count++;
                }
            }
            bar.Maximum = memory.Length;
            bar.Value = count;
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

        private void addInfo(double[] program)
        {
            textBoxInfo.AppendText("Program " + (char)program[0] + " :\t" + program[1] + " bytes.");
            textBoxInfo.AppendText(Environment.NewLine);
        }

        private int scanSpace(int blocksRequired, Button[] memory)
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

        private void removeFirstProgram(Button[] memory, List<double[]> queue)
        {
            for (int i = 0; i < memory.Length; i++)
            {
                if (memory[i].Text.Contains((char)queue[0][0]))
                {
                    resetBlock(memory[i]);
                }
            }
            Console.WriteLine("[  OK   ] Removed " + (char)queue[0][0] + ".");
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Random must be on...\nWould you like to turn it on?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            //{

                checkBoxProgramRandomSize.Checked = true;
            //}

            /*while (true)
            {
                //addMemory();
                

            }*/
            Thread thread = new Thread(new ThreadStart(addProgramThread));
            thread.Start();
            
        }

        // DIFFERENT THREAD TESTS

        private void addProgramThread()
        {
            //
            this.Invoke((MethodInvoker)delegate
            {
                buttonStart.Enabled = false;

            });

            // determine program details
            int pSize = 0;
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
            int pName = programName;
            Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            int pBlocks = (int)Math.Ceiling(pSize / _frameSize);

            // scan for space
            int start = scanSpace(pBlocks, memoryPhysical);
            bool couldHelp = true;

            // if no space, move to swap
            while (start == -1 && couldHelp)
            {
                couldHelp = threadFIFO();
                start = scanSpace(pBlocks, memoryPhysical);
            }

            // add program to ram
            if (start != -1)
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
                    Thread.Sleep(500);
                }
                Console.WriteLine("[  OK   ] Program " + (char)pName + " is added to ram.");
                programName++;

                // add details to list
                programInformation.Add(new double[] { pName, pSize });
            }
            else
            {
                Console.WriteLine("[ ERROR ] Program " + (char)pName + " could not be added since their is no space in ram or programs that can be moved to swap.");
            }

            this.Invoke((MethodInvoker)delegate
            {
                buttonStart.Enabled = true;
                buttonStart.PerformClick();
            });
        }

        private bool threadFIFO()
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
                    for (int i = 0; i < memoryStorage.Length; i++)
                    {
                        if (memoryStorage[i].Text.Contains((char)infoSwap[0][0]))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                memoryStorage[i].Text = "null";
                                memoryStorage[i].ForeColor = Color.Black;
                                memoryStorage[i].BackColor = Color.LightGray;

                            });
                            Thread.Sleep(500);
                           
            
                        }
                    }
                    Console.WriteLine("[  OK   ] Program " + (char)infoSwap[0][0] + " is dropped.");
                    infoSwap.RemoveAt(0);
                    start = scanSpace(blockRequired, memoryStorage);
                }

                // determine color of program moved to swap
                Color color = Color.Transparent;
                    for (int i = 0; i < memoryPhysical.Length; i++)
                    {
                        if (memoryPhysical[i].Text.Contains((char)programInformation[0][0]))
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                color = memoryPhysical[i].BackColor;

                            });
                            

                        
                        }
                    }
                
                
                // remove from ram
                for (int i = 0; i < memoryPhysical.Length; i++)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                    if (memoryPhysical[i].Text.Contains((char)programInformation[0][0]))
                    {
                        
                            memoryPhysical[i].Text = "null";
                            memoryPhysical[i].ForeColor = Color.Black;
                            memoryPhysical[i].BackColor = Color.LightGray;

                        
                        
                    }
                    });

                    Thread.Sleep(500);
                }

                int count = 0;
                for (int i = 0; i < memoryPhysical.Length; i++)
                {
                    if (!memoryPhysical[i].Text.Contains("null"))
                    {
                        count++;
                    }
                }

                this.Invoke((MethodInvoker)delegate
                {
                    progressBarPhysical.Maximum = 28;
                    progressBarPhysical.Value = count;

                });
                

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
                    Thread.Sleep(500);

                }

                count = 0;
                for (int i = 0; i < memoryStorage.Length; i++)
                {
                    if (!memoryStorage[i].Text.Contains("null"))
                    {
                        count++;
                    }
                }

                this.Invoke((MethodInvoker)delegate
                {
                    progressBarSwap.Maximum = 28;
                    progressBarSwap.Value = count;

                });

                Console.WriteLine("[  OK   ] Program " + (char)programInformation[0][0] + " is moved to swap.");

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
