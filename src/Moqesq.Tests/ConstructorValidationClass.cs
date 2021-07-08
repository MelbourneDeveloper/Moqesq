using System;
using System.Threading.Tasks;

namespace Moqesq.Tests
{
    public class ConstructorValidationClass
    {
        IValueHolder valueHolder;

        public ConstructorValidationClass(IValueHolder valueHolder)
        {
            if (valueHolder.Value == null) throw new ArgumentNullException(nameof(valueHolder));
            this.valueHolder = valueHolder;
        }

        public Task<object> GetValue() => Task.FromResult(valueHolder.Value);

    }

    public interface IValueHolder
    {
        public object Value { get; }
    }
}
