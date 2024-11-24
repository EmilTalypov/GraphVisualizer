using GraphVisualizer.ViewModels;

namespace GraphVisualizer;

public partial class MainPage : ContentPage
{
    public MainPage(GraphViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
