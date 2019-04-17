using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryManagement
{
    class LogSystem
    {
        private int totalPrograms = 0;
        private int totalPageReads = 0;
        private int totalPageReadsSuccesful = 0;
        private int totalPageReadsFailed = 0;
        private int totalPageFaults = 0;
        private int totalPageFaultsResolved = 0;
        private int totalUnswappedPages = 0;
        private int totalHitsTLB = 0;
        private int totalMissesTLB = 0;
        private int totalSwappedPages = 0;
        private int totalPages = 0;
        private int totalDroppedPages = 0;
        private double totalPagesFragAmount = 0;
        private double totalPagesFragSize = 0;

        public void logFramentation(int pages, int programSize)
        {
            totalPagesFragAmount += (double)pages;
            totalPagesFragSize += (double)programSize;
        }

        public void logProgramAdded()
        {
            totalPrograms++;
        }

        public void logPageRead()
        {
            totalPageReads++;
        }

        public void logTLBHit()
        {
            totalHitsTLB++;
        }

        public void logTLBMiss()
        {
            totalMissesTLB++;
        }

        public void logSuccessfulPageRead()
        {
            totalPageReadsSuccesful++;
        }

        public void logFailedPageRead()
        {
            totalPageReadsFailed++;
        }

        public void logPageFaultsResolved()
        {
            totalPageFaultsResolved++;
        }

        public void logPageFaults()
        {
            totalPageFaults++;
        }

        public void logPageDrop()
        {
            totalDroppedPages++;
        }

        public void logPageSwap()
        {
            totalSwappedPages++;
        }

        public void logPageAdd()
        {
            totalPages++;
        }

        public void logPageUnswap()
        {
            totalUnswappedPages++;
        }


        public string getLog()
        {
            string log = "SESSION LOG DETAILS\n";
            log += "\nTotal programs added: " + totalPrograms;
            log += "\nTotal pages: " + totalPages;
            log += "\nTotal pages swapped: " + totalSwappedPages;
            log += "\nTotal pages unswapped: " + totalUnswappedPages;
            log += "\nTotal pages dropped: " + totalDroppedPages;
            log += "\nTotal page reads: " + totalPageReads;
            log += "\nTotal succesful page reads: " + totalPageReadsSuccesful;
            log += "\nTotal failed page reads: " + totalPageReadsFailed;
            log += "\nTotal page faults: " + totalPageFaults;
            log += "\nTotal page faults resolved: " + totalPageFaultsResolved;
            log += "\nTotal TLB hits: " + totalHitsTLB;
            log += "\nTotal TLB misses: " + totalMissesTLB;
            log += "\nAverage fragmentation per page: " + (totalPagesFragSize / totalPagesFragAmount);
            log += "\n";
            return log;
        }
    }
}
