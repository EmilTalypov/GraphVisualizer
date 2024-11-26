using GraphVisualizer.GraphView;

namespace GraphVisualizer;

public partial class MainPage : ContentPage
{
    public MainPage(GraphViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
