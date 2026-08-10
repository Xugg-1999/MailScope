using EmailAnalyzer.ViewModels;
using System.Windows;
using System.Diagnostics;
using System.Windows.Navigation;
namespace EmailAnalyzer;
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    public MainWindow()
    {
        InitializeComponent();
        _viewModel =
            new MainViewModel();
        DataContext =
            _viewModel;
    }
    private void Hyperlink_RequestNavigate(
    object sender,
    RequestNavigateEventArgs e)
    {
        Process.Start(
            new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            }
        );
    }
    /// <summary>
    /// PasswordBox不能直接Binding
    /// 手动同步密码到ViewModel
    /// </summary>
    private void PasswordBox_PasswordChanged(
        object sender,
        RoutedEventArgs e
    )
    {
        if (sender is System.Windows.Controls.PasswordBox box)
        {
            _viewModel.Password =
                box.Password;
        }
    }
}
