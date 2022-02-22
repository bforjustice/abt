namespace MDWindow.ViewModels
{
    using MDWindow.Models;

    public class MDViewModel
    {
        private MDMainModel myModel;

        public MDViewModel()
        {
            this.myModel = new MDMainModel();
        }

        public void Initialize(string coinType)
        {
            this.myModel.Initialize(coinType);
        }

        public void StartMD(string coinType)
        {
            this.myModel.StartMD(coinType);
        }

        public void OpenPosition()
        {
            this.myModel.CallLoadPositions();
        }
    }
}