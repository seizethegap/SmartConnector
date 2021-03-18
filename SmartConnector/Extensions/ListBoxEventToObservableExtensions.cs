using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConnector.Extensions
{
    public static class ListBoxEventToObservableExtensions
    {
        /// <summary>Converts CollectionChanged to an observable sequence.</summary>
        public static IObservable<NotifyCollectionChangedEventArgs> ToObservable<T>(this T source)
            where T : INotifyCollectionChanged
        {
            return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h);
        }
    }
}
