Get-ChildItem -Path .\Results\Processed\IndividualTests -Recurse -Directory -ErrorAction SilentlyContinue |
    ForEach-Object -Process {
        $args = "AlgorithmErrorVisualizer.r" , $_.FullName
        & "rscript.exe" $args
     }