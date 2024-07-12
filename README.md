# Avalonia.Controls.SelectingCanvas
![](images/screenshot.png)

Avalonia Control for visually selecting controls via a mouse drag selection box.

## Requirements
* .NET 8.0
* Avalonia 11.0.10 or later

## Using SelectingCanvas

`MainWindow.xaml`
```XAML
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d" d:DesignWidth="600" d:DesignHeight="800"
        xmlns:fd="https://github.com/FlatlinerDOA"
        x:Class="Avalonia.Controls.SelectingCanvas.Demo.MainWindow"
        xmlns:vm="using:Avalonia.Controls.SelectingCanvas.Demo"
        x:DataType="vm:MainViewModel"
        Title="SelectingCanvas Demo">
     <Window.Styles>
    <Style Selector="Canvas">
      <Setter Property="Background" Value="Gray" />
    </Style>
    <Style Selector="Canvas[(fd|SelectingCanvas.IsSelected)=True]">
      <Setter Property="Background" Value="DarkGray" />
    </Style>
    <Style Selector="Rectangle">
      <Setter Property="Fill" Value="Crimson" />
      <Setter Property="Stroke" Value="White" />
      <Setter Property="StrokeThickness" Value="2" />
    </Style>
    <Style Selector="Rectangle[(fd|SelectingCanvas.IsSelectable)=False]">
      <Setter Property="Opacity" Value="0.5" />
    </Style>
    <Style Selector="Rectangle[(fd|SelectingCanvas.IsSelected)=True]">
      <Setter Property="Stroke" Value="Blue" />
    </Style>
  </Window.Styles>
  <Grid RowDefinitions="Auto,*">
    <StackPanel Orientation="Vertical"
                Margin="40"
                HorizontalAlignment="Center" Grid.Row="0" Grid.Column="1">
      <TextBlock Text="Mouse Down - Start Selecting"/>
      <TextBlock Text="Mouse Up - Release Selection"/>
      <TextBlock Text="Bind the attached property SelectingCanvas.IsSelected to a property on your view model" />
      <TextBlock Text="Setting SelectingCanvas.IsSelectable to False disallows selection of an item (all items are selectable by default)" TextWrapping="Wrap"/>
      <TextBlock Text="NOTE: Selection is recursive all descendants inside selection area will be selected." TextWrapping="Wrap"/>
      <CheckBox IsChecked="{Binding IsChildRectangleSelected}">Binding: MainViewModel.IsChildRectangleSelected</CheckBox>
    </StackPanel>
    <fd:SelectingCanvas 
      Grid.Row="1"
      Background="LightGray" 
      Width="500" Height="500"
      SelectionFill="#336495ed"
      SelectionStroke="CornflowerBlue"
      SelectionStrokeThickness="1">
      <!-- All descendant children are selectable by default (including this child canvas) -->
      <Canvas Canvas.Left="100" Canvas.Top="100" Width="300" Height="300">
        <!-- Pre-selected control -->
        <Rectangle fd:SelectingCanvas.IsSelected="True" Canvas.Left="100" Canvas.Top="100" Width="50" Height="50" />
        <!-- Selection disabled -->
        <Rectangle fd:SelectingCanvas.IsSelectable="False" Canvas.Left="150" Canvas.Top="150" Width="50" Height="50" />
        <!-- Data bind selection to a view model -->
        <Rectangle fd:SelectingCanvas.IsSelected="{Binding IsChildRectangleSelected}" Canvas.Left="200" Canvas.Top="200" Width="50" Height="50" />
        <Rectangle Canvas.Left="300" Canvas.Top="300" Width="50" Height="50" />
        <!-- Out of bounds elements are not selectable -->
        <Rectangle Canvas.Left="400" Canvas.Top="400" Width="50" Height="50" />
      </Canvas>
    </fd:SelectingCanvas>
  </Grid>
</Window>
```

## License

SelectingCanvas is licensed under the [MIT license](LICENSE.TXT).