type state =
   struct
      val X: bool
      val Y: bool
      new(x: bool, y: bool) = { X = x; Y = y }
   end

let mainArray = [state(false,false); state(true,false); state(true,true); state(false,true) ]

let right = [ for i in 0..9 -> mainArray.[i%4]]
let left =  [ for i in 0..9 -> mainArray.[(20-i)%4]]

let bitCompares2 a b = [ a && b; a || b; a <> b]

let showChangeLogic (next:state, prev:state) =  
        [next.X; next.Y; prev.X; prev.Y]
        |> List.append (bitCompares2 next.X prev.X)
        |> List.append (bitCompares2 next.Y prev.Y)
   
let crossFun f l1 = 
  seq { for el1 in l1 do
          for el2 in l1 do
            yield f el1 el2 };;  

//type Adder = bool -> bool -> bool list
let crossFun2 f l1 = 
    seq { for i in 0..Seq.length(l1)-1 do 
            if i < Seq.length(l1)-1 then
                for j in i+1..Seq.length(l1)-1 do 
                    yield f (l1|>Seq.item(i)) (l1|>Seq.item(j)) }
//[[1;2];[13;22]] |> List.map(fun x-> x |> List.append([44]))
//crossFun bitCompares2 [true;false] |> List.concat
//let crossFun f x = x |> List.map(fun xi -> x |> List.map(fun xj -> f xi xj) ) |> List.concat
let appendComplexLogic X = X |> Seq.map (fun x -> (( (crossFun2 bitCompares2 x) |> List.concat) ) |> List.append x )
    
let nextPrevPair (Y:list<state>) = Y
                                 |> Seq.skip 1
                                 |> Seq.mapi (fun i x -> x, Y.Item(i))

let displayBit x =  printf "%d" (if x then 1 else 0)
let showAll G = 
    nextPrevPair G 
        |> Seq.map(showChangeLogic) 
        |> appendComplexLogic 
        |> Seq.iter (fun x -> x|> Seq.iter displayBit; printf "\n" )

showAll right
printf "\n\n"
showAll left

//let oop h = [h;h]
//let a = [for i in 0..4 -> oop (oop i) |> Seq.concat |> Seq.toArray]
//oop (oop 4) |> Seq.concat |> Seq.toArray

//let a2 s d = s + d
//crossFun2 a2 ([3;4;6])
//let f12 t = t |> List.map(fun i -> t |> List.iter(fun j -> yield j+i;))