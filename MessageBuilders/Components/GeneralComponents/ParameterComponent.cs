namespace MessageBuilders.Components.GeneralComponents
{
    using LogTrace.Interfaces;
    using MessageBuilders.Interfaces;
    using System;
    using MessageBuilders.Components;
    public class ParameterComponent : ComponentBase<object>, IParameterComponent<object>
    {
        protected readonly static ILogger myLogger = LogTraceService.Instance.GetLogger("CommLogger");

        private object myValue;

        private string myType = string.Empty;

        public ParameterComponent(string key, string type)
        {
            this.myKey = key;
            this.myType = type;
        }

        public override object Result => this.myValue;

        public void SetValue(object value)
        {
            this.myValue = this.ConvertValue(value);
        }

        private object ConvertValue(object value)
        {
            try
            {
                switch (myType)
                {
                    case "Long":
                        return Convert.ToInt64(value);

                    case "Integer":
                        return Convert.ToInt32(value);

                    case "Double":
                        return Convert.ToDouble(value);

                    case "String":
                        return Convert.ToString(value);

                    case "Boolean":
                        return Convert.ToBoolean(value);

                    default:
                        return value;
                }
            }
            catch
            {
                myLogger.Error($"Parse Value Error : {value}");

                if (myType.Equals("Long") ||
                    myType.Equals("Integer") ||
                    myType.Equals("Double"))
                {
                    return 0;
                }
                else if (myType.Equals("Boolean"))
                {
                    return false;
                }
                else
                {
                    return "0";
                }
            }
        }
    }
}