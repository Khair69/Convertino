using Convertino.Models;
using Convertino.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Convertino;
public partial class MainWindow : Window
{
    private readonly ObservableCollection<SourceFile> files = [];
    private readonly ConversionService service = new();
    private CancellationTokenSource? cancellation;
    public MainWindow()
    {
        InitializeComponent(); FilesList.ItemsSource = files;
        if (Environment.GetCommandLineArgs().Length > 1) AddFiles(Environment.GetCommandLineArgs().Skip(1));
    }
    private void Window_DragOver(object sender, DragEventArgs e) => e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    private void Window_Drop(object sender, DragEventArgs e) { if (e.Data.GetData(DataFormats.FileDrop) is string[] paths) AddFiles(paths); }
    private void AddFiles_Click(object sender, RoutedEventArgs e) { var dialog = new OpenFileDialog { Multiselect = true, Filter = "Supported files|*.*" }; if (dialog.ShowDialog() == true) AddFiles(dialog.FileNames); }
    private void AddFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths.Where(File.Exists).Where(x => !files.Any(f => string.Equals(f.Path, x, StringComparison.OrdinalIgnoreCase)))) files.Add(new SourceFile(path));
        var targets = FileClassifier.Targets(files); TargetBox.ItemsSource = targets; TargetBox.SelectedIndex = targets.Count > 0 ? 0 : -1;
        FilesList.Visibility = files.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        TargetBox.IsEnabled = ConvertButton.IsEnabled = files.Count > 0 && targets.Count > 0;
        StatusText.Text = files.Count == 0 ? "Ready" : $"{files.Count} file{(files.Count == 1 ? "" : "s")} ready";
    }
    private void Stop_Click(object sender, RoutedEventArgs e) => cancellation?.Cancel();
    private async void Convert_Click(object sender, RoutedEventArgs e)
    {
        if (TargetBox.SelectedItem is not string target) return;
        cancellation = new(); ConvertButton.IsEnabled = false; TargetBox.IsEnabled = false; StopButton.Visibility = Visibility.Visible; ProgressBar.Visibility = Visibility.Visible;
        var completed = 0;
        try
        {
            foreach (var file in files.ToArray())
            {
                StatusText.Text = $"Converting {file.Name}";
                var progress = new Progress<double>(value => ProgressBar.Value = (completed + value) / files.Count * 100);
                await service.ConvertAsync(file, target, progress, cancellation.Token); completed++;
            }
            ProgressBar.Value = 100; StatusText.Text = $"Done — {completed} file{(completed == 1 ? "" : "s")} converted";
        }
        catch (OperationCanceledException) { StatusText.Text = "Conversion stopped"; }
        catch (Exception ex) { StatusText.Text = ex.Message; }
        finally { cancellation.Dispose(); cancellation = null; StopButton.Visibility = Visibility.Collapsed; ConvertButton.IsEnabled = files.Count > 0; TargetBox.IsEnabled = files.Count > 0; }
    }
}
