using System;
using System.Collections.Specialized;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using SmartConnector.Extensions;

namespace SmartConnector.Behaviors
{
    public class ScrollOnNewItemBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty IsActiveScrollOnNewItemProperty = DependencyProperty.Register(
            name: "IsActiveScrollOnNewItem",
            propertyType: typeof(bool),
            ownerType: typeof(ScrollOnNewItemBehavior),
            typeMetadata: new PropertyMetadata(defaultValue: true, propertyChangedCallback: PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Intent: immediately scroll to the bottom if our dependency property changes.
            ScrollOnNewItemBehavior behavior = dependencyObject as ScrollOnNewItemBehavior;
            if (behavior == null)
            {
                return;
            }

            behavior.IsActiveScrollOnNewItemMirror = (bool)dependencyPropertyChangedEventArgs.NewValue;

            if (behavior.IsActiveScrollOnNewItemMirror == false)
            {
                return;
            }

            ListboxScrollToBottom(behavior.ListBox);
        }

        public bool IsActiveScrollOnNewItem
        {
            get { return (bool)this.GetValue(IsActiveScrollOnNewItemProperty); }
            set { this.SetValue(IsActiveScrollOnNewItemProperty, value); }
        }

        public bool IsActiveScrollOnNewItemMirror { get; set; } = true;

        protected override void OnAttached()
        {
            this.AssociatedObject.Loaded += this.OnLoaded;
            this.AssociatedObject.Unloaded += this.OnUnLoaded;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= this.OnLoaded;
            this.AssociatedObject.Unloaded -= this.OnUnLoaded;
        }

        private IDisposable rxScrollIntoView;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var changed = this.AssociatedObject.ItemsSource as INotifyCollectionChanged;
            if (changed == null)
            {
                return;
            }

            // Intent: If we scroll into view on every single item added, it slows down to a crawl.
            this.rxScrollIntoView = changed
                .ToObservable()
                .ObserveOn(new EventLoopScheduler(ts => new Thread(ts) { IsBackground = true }))
                .Where(o => this.IsActiveScrollOnNewItemMirror == true)
                .Where(o => o.NewItems?.Count > 0)
                .Sample(TimeSpan.FromMilliseconds(180))
                .Subscribe(o =>
                {
                    this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ListboxScrollToBottom(this.ListBox);
                    }));
                });
        }

        ListBox ListBox => this.AssociatedObject;

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            this.rxScrollIntoView?.Dispose();
        }

        /// <summary>
        /// Scrolls to the bottom. Unlike other methods, this works even if there are duplicate items in the listbox.
        /// </summary>
        private static void ListboxScrollToBottom(ListBox listBox)
        {
            if (VisualTreeHelper.GetChildrenCount(listBox) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }
    }
}