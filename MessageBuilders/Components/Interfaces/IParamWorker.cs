namespace MessageBuilders.Components.Interfaces
{
    using MessageBuilders.Components.RESTBuilder.OptionModels;
    using MessageBuilders.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IParamWorker<T>
    {
        string Key { get; }

        void Do(T opts);

        T Result { get; }

        void SetComponent(IBlockComponent<object> param);
    }
}
