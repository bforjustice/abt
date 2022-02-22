namespace MDWindow.UserControls
{
    using log4net.Core;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class OrderBookCtrlLogView : UserControl
    {
        private log4net.Appender.MemoryAppender logger;

        private string filterWord = string.Empty;

        private FlowDocument doc;

        private Paragraph paragraph;

        private string loggerName = string.Empty;

        public OrderBookCtrlLogView()
        {
            InitializeComponent();

            this.loggerName = "OrderBookControlLogger";
            DataContext = doc = new FlowDocument();

            doc.Blocks.Add(paragraph = new Paragraph());

            logger = new log4net.Appender.MemoryAppender();
            log4net.Config.BasicConfigurator.Configure(logger);

            Task.Factory.StartNew(AddDataLoop);
        }

        private void AddDataLoop()
        {
            while (true)
            {
                Thread.Sleep(10);
                Dispatcher.BeginInvoke((Action)(AddMessages));
            }
        }

        private void AddMessages()
        {
            LoggingEvent[] events = logger.GetEvents();
            if (events != null && events.Length > 0)
            {
                logger.Clear();
                foreach (LoggingEvent ev in events)
                {
                    if (ev.LoggerName.Equals(loggerName))
                    {
                        var run = new Run(string.Concat(ev.LoggerName, ": [", ev.TimeStamp, "] :", ev.RenderedMessage));
                        run.FontFamily = new FontFamily("Courier New");
                        run.FontSize = 12;
                        paragraph.Inlines.Add(run);
                        paragraph.Inlines.Add(new LineBreak());
                    }
                }
            }
        }
    }
}