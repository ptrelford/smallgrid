namespace Ag

open System
open System.Collections.Generic
open System.Windows
open System.Windows.Controls

type AutoGrid () = 
    inherit Panel ()
    let locateChildren (grid:Panel) =
        grid.Children |> Seq.map (fun child ->
            let element = child :?> FrameworkElement
            child, Grid.GetColumn(element), Grid.GetRow(element)
        ) |> Seq.toArray
    let measureChildren children =
        let availableSize = Size(Double.PositiveInfinity,Double.PositiveInfinity)
        let maxColumn, maxRow = 
            Array.fold (fun (mc,mr) (_,r,c) -> max c mc, max r mr) (0,0) children
        let widths = Array.create (maxColumn+1) 0.0
        let heights = Array.create (maxRow+1) 0.0
        for (child:UIElement,row,column) in children do
            child.Measure(availableSize)
            widths.[column] <- max widths.[column] child.DesiredSize.Width
            heights.[row] <- max heights.[row] child.DesiredSize.Height
        widths,heights
    override grid.MeasureOverride(availableSize:Size) =
        let widths, heights = grid |> locateChildren |> measureChildren
        Size(Array.sum widths, Array.sum heights)
    override grid.ArrangeOverride(finalSize:Size) =
        let children = locateChildren grid
        let widths, heights = measureChildren children
        let xs, ys = Array.scan (+) 0.0 widths, Array.scan (+) 0.0 heights
        for (child:UIElement,row,column) in children do
            Rect(xs.[column], ys.[row], widths.[column], heights.[row])
            |> child.Arrange
        finalSize