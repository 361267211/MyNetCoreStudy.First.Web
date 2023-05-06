using System;

namespace NetCoreStudy.Core
{
    public class TaskHandler
    {
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private readonly Action Action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="time"></param>
        public TaskHandler(Action action, int time)
        {
            this.Action = action;
            timer.Enabled = true;
            timer.Interval = time;//Interval单位:毫秒           
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(DoTask);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoTask(object sender, System.Timers.ElapsedEventArgs e)
        {
            Action();
        }
    }
}
