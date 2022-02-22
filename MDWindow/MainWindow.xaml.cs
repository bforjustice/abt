namespace MDWindow
{
    using MDWindow.ViewModels;
    using System.Windows;
    using System.Windows.Documents;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private log4net.Appender.MemoryAppender logger;

        private string filterWord = string.Empty;

        private FlowDocument doc;
        private Paragraph paragraph;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MDViewModel();
        }

        private void MDStartBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MDViewModel)this.DataContext).Initialize(this.CoinList.SelectionBoxItem.ToString());
            ((MDViewModel)this.DataContext).StartMD(this.CoinList.SelectionBoxItem.ToString());
        }

        private void OpenOrderLoadBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OpenPositionLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MDViewModel)this.DataContext).OpenPosition();
        }

        private void ClearOrderLoadBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ClearPositionLoadBtn_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}