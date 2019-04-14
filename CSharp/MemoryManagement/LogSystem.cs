using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManagement
{
    class LogSystem
    {
        private int addProgram = 0;
        private int addFailed = 0;
        private int readProgram = 0;
        private int readFailed = 0;
        private int pageFault = 0;
        private int pageFaultResolved = 0;
        private int pageFaultUnresolved = 0;
        private int totalEntriesTLB = 0;
        private int readTLBSuccess = 0;
        private int readTLBFailed = 0;
        private int movedToSwap = 0;
        private int movedToPhysical = 0;
        private int totalPages = 0;
        private int totalPagesDropped = 0;

        public void logAdd()
        {
            ++addProgram;
        }

        public void logRead()
        {
            ++readProgram;
        }

        public void logAddPage()
        {
            ++totalPages;
        }

        public void logDroppedPage()
        {
            ++totalPagesDropped;
        }

        public void logPageFault()
        {
            ++pageFault;
        }

        public void logPageFaultResolved()
        {
            ++pageFaultResolved;
        }

        public void logPageFaultUnresolved()
        {
            ++pageFaultUnresolved;
        }

        public void logMoveToSwap()
        {
            ++movedToSwap;
        }

        public void logMoveToPhysical()
        {
            ++movedToPhysical;
        }

        public void logEntryTLB()
        {
            ++totalEntriesTLB;
        }

        public void logSuccessReadTLB()
        {
            ++readTLBSuccess;
        }

        public void logFailedReadTLB()
        {
            ++readTLBFailed;
        }

        public void logReadFailed()
        {
            ++readFailed;
        }

        public void logAddFailed()
        {
            ++readFailed;
        }

        public string getLog()
        {
            string log = "SESSION LOG DETAILS\n";
            log += "\nPrograms added: " + addProgram;
            log += "\nPrograms failed to add: " + addFailed;
            log += "\nTotal pages this session: " + totalPages;
            log += "\nTotal pages dropped this session: " + totalPagesDropped;
            log += "\nProgram memory allocation read: " + readProgram;
            log += "\nProgram memory allocation read failed: " + readFailed;
            log += "\nPage faults encountered: " + pageFault;
            log += "\nPage faults resolved: " + pageFaultResolved;
            log += "\nPage faults unresolved: " + pageFaultUnresolved;
            log += "\nTLB total entries: " + totalEntriesTLB;
            log += "\nTLB read, total successful: " + readTLBSuccess;
            log += "\nTLB read, total failed: " + readTLBFailed;
            log += "\nPrograms pages moved to swap: " + movedToSwap;
            log += "\nSwap program pages moved to physical: " + movedToPhysical;
            log += "\n";
            return log;
        }
    }
}
