using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseImporter.Views.Behaviours
{
    public class ListBoxBehaviours
    {
        private static readonly Dictionary<ListBox, Capture> Associations =
            new Dictionary<ListBox, Capture>();


        public static readonly DependencyProperty ScrollOnNewItemProperty =
            DependencyProperty.RegisterAttached(
                "ScrollOnNewItem",
                typeof(bool),
                typeof(ListBoxBehaviours),
                new UIPropertyMetadata(false, OnScrollOnNewItemChanged));


        public static bool GetScrollOnNewItem(DependencyObject obj)
            => (bool) obj.GetValue(ScrollOnNewItemProperty);

        public static void SetScrollOnNewItem(DependencyObject obj, bool value)
            => obj.SetValue(ScrollOnNewItemProperty, value);

        public static void OnScrollOnNewItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ListBox listBox))
                return;

            var oldValue = (bool) e.OldValue;
            var newValue = (bool) e.NewValue;

            if (newValue == oldValue)
                return;

            if (newValue)
            {
                listBox.Loaded += OnListBoxLoaded;
                listBox.Unloaded += OnListBoxUnloaded;
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(listBox)["ItemsSource"];
                itemsSourcePropertyDescriptor.AddValueChanged(listBox, OnListBoxItemsSourceChanged);
            }
            else
            {
                listBox.Loaded -= OnListBoxLoaded;
                listBox.Unloaded -= OnListBoxUnloaded;
                if (Associations.ContainsKey(listBox))
                    Associations[listBox].Dispose();
                var itemsSourcePropertyDescriptor = TypeDescriptor.GetProperties(listBox)["ItemsSource"];
                itemsSourcePropertyDescriptor.RemoveValueChanged(listBox, OnListBoxItemsSourceChanged);
            }
        }

        private static void OnListBoxItemsSourceChanged(object sender, EventArgs e)
        {
            var listBox = (ListBox) sender;
            if (Associations.ContainsKey(listBox))
                Associations[listBox].Dispose();
            Associations[listBox] = new Capture(listBox);
        }

        private static void OnListBoxUnloaded(object sender, RoutedEventArgs e)
        {
            var listBox = (ListBox) sender;
            if (Associations.ContainsKey(listBox))
                Associations[listBox].Dispose();
            listBox.Unloaded -= OnListBoxUnloaded;
        }

        private static void OnListBoxLoaded(object sender, RoutedEventArgs e)
        {
            var listBox = (ListBox) sender;
            listBox.Loaded -= OnListBoxLoaded;
            Associations[listBox] = new Capture(listBox);
        }

        private class Capture : IDisposable
        {
            private readonly ListBox _listBox;
            private readonly INotifyCollectionChanged _incc;

            public Capture(ListBox listBox)
            {
                _listBox = listBox;
                _incc = listBox.ItemsSource as INotifyCollectionChanged;
                if (_incc != null)
                {
                    _incc.CollectionChanged += OnInccCollectionChanged;
                }
            }

            private void OnInccCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Add)
                    return;

                _listBox.ScrollIntoView(e.NewItems[0]);
                _listBox.SelectedItem = e.NewItems[0];
            }

            public void Dispose()
            {
                if (_incc != null)
                    _incc.CollectionChanged -= OnInccCollectionChanged;
            }
        }
    }
}