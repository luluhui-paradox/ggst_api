using ggst_api.ScheduleTask;
using ggst_api.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace TestProject1
{
    public class SchedulerTester
    {
        private readonly IUpdateDbSchedule _updateDbSchedule;

        private readonly ITestOutputHelper _logger;

        public SchedulerTester(IUpdateDbSchedule updateDbSchedule,ITestOutputHelper logger)
        {
            _updateDbSchedule = updateDbSchedule;
            _logger = logger;
        }

        [Fact]
        public async void updateTest() { 
            int res=await _updateDbSchedule.updateDb();
            _logger.WriteLine($"update items: {res}");
        }


        [Fact]
        public void selectAllDistinctTest() {

            var res= _updateDbSchedule.getAllDistinctFromDb();
            _logger.WriteLine($"res : {res.Count} ");
        }
    }
}
