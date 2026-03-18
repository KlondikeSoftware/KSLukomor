using System;
using System.Reactive.Disposables;
using Lukomor.Reactive;

namespace Lukomor.MVVM.Binders
{
    public abstract class ObservableCollectionBinder<T> : ObservableCollectionBinder 
    {
        public override Type ArgumentType => typeof(T);

        protected sealed override IDisposable BindInternal(IViewModel viewModel)
        {
            var propertyInfo = viewModel.GetType().GetProperty(PropertyName);
            var reactiveCollection = (IReadOnlyReactiveCollection<T>)propertyInfo.GetValue(viewModel);
        
            var addedSubscription = reactiveCollection.Added.Subscribe(OnItemAdded);
            var removedSubscription = reactiveCollection.Removed.Subscribe(OnItemRemoved);
            var compositeDisposable = new CompositeDisposable();
            
            compositeDisposable.Add(addedSubscription);
            compositeDisposable.Add(removedSubscription);
        
            return compositeDisposable;
        }
        
        protected abstract void OnItemAdded(T viewModel);
        protected abstract void OnItemRemoved(T value);
    }
}