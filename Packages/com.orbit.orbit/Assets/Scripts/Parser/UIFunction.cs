using System.Reflection;

namespace Orbit.Parser {
    using Util;

    public class UIFunction {
        private UIRenderData renderData;
        private MethodInfo methodInfo;

        public UIFunction(UIRenderData renderData, MethodInfo methodInfo) {
            this.renderData = renderData;
            this.methodInfo = methodInfo;
        }

        public object Invoke(object parameter) {
            return methodInfo.Invoke(renderData.Host, ArrayParameters<object>.Single(parameter));
        }
        public object Invoke(params object[] parameters) {
            return methodInfo.Invoke(renderData.Host, parameters);
        }
        public T Invoke<T>(object parameter) {
            return (T) methodInfo.Invoke(renderData.Host, ArrayParameters<object>.Single(parameter));
        }
        public T Invoke<T>(params object[] parameters) {
            return (T) methodInfo.Invoke(renderData.Host, parameters);
        }
    }
}
