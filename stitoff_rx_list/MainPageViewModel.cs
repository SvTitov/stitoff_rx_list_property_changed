using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using DynamicData;

namespace stitoff_rx_list
{
    public class MainPageViewModel: BaseNotifyPropertyChanged
    {
        private IDisposable _selectedSub;

        private SourceList<Foo> _source;
        private ReadOnlyObservableCollection<Foo> _list;
        private bool isAnySelected;

        public ReadOnlyObservableCollection<Foo> List { get => _list; }

        public bool IsAnySelected
        {
            get => isAnySelected;
            set
            {
                isAnySelected = value;
                NotifyPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            _source = new SourceList<Foo>();
            var connect = _source.Connect()
               .Bind(out _list);

            _selectedSub = connect
                .WhenPropertyChanged(foo => foo.IsSelected, false)
                .ObserveOn(DefaultScheduler.Instance)
                .Subscribe(_ => OnSelectChanged());

            _source.AddRange(new[]
            {
                new Foo("Select me!"),
                new Foo("Select me!")
            });
        }

        private void OnSelectChanged()
        {
            var result = List.Any(x => x.IsSelected);
            IsAnySelected = result;
        }
    }

    public class Foo : BaseNotifyPropertyChanged
    {
        private string title;
        private bool isSelected;

        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Foo(string title)
        {
            Title = title;
        }
    }

    public class BaseNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
