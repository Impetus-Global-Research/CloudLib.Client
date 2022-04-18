using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CloudLib.Client.Core.Models;
using CloudLib.Client.Core.Services;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CloudLib.Client.ViewModels
{
    public class TelerikDataGridViewModel : ObservableObject
    {
        public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

        public TelerikDataGridViewModel()
        {
        }

        public async Task LoadDataAsync()
        {
            Source.Clear();

            // Replace this with your actual data
            var data = await SampleDataService.GetGridDataAsync();

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }
    }
}
