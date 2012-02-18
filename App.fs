namespace Ag

open System.Windows
open System.Windows.Controls

type App() as this = 
    inherit Application()

    do
        let grid = SmallGrid()
        for x = 0 to 20 do 
            if x % 2 = 0 then grid.Columns.Add(ColumnDefinition())
            else grid.Columns.Add(ColumnDefinition(Width=GridLength(100.0)))
        for y = 0 to 20 do grid.Rows.Add(RowDefinition())
        for y = 0 to 20 do
            for x = 0 to 20 do
                let child = TextBlock(Text="Hello " + x.ToString() + " " + y.ToString())
                child.SetValue(Grid.ColumnProperty, x)
                child.SetValue(Grid.RowProperty, y)
                grid.Children.Add child

        this.Startup.AddHandler(fun o e -> this.RootVisual <- grid)
