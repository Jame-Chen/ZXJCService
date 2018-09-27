using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using WcfServiceZXJC;
using System.IO;
using log4net.Config;

namespace TestForms
{
    public partial class Form1 : Form
    {

        //ZXJCJob zxjcjob = new ZXJCJob();
        ILog logger;
        public Form1()
        {
            InitializeComponent();
        }
        private static void InitLog4Net()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            InitLog4Net();
             logger = LogManager.GetLogger(typeof(ZXJCJob));
            try
            {
                logger.Info("开始");
              // System.Threading.Timer threadTimer = new System.Threading.Timer(new System.Threading.TimerCallback(TenantSynchronize), null, 3000, 6000);
                ZXJCJob x = new ZXJCJob();
                //x.UpdateDevice();
                //object obj = null;
                //x.DeviceSynchronize(obj);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void TenantSynchronize(object obj) {
            logger.Info("OK");
        }
    }
}
