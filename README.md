# FlatlinerDOA.Controls
## Requirements
* .NET 8.0
* Avalonia 11.0.10 or later

## SelectingCanvas

![](https://raw.githubusercontent.com/FlatlinerDOA/FlatlinerDOA.Controls/main/images/SelectingCanvasDemo.gif)


A control for visually selecting descendant controls via a mouse drag selection box.

1. Add `xmlns:fd="https://github.com/FlatlinerDOA"` to your Xaml file
1. Define a `<fd:SelectingCanvas>` control
1. Add any selectable children to your control, or use the`<fd:SelectingCanvas>` in an `<ItemsPanelTemplate>` if you intend to data-bind your selectable items
1. To visually show what is and isn't selected use a style selector for your elements such as the following:

```
  <Style Selector="Rectangle[(fd|SelectingCanvas.IsSelectable)=False]">
      <Setter Property="Opacity" Value="0.5" />
    </Style>
    <Style Selector="Rectangle[(fd|SelectingCanvas.IsSelected)=True]">
      <Setter Property="Stroke" Value="Blue" />
    </Style>
 ```

### Example


`SelectingCanvasDemo.axaml`
```XAML
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="1024"
             xmlns:fd="https://github.com/FlatlinerDOA"
             xmlns:vm="using:FlatlinerDOA.Controls.Demo"
             x:DataType="vm:MainViewModel"
             x:Class="FlatlinerDOA.Controls.Demo.SelectingCanvasDemo">
  <UserControl.Styles>
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
  </UserControl.Styles>
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
</UserControl>
```

## InkCanvas

A natural feeling pen experience, useful for collecting signatures or as a nice drawing tool.

![](https://raw.githubusercontent.com/FlatlinerDOA/FlatlinerDOA.Controls/main/images/InkCanvasDemo.gif)


1. Add `xmlns:fd="https://github.com/FlatlinerDOA"` to your Xaml file
1. Define a `<fd:InkCanvas>` control
1. To clear currently you must currently call the `Children.Clear()`, I hope to improve this in future.
1. To export the ink to an SVG `XElement` call this.FindDescendantOfType<InkCanvas>().ToSVG() which you can then save to a file.

## VirtualizingWrapPanel

A virtualizing layout control for laying out a series of tiles of uniform width horizontally or uniform height vertically.

This is a work in progress.

## License

SelectingCanvas is licensed under the [MIT license](LICENSE.TXT).