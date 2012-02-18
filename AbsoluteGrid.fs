namespace Ag

open System
open System.Collections.Generic
open System.Windows
open System.Windows.Controls

type AbsoluteGrid () =
    inherit Panel ()
    let columns = List<double>()
    let rows = List<double>()
    
    member grid.Columns = columns :> IList<double>
    member grid.Rows = rows :> IList<double>
    
    override grid.MeasureOverride(availableSize:Size) =
        Size(columns |> Seq.sum, rows |> Seq.sum)

    override grid.ArrangeOverride(finalSize:Size) =
        let xs = columns |> Seq.scan (+) 0.0 |> Seq.toArray
        let ys = rows |> Seq.scan (+) 0.0 |> Seq.toArray
        for child in grid.Children do
            let child = child :?> FrameworkElement
            let column = Grid.GetColumn(child)
            let row = Grid.GetRow(child)
            let r = Rect(xs.[column], ys.[row], columns.[column], rows.[row])
            child.Arrange r
        finalSize


