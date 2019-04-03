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
        private bool _programRandomSize = false;
        private Random _random = new Random();
        private Button[] _memoryPhysical;
        private Button[] _memoryStorage;
        private int _programName = 65;
        private List<double[]> _programInformation = new List<double[]>();
        private List<double[]> _pageTable = new List<double[]>();
        private List<double[]> _infoSwap = new List<double[]>();
        private int _sleepTime = 250;
        private bool _isSimulation;
        private bool _AddProgram = true; //reset required
        private int _countUsedP = 0;
        private int _countUsedS = 0;
        private double _num;
        private const int _storedName = 0; // stored variable references position in arrays
        private const int _storedPage = 1;
        private const int _storedSize = 2;
        private const int _storedStart = 3;
        private const int _storedStop = 4;
        private const int _storedPageFrag = 5;
        private string _readProgramPage;
        private int _statsAdds = 0;
        private int _statsRead = 0;
        private int _statsPageFault = 0;
        private int _statsPageFaultResolved = 0;
        private int _statsPageFaultUnresolved = 0;
        private int _statsTotalEntriesTLB = 0;
        private int _statsMovedToSwap = 0;

        // CONSTRUCTOR

        public FormMain()
        {
            InitializeComponent();
            setup();
        }

        // EVENTS

        private void ButtonProgramAdd_Click(object sender, EventArgs e)
        {
            componentsGUI(false);

            Thread thread = new Thread(new ThreadStart(addProgramThread));
            thread.Start();
        }

        private void ButtonReadProgram_Click(object sender, EventArgs e)
        {
            if (checkBoxRandomRead.Checked)
            {
                comboBoxReadProgram.SelectedIndex = _random.Next(comboBoxReadProgram.Items.Count);
            }
            componentsGUI(false);
            _readProgramPage = comboBoxReadProgram.SelectedItem.ToString();
            Thread thread = new Thread(new ThreadStart(readProgramThread));
            thread.Start();
        }

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
        private void CheckBoxRandomRead_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRandomRead.Checked)
            {
                comboBoxReadProgram.Enabled = false;
            }
            else
            {
                comboBoxReadProgram.Enabled = true;
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
            checkBoxRandomRead.Checked = true;
            componentsGUI(false);
            _isSimulation = true;
            _sleepTime = 100;
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

        private void simulation()
        {

            this.Invoke((MethodInvoker)delegate
            {
                if (_random.Next(101) > 40)
                {
                    Console.WriteLine("Read is requested");
                    buttonReadProgram.PerformClick();
                }
                else
                {
                    Console.WriteLine("Add is requested");
                    buttonProgramAdd.PerformClick();

                }
            });
        }

        private void componentsGUI(bool enable)
        {
            comboBoxProgramSelectedSize.Enabled = enable;
            comboBoxReadProgram.Enabled = enable;
            textBoxFrameSize.Enabled = enable;
            textBoxPageSize.Enabled = enable;
            buttonReadProgram.Enabled = enable;
            buttonProgramAdd.Enabled = enable;
            buttonStart.Enabled = enable;

        }

        private void pageSize(double num)
        {
            textBoxFrameSize.Text = "" + num;
            textBoxPageSize.Text = "" + num;
            _frameSizeTotal = 28 * num;
            _pageSizeTotal = 56 * num;
            _frameSize = num;
            _pageSize = num;
            labelMemoryUsedP.Text = _countUsedP + " / " + _frameSizeTotal + " bytes";
            labelMemoryUsedS.Text = _countUsedS + " / " + _pageSizeTotal + " bytes";
        }

        private void setup()
        {
            comboBoxProgramSelectedSize.SelectedIndex = 0;
            _memoryPhysical = new Button[] { buttonP1, buttonP2, buttonP3, buttonP4, buttonP5, buttonP6, buttonP7,
                                             buttonP8, buttonP9, buttonP10, buttonP11, buttonP12, buttonP13, buttonP14,
                                             buttonP15, buttonP16, buttonP17, buttonP18, buttonP19, buttonP20, buttonP21,
                                             buttonP22, buttonP23, buttonP24, buttonP25, buttonP26, buttonP27, buttonP28 };

            _memoryStorage = new Button[] { buttonS1, buttonS2, buttonS3, buttonS4, buttonS5, buttonS6, buttonS7,
                                             buttonS8, buttonS9, buttonS10, buttonS11, buttonS12, buttonS13, buttonS14,
                                             buttonS15, buttonS16, buttonS17, buttonS18, buttonS19, buttonS20, buttonS21,
                                             buttonS22, buttonS23, buttonS24, buttonS25, buttonS26, buttonS27, buttonS28,
                                             buttonS29, buttonS30, buttonS31, buttonS32, buttonS33, buttonS34, buttonS35,
                                             buttonS36, buttonS37, buttonS38, buttonS39, buttonS40, buttonS41, buttonS42,
                                             buttonS43, buttonS44, buttonS45, buttonS46, buttonS47, buttonS48, buttonS49,
                                             buttonS50, buttonS51, buttonS52, buttonS53, buttonS54, buttonS55, buttonS56};
            for (int i = 0; i < _memoryPhysical.Length; i++)
            {
                resetBlock(_memoryPhysical[i]);
            }
            for (int i = 0; i < _memoryStorage.Length; i++)
            {
                resetBlock(_memoryStorage[i]);
            }
            textBoxFrameSize.Text = "" + 18;
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
            for (int i = 0; i < _memoryPhysical.Length; i++)
            {
                if (_memoryPhysical[i].Text.Contains((char)name))
                {
                    return _memoryPhysical[i].BackColor;
                }
            }
            for (int i = 0; i < _memoryStorage.Length; i++)
            {
                if (_memoryStorage[i].Text.Contains((char)name))
                {
                    return _memoryStorage[i].BackColor;
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

        private int readScanTLB(string programPage)
        {
            for (int i = 0; i < _programInformation.Count; i++)
            {

                if (programPage == ((char)_programInformation[i][_storedName] + "-" + _programInformation[i][_storedPage]))
                {
                    return i;
                }
            }
            return -1;
        }

        private int readScanPhysical()
        {
            string text = "";
            for (int i = 0; i < _programInformation.Count; i++)
            {

                if (_readProgramPage == ((char)_programInformation[i][_storedName] + "-" + _programInformation[i][_storedPage]))
                {
                    return i;
                }
            }

            return -1;
        }

        private bool readScanSwap()
        {
            int location = -1;
            for (int i = 0; i < _infoSwap.Count; i++)
            {
                if (_readProgramPage == ((char)_infoSwap[i][_storedName] + "-" + _infoSwap[i][_storedPage]))
                {
                    location = i;
                }
            }
            if (location == -1)
            {
                return false; // not found in swap
            }
            else
            {
                double[] arr = _infoSwap[location];
                _infoSwap.RemoveAt(location); // remove program from swap information

                Color colour = Color.White;

                for (int i = (int)arr[_storedStart]; i < (int)arr[_storedStop]; i++)
                {
                    colour = _memoryStorage[i].BackColor;
                }

                removeProgramFromMemory((int)arr[_storedStart], (int)arr[_storedStop], _memoryStorage); // drop program from swap 
                string pName = (char)arr[_storedName] + "-" + arr[_storedPage];
                double pSize = arr[_storedSize];

                int start = scanSpace(1, _memoryPhysical); // scan for space
                bool couldHelp = true; // should find better method

                while (start == -1 && couldHelp) // if no space, move to swap according to paging algorithm
                {
                    couldHelp = FIFO();
                    start = scanSpace(1, _memoryPhysical);
                }

                if (start != -1) // add program to ram
                {
                    int end = start + 1;

                    for (int i = start; i < end; i++)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            _memoryPhysical[i].Text = pName + " : " + pSize;
                            _memoryPhysical[i].ForeColor = Color.White;
                            _memoryPhysical[i].BackColor = colour;
                        });
                        Thread.Sleep(_sleepTime);
                    }

                    // add details to list
                    _programInformation.Add(new double[] { arr[_storedName], arr[_storedPage], pSize, start, end }); // position

                }
                else
                {
                    displayMessage("Program " + pName + " could not be added.");
                }
                return true;
            }

        }

        // MAIN METHODS, ADD OR READ PROGRAM TO MEMORY

        private void readProgramThread()
        {

            int location;
            // check tlb

            // check memory if cant find it is page fault
            location = readScanPhysical();
            if (location == -1)
            {
                bool found = readScanSwap();
                if (found)
                {
                    location = readScanPhysical();
                    displayMessage("Page " + _readProgramPage + " read in memory.");
                }
                else
                {
                    displayMessage("Page " + _readProgramPage + " unavailable, it was dropped in swap.");
                }
            }
            else
            {
                displayMessage("Page " + _readProgramPage + " read in memory.");
                // add fragmentation/size
            }

            // update tlb
            // end
            this.Invoke((MethodInvoker)delegate
            {
                componentsGUI(true);
            });
            Console.WriteLine("Simulation : " +_isSimulation);
            if (_isSimulation)
            {
                simulation();
            }

        }

        private int determineProgramSize()
        {
            int size = 0;
            if (checkBoxProgramRandomSize.Checked)
            {
                size = _random.Next(1, 129);
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    size = (int)Math.Pow(2, (comboBoxProgramSelectedSize.SelectedIndex + 1));
                });
            }
            return size;
        }

        private void removeProgramFromMemory(int start, int end, Button[] memory)
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

        private void addProgramToMemory(int startAllocating, string blockName, Color blockColor, Button[] memory)
        {
            int end = startAllocating + 1;

            for (int i = startAllocating; i < end; i++)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    memory[i].Text = blockName;
                    memory[i].ForeColor = Color.White;
                    memory[i].BackColor = blockColor;
                });
                Thread.Sleep(_sleepTime);
            }
        }

        private void addProgramThread()
        {
            if (_AddProgram)
            {
                // new program details
                int programSize = determineProgramSize(); 
                string programName = ""; 
                Color memoryColor = Color.FromArgb(_random.Next(14, 129), _random.Next(14, 129), _random.Next(14, 129)); 
                int requiredMemoryBlock = (int)Math.Ceiling(programSize / _frameSize); 
                double memoryFragmentation = (programSize / _frameSize) - (int)Math.Floor(programSize / _frameSize);

                for (int j = 0; j < requiredMemoryBlock; j++)
                {
                    programName = (char)_programName + "-" + j; // individual memory block name

                    int start = scanSpace(1, _memoryPhysical); // scan for space
                    bool couldHelp = true; // should find better method

                    while (start == -1 && couldHelp) 
                    {
                        couldHelp = FIFO(); // if no space, move to swap according to paging algorithm
                        start = scanSpace(1, _memoryPhysical); // determine if space is available
                    }
                    if (start != -1) 
                    {
                        addProgramToMemory(start, programName + " : " + programSize, memoryColor, _memoryPhysical); // add program to ram
                        _programInformation.Add(new double[] { _programName, j, programSize, start, start + 1 }); // add details to list
                        this.Invoke((MethodInvoker)delegate
                        {
                            comboBoxReadProgram.Items.Add(programName);
                        });
                    }
                    else
                    {
                        displayMessage("Program " + programName + " could not be added.");
                    }
                }
                displayMessage("Program " + (char)_programName + " is added to ram.");

                _programName++; // increment char number for next program name
                if (_programName == 91)
                {
                    _programName = 97;
                }
                if (_programName == 122)
                {
                    _AddProgram = false;
                }
            }
            else
            {
                _isSimulation = false;
                if (MessageBox.Show("Reset required. Press 'Yes' to reset.", "Simulation End", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        buttonReset.PerformClick();
                    });
                }
            }
            this.Invoke((MethodInvoker)delegate
            {
                componentsGUI(true); // enable GUI when done adding
            });
            Console.WriteLine("Simulation : " + _isSimulation);
            if (_isSimulation)
            {
                simulation(); // continue if simulation
            }
        }

        // ALGORITHMS

        private bool FIFO()
        {
            if (_programInformation.Count > 0)
            {
                int start = scanSpace(1, _memoryStorage); // check if there is space in swape
                while (start == -1) // remove programs till enough space is available
                {
                    removeProgramFromMemory((int)_infoSwap[0][_storedStart], (int)_infoSwap[0][_storedStop], _memoryStorage); // drop program from swap if no space
                    displayMessage("Program " + (char)_infoSwap[0][_storedName] + "-" + _infoSwap[0][_storedPage] + " is dropped.");
                    _infoSwap.RemoveAt(0); // remove program from swap information
                    start = scanSpace(1, _memoryStorage); // re-evaluate
                }

                // determine colour of existing block in use 
                Color memoryColour = _memoryPhysical[(int)_programInformation[0][_storedStart]].BackColor; // determine color of program moved to swap
                
                // remove program from physical
                removeProgramFromMemory((int)_programInformation[0][_storedStart], (int)_programInformation[0][_storedStop], _memoryPhysical); // remove from ram

                // add program to swap
                _statsMovedToSwap++;
                addProgramToMemory(start, (char)_programInformation[0][_storedName] + "-" + _programInformation[0][_storedPage] + " : " + _programInformation[0][_storedSize], memoryColour, _memoryStorage);

                // display when it is done
                displayMessage("Program " + (char)_programInformation[0][_storedName] + "-" + _programInformation[0][_storedPage] + " is moved to swap.");

                // transfer program data from physical to swap
                _programInformation[0][_storedStart] = start;
                _programInformation[0][_storedStop] = start + 1;
                _infoSwap.Add(_programInformation[0]);
                _programInformation.RemoveAt(0);

                // succesful
                return true;
            }
            else
            {
                // unsuccesful
                return false;
            }
        }
    }
}
