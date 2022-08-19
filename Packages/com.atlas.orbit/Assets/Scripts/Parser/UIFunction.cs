using System.Reflection;

namespace Atlas.Orbit.Parser {
    public class UIFunction {
        private UIRenderData renderData;
        private MethodInfo methodInfo;

        public UIFunction(UIRenderData renderData, MethodInfo methodInfo) {
            this.renderData = renderData;
            this.methodInfo = methodInfo;
        }

        public object Invoke(params object[] parameters) {
            return methodInfo.Invoke(renderData.Host, parameters);
        }
        public T Invoke<T>(params object[] parameters) {
            return (T) methodInfo.Invoke(renderData.Host, parameters);
        }
    }
}
