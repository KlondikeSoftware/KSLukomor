using System;
using System.Collections.Generic;
using System.Linq;

namespace Lukomor.Reactive
{
    public sealed class ReactiveProperty<T> : IReactiveProperty<T>
    {
        private T _value;
        private bool _hasValue;
        private readonly List<IObserver<T>> _observers = new();

        public T Value
        {
            get => _value;
            set
            {
                if (!_hasValue)
                {
                    _hasValue = true;
                    _value = value;

                    NotifyAboutNewValue(value);
                }
                else if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;

                    NotifyAboutNewValue(value);
                }
            }
        }
        
        public bool HasValue => _hasValue;

        public void Dispose()
        {
            foreach (var observer in _observers.ToList())
            {
                observer.OnCompleted();
            }
            _observers.Clear();
            _hasValue = false;
            _value = default;
        }

        public ReactiveProperty() { }

        public ReactiveProperty(T valueByDefault)
        {
            _value = valueByDefault;
            _hasValue = true;
        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            
            if (_hasValue)
            {
                observer.OnNext(_value);
            }

            return new ReactiveSubscription<T>(this, observer);
        }
        
        public void Unsubscribe(IObserver<T> observer)
        {
            if (_observers.Contains(observer))
            {
                _observers.Remove(observer);
            }
        }

        /// <summary>
        /// Set value and invoke event;
        /// </summary>
        /// <param name="newValue"></param>
        public void Set(T newValue)
        {
            _value = newValue;
            _hasValue = true;
            NotifyAboutNewValue(newValue);
        }

        private void NotifyAboutNewValue(T newValue)
        {
            var count = _observers.Count;
                    
            for (var i = 0; i < count; i++)
            {
                try
                {
                    _observers[i].OnNext(newValue);
                }
                catch (Exception e)
                {
                    continue;
                }
               
            }
        }
    }
}