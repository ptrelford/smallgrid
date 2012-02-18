namespace Ag

open System
open System.Collections.Generic
open System.Windows
open System.Windows.Controls

module Elements =
    let iteri f (children:UIElement seq) =
        for child in children do
            let element = child :?> FrameworkElement
            let column = Grid.GetColumn(element)
            let row = Grid.GetRow(element)
            f column row child

[<AutoOpen>]
module Measure =
    let measureGrid (columns:IList<ColumnDefinition>) (rows:IList<RowDefinition>) (grid:Panel) =
        let widths = 
            columns 
            |> Seq.map (fun column ->
                if column.Width.IsAbsolute 
                then column.Width.Value
                else 0.0
            ) 
            |> Seq.toArray

        let heights = 
            rows
            |> Seq.map (fun row ->
                if row.Height.IsAbsolute 
                then row.Height.Value
                else 0.0
            ) 
            |> Seq.toArray

        let availableSize = Size(Double.PositiveInfinity, Double.PositiveInfinity)

        let measureChild (child:UIElement) column row =
            if columns.[column].Width.IsAuto || columns.[column].Width.IsStar ||
               rows.[row].Height.IsAuto || rows.[row].Height.IsStar
            then
                child.Measure(availableSize)
                if columns.[column].Width.IsAuto || columns.[column].Width.IsStar
                then widths.[column] <- max child.DesiredSize.Width widths.[column]
                if rows.[row].Height.IsAuto || rows.[row].Height.IsStar
                then heights.[column] <- max child.DesiredSize.Height heights.[column]

        grid.Children
        |> Elements.iteri (fun column row child ->
            if column >= 0 && column < columns.Count && 
               row >= 0 && row < rows.Count 
            then measureChild child column row
        )
        
        widths, heights

type SmallGrid () =
    inherit Panel()

    let columns = List<ColumnDefinition>()
    let rows = List<RowDefinition>()
    
    member grid.Columns = columns :> IList<ColumnDefinition>
    member grid.Rows = rows :> IList<RowDefinition>
    
    override grid.MeasureOverride(availableSize:Size) =
        let widths, heights = measureGrid columns rows grid
        let totalWidth =
            columns 
            |> Seq.mapi (fun index column ->
                if column.Width.IsAuto || column.Width.IsStar
                then widths.[index]
                else column.Width.Value
            )
            |> Seq.sum
        let totalHeight =
            rows
            |> Seq.mapi (fun index row ->
                if row.Height.IsAuto || row.Height.IsStar
                then heights.[index]
                else row.Height.Value
            )
            |> Seq.sum
        Size(totalWidth, totalHeight)

    override grid.ArrangeOverride(finalSize:Size) =
        let widths, heights = measureGrid columns rows grid
        let xs = widths |> Array.scan (fun x width -> x + width) 0.0
        let ys = heights |> Array.scan (fun y height -> y + height) 0.0
        let dimensions column row =
            let width =
                if columns.[column].Width.IsAuto || columns.[column].Width.IsStar
                then widths.[column]
                else columns.[column].Width.Value
            let height =
                if rows.[row].Height.IsAuto || rows.[row].Height.IsStar
                then heights.[row]
                else rows.[row].Height.Value
            width, height
        grid.Children |> Elements.iteri (fun column row child ->
            if column >= 0 && column < columns.Count && 
               row >= 0 && row < rows.Count 
            then 
                let width, height = dimensions column row
                let x, y = xs.[column], ys.[row]
                Rect(x, y, width, height) 
                |> child.Arrange
        )
        finalSize